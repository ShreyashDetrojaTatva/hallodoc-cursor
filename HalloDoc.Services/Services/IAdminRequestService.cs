using HalloDoc.Repositories.DTOs;

namespace HalloDoc.Services.Services
{
    public interface IAdminRequestService
    {
        Task<RequestDetailsDto?> GetRequestDetailsAsync(int requestId);
        Task<bool> UpdateRequestAsync(UpdateRequestDto updateRequest);
        Task<List<PhysicianDto>> GetPhysiciansAsync();
        Task<bool> AssignRequestAsync(AssignRequestDto assignRequest);
        Task<bool> CancelRequestAsync(CancelRequestDto cancelRequest);
        Task<bool> SendAgreementAsync(SendAgreementDto sendAgreementDto);
    }
} 