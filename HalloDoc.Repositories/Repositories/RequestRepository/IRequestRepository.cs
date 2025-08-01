using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.DTOs.Pagination;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.RequestRepository
{
    public interface IRequestRepository
    {
        Task<Request> CreateRequestAsync(RequestCreateDto dto);
        Task<List<RequestListDto>> GetPatientRequestsAsync(int patientId, RequestFilterDto? filter = null);
        Task<Request?> GetByIdAsync(int requestId);
        
        // Admin Dashboard Methods
        Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request);
        Task<PaginationResponseDto<RequestDataDto>> GetAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request);
    }
} 