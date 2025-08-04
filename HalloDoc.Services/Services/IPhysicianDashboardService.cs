using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using HalloDoc.Common.Constants;

namespace HalloDoc.Services.Services
{
    public interface IPhysicianDashboardService
    {
        Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStateAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request);
        Task<List<DashboardStateDto>> GetStateCountsAsync();
        Task<byte[]> ExportRequestsAsync(DashboardRequestStatus state, PaginationRequestDto<DashboardFiltersDto> request);
        Task<byte[]> ExportAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request);
        Task<bool> AcceptRequestAsync(AcceptRequestDto acceptRequest);
    }
} 