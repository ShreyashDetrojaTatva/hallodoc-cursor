using HalloDoc.Common.Constants;
using HalloDoc.Common.Utility;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Repositories.Repositories.RequestRepository;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;

namespace HalloDoc.Services.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly IRequestRepository _requestRepository;

        public AdminDashboardService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStateAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request)
        {
            // Get requests based on state mapping
            var statusIds = RequestStatusMapper.GetStatusIdsForState(state);
            var response = await _requestRepository.GetRequestsByStatusIdsAsync(statusIds, request);

            return response;
        }

        public async Task<List<DashboardStateDto>> GetStateCountsAsync()
        {
            var stateCounts = new List<DashboardStateDto>
            {
                new DashboardStateDto { Id = (int)DashboardRequestStatus.New, Name = "NEW", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.New)), Color = "#1976d2", Icon = "settings" },
                new DashboardStateDto { Id = (int)DashboardRequestStatus.Pending, Name = "PENDING", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.Pending)), Color = "#42a5f5", Icon = "notifications" },
                new DashboardStateDto { Id = (int)DashboardRequestStatus.Active, Name = "ACTIVE", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.Active)), Color = "#66bb6a", Icon = "check_circle" },
                new DashboardStateDto { Id = (int)DashboardRequestStatus.Conclude, Name = "CONCLUDE", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.Conclude)), Color = "#ec407a", Icon = "schedule" },
                new DashboardStateDto { Id = (int)DashboardRequestStatus.ToClose, Name = "TO-CLOSE", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.ToClose)), Color = "#42a5f5", Icon = "folder" },
                new DashboardStateDto { Id = (int)DashboardRequestStatus.Unpaid, Name = "UNPAID", Count = await GetRequestCountByStatusIds(RequestStatusMapper.GetStatusIdsForState(DashboardRequestStatus.Unpaid)), Color = "#ab47bc", Icon = "attach_money" }
            };

            return stateCounts;
        }

        public async Task<byte[]> ExportRequestsAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request)
        {
            var statusIds = RequestStatusMapper.GetStatusIdsForState(state);
            var response = await _requestRepository.GetRequestsByStatusIdsAsync(statusIds, request);
            
            // Convert to Excel format
            return GenerateExcelData(response.Items, $"Admin Dashboard - {state}");
        }

        public async Task<byte[]> ExportAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request)
        {
            var response = await _requestRepository.GetAllRequestsAsync(request);
            
            // Convert to Excel format
            return GenerateExcelData(response.Items, "Admin Dashboard - All Requests");
        }

        private async Task<int> GetRequestCountByStatusIds(int[] statusIds)
        {
            // Get real count from the repository
            return await _requestRepository.GetRequestCountByStatusIdsAsync(statusIds);
        }

        private byte[] GenerateExcelData(List<RequestDataDto> requests, string sheetName)
        {
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage())
            {
                var worksheet = package.Workbook.Worksheets.Add(sheetName);

                // Add headers
                var headers = new[]
                {
                    "Patient Full Name", "Date of Birth", "Requestor Name", "Physician Name",
                    "Date of Service", "Phone", "Address", "Request Status", "Request Type", "Requested Date"
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
                    worksheet.Cells[row, 4].Value = request.PhysicianName ?? "";
                    worksheet.Cells[row, 5].Value = request.DateOfService ?? "";
                    worksheet.Cells[row, 6].Value = request.Phone ?? "";
                    worksheet.Cells[row, 7].Value = request.Address ?? "";
                    worksheet.Cells[row, 8].Value = ((RequestStatus)Convert.ToInt32(request.RequestStatus)).ToString();
                    worksheet.Cells[row, 9].Value = ((RequestType)request.RequestType).ToString();
                    worksheet.Cells[row, 10].Value = request.RequestedDate ?? "";
                }

                // Auto-fit columns
                worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                return package.GetAsByteArray();
            }
        }
    }
} 