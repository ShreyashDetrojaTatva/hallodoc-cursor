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
                new DashboardStateDto { Id = 1, Name = "NEW", Count = await GetRequestCountByStatusIds(new[] { 1 }), Color = "#1976d2", Icon = "settings" },
                new DashboardStateDto { Id = 2, Name = "PENDING", Count = await GetRequestCountByStatusIds(new[] { 2 }), Color = "#42a5f5", Icon = "notifications" },
                new DashboardStateDto { Id = 3, Name = "ACTIVE", Count = await GetRequestCountByStatusIds(new[] { 4, 5 }), Color = "#66bb6a", Icon = "check_circle" },
                new DashboardStateDto { Id = 4, Name = "CONCLUDE", Count = await GetRequestCountByStatusIds(new[] { 6 }), Color = "#ec407a", Icon = "schedule" },
                new DashboardStateDto { Id = 5, Name = "TO-CLOSE", Count = await GetRequestCountByStatusIds(new[] { 8, 3, 7 }), Color = "#42a5f5", Icon = "folder" },
                new DashboardStateDto { Id = 6, Name = "UNPAID", Count = await GetRequestCountByStatusIds(new[] { 9 }), Color = "#ab47bc", Icon = "attach_money" }
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
            // This would be implemented in the repository
            // For now, return a mock count
            return await Task.FromResult(new Random().Next(100, 2000));
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