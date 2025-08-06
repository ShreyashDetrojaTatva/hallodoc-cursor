using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Repositories.Repositories.RequestRepository;
using HalloDoc.Repositories.Repositories.PhysicianRepository;
using HalloDoc.Common.Constants;
using HalloDoc.Services.Helpers;
using HalloDoc.Common.Utility;
using System.Text;
using OfficeOpenXml;

namespace HalloDoc.Services.Services
{
    public class PhysicianDashboardService : IPhysicianDashboardService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IWorkContext _workContext;

        public PhysicianDashboardService(
            IRequestRepository requestRepository,
            IPhysicianRepository physicianRepository,
            IWorkContext workContext)
        {
            _requestRepository = requestRepository;
            _physicianRepository = physicianRepository;
            _workContext = workContext;
        }

        public async Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStateAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request)
        {
            var physicianId = await GetCurrentPhysicianId();
            if (physicianId == null)
            {
                throw new InvalidOperationException("User is not a physician");
            }

            // Get the request statuses that map to the dashboard state
            var requestStatuses = RequestStatusMapper.GetStatusIdsForState(state);
            
            // Get requests for the specific physician and state
            var requests = await _requestRepository.GetRequestsByStatusIdsForPhysicianAsync(
                requestStatuses,
                request,
                physicianId.Value
            );

            return requests;
        }

        public async Task<List<DashboardStateDto>> GetStateCountsAsync()
        {
            var physicianId = await GetCurrentPhysicianId();
            if (physicianId == null)
            {
                throw new InvalidOperationException("User is not a physician");
            }

            var stateCounts = new List<DashboardStateDto>();
            
            // Define the states that physicians can see
            var physicianStates = new[]
            {
                DashboardRequestStatus.New,
                DashboardRequestStatus.Pending,
                DashboardRequestStatus.Active,
                DashboardRequestStatus.Conclude
            };

            foreach (var state in physicianStates)
            {
                var requestStatuses = RequestStatusMapper.GetStatusIdsForState(state);
                var count = await _requestRepository.GetRequestCountByStatusIdsForPhysicianAsync(
                    requestStatuses,
                    physicianId.Value
                );

                stateCounts.Add(new DashboardStateDto
                {
                    Id = (int)state,
                    Name = GetDashboardStatusDisplayName(state),
                    Count = count,
                    Color = GetStateColor(state),
                    Icon = GetStateIcon(state)
                });
            }

            return stateCounts;
        }

        public async Task<byte[]> ExportRequestsAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request)
        {
            var physicianId = await GetCurrentPhysicianId();
            if (physicianId == null)
            {
                throw new InvalidOperationException("User is not a physician");
            }

            var requests = await GetRequestsByStateAsync(state, request);
            return GenerateExcelExport(requests.Items, $"Physician Dashboard - {state}");
        }

        public async Task<byte[]> ExportAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request)
        {
            var physicianId = await GetCurrentPhysicianId();
            if (physicianId == null)
            {
                throw new InvalidOperationException("User is not a physician");
            }

            // Get all requests for the physician (all states)
            var allStates = new[]
            {
                DashboardRequestStatus.New,
                DashboardRequestStatus.Pending,
                DashboardRequestStatus.Active,
                DashboardRequestStatus.Conclude
            };

            var allRequests = new List<RequestDataDto>();
            foreach (var state in allStates)
            {
                var stateRequests = await GetRequestsByStateAsync(state, request);
                allRequests.AddRange(stateRequests.Items);
            }

            return GenerateExcelExport(allRequests, "Physician Dashboard - All Requests");
        }

        public async Task<bool> AcceptRequestAsync(AcceptRequestDto acceptRequest)
        {
            try
            {
                var physicianId = await GetCurrentPhysicianId();
                if (physicianId == null)
                {
                    throw new InvalidOperationException("User is not a physician");
                }

                // Get the request to verify it belongs to this physician
                var request = await _requestRepository.GetByIdAsync(acceptRequest.RequestId);
                if (request == null)
                {
                    return false;
                }

                // Verify the request is assigned to this physician
                if (request.PhysicianId != physicianId.Value)
                {
                    return false;
                }

                // Verify the request is in a state that can be accepted (Unassigned)
                var acceptableStatuses = new[] 
                { 
                    (int)RequestStatus.Unassigned
                };

                if (!acceptableStatuses.Contains(request.RequestStatus))
                {
                    return false;
                }

                // Change the request status to Accepted (which maps to Pending in dashboard)
                request.RequestStatus = (int)RequestStatus.Accepted;
                
                // Note: Notes will be stored in notes table later
                // For now, we just update the request status

                return await _requestRepository.UpdateAsync(request);
            }
            catch
            {
                return false;
            }
        }

        private async Task<int?> GetCurrentPhysicianId()
        {
            var currentUser = _workContext.CurrentUser();
            if (currentUser == null)
            {
                return null;
            }

            var physician = await _physicianRepository.GetByUserIdAsync(currentUser.UserId);
            return physician?.PhysicianId;
        }

        private string GetStateColor(DashboardRequestStatus state)
        {
            return state switch
            {
                DashboardRequestStatus.New => "#1976d2",
                DashboardRequestStatus.Pending => "#42a5f5",
                DashboardRequestStatus.Active => "#66bb6a",
                DashboardRequestStatus.Conclude => "#ec407a",
                _ => "#666666"
            };
        }

        private string GetStateIcon(DashboardRequestStatus state)
        {
            return state switch
            {
                DashboardRequestStatus.New => "settings",
                DashboardRequestStatus.Pending => "notifications",
                DashboardRequestStatus.Active => "check_circle",
                DashboardRequestStatus.Conclude => "schedule",
                _ => "help"
            };
        }

        private string GetDashboardStatusDisplayName(DashboardRequestStatus status)
        {
            return status switch
            {
                DashboardRequestStatus.New => "NEW",
                DashboardRequestStatus.Pending => "PENDING",
                DashboardRequestStatus.Active => "ACTIVE",
                DashboardRequestStatus.Conclude => "CONCLUDE",
                _ => "UNKNOWN"
            };
        }

        private byte[] GenerateExcelExport(List<RequestDataDto> requests, string sheetName)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Add headers
                var headers = new[]
                {
                    "Patient Name", "Date of Birth", "Requestor", "Physician",
                    "Date of Service", "Requested Date", "Phone", "Address", "Status"
                };

                for (int i = 0; i < headers.Length; i++)
                {
                    worksheet.Cells[1, i + 1].Value = headers[i];
                    worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                }

                // Add data rows
                for (int i = 0; i < requests.Count; i++)
                {
                    var request = requests[i];
                    var row = i + 2;

                    worksheet.Cells[row, 1].Value = request.PatientFullName ?? "";
                    worksheet.Cells[row, 2].Value = request.DateOfBirth ?? "";
                    worksheet.Cells[row, 3].Value = request.RequestorName ?? "";
                    worksheet.Cells[row, 4].Value = request.PhysicianName ?? "-";
                    worksheet.Cells[row, 5].Value = request.DateOfService ?? "-";
                    worksheet.Cells[row, 6].Value = request.RequestedDate ?? "";
                    worksheet.Cells[row, 7].Value = request.Phone ?? "";
                    worksheet.Cells[row, 8].Value = request.Address ?? "";
                    worksheet.Cells[row, 9].Value = ((RequestStatus)Convert.ToInt32(request.RequestStatus)).ToString();
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
} 