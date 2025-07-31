using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
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
        Task<List<RequestDataDto>> GetRequestsByStatusIdsAsync(int[] statusIds, DashboardFiltersDto filters);
        Task<List<RequestDataDto>> GetAllRequestsAsync(DashboardFiltersDto filters);
    }
} 