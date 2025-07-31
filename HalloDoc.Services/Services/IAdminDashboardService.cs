using HalloDoc.Repositories.DTOs;
using HalloDoc.Common.Constants;

namespace HalloDoc.Services.Services
{
    public interface IAdminDashboardService
    {
        Task<DashboardResponseDto> GetRequestsByStateAsync(DashboardRequestStatus state, DashboardFiltersDto filters);
        Task<List<DashboardStateDto>> GetStateCountsAsync();
        Task<byte[]> ExportRequestsAsync(DashboardRequestStatus state, DashboardFiltersDto filters);
        Task<byte[]> ExportAllRequestsAsync(DashboardFiltersDto filters);
    }
} 