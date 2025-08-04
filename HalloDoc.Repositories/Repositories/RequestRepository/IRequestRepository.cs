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
        Task<bool> UpdateAsync(Request request);
        
        // Admin Dashboard Methods
        Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request);
        Task<PaginationResponseDto<RequestDataDto>> GetAllRequestsAsync(PaginationRequestDto<DashboardFiltersDto> request);
        Task<int> GetRequestCountByStatusIdsAsync(int[] statusIds);
        
        // Physician Dashboard Methods
        Task<PaginationResponseDto<RequestDataDto>> GetRequestsByStatusIdsForPhysicianAsync(int[] statusIds, PaginationRequestDto<DashboardFiltersDto> request, int physicianId);
        Task<int> GetRequestCountByStatusIdsForPhysicianAsync(int[] statusIds, int physicianId);
    }
} 