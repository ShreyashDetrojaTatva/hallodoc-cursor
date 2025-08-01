using HalloDoc.Common.Constants;
using HalloDoc.Common.Utility;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Repositories.Repositories.RequestRepository;
using Microsoft.EntityFrameworkCore;

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
            
            // Convert to CSV format
            return GenerateCsvData(response.Items);
        }

        public async Task<byte[]> ExportAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request)
        {
            var response = await _requestRepository.GetAllRequestsAsync(request);
            
            // Convert to CSV format
            return GenerateCsvData(response.Items);
        }

        private async Task<int> GetRequestCountByStatusIds(int[] statusIds)
        {
            // Get real count from the repository
            return await _requestRepository.GetRequestCountByStatusIdsAsync(statusIds);
        }

        private byte[] GenerateCsvData(List<RequestDataDto> requests)
        {
            var csvLines = new List<string>
            {
                "Patient Full Name,Date of Birth,Requestor Name,Physician Name,Date of Service,Phone,Address,Request Status,Request Type,Requested Date"
            };

            foreach (var request in requests)
            {
                var line = $"{request.PatientFullName}," +
                          $"{request.DateOfBirth}," +
                          $"{request.RequestorName}," +
                          $"{request.PhysicianName ?? ""}," +
                          $"{request.DateOfService ?? ""}," +
                          $"{request.Phone}," +
                          $"{request.Address}," +
                          $"{request.RequestStatus ?? ""}," +
                          $"{request.RequestType}," +
                          $"{request.RequestedDate}";
                
                csvLines.Add(line);
            }

            return System.Text.Encoding.UTF8.GetBytes(string.Join("\n", csvLines));
        }
    }
} 