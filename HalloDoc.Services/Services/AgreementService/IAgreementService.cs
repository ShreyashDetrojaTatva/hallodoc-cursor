using HalloDoc.Repositories.DTOs;
using System.Threading.Tasks;

namespace HalloDoc.Services.Services.AgreementService
{
    public interface IAgreementService
    {
        Task<bool> SendAgreementAsync(SendAgreementDto sendAgreementDto);
        Task<AgreementDetailsDto?> GetAgreementDetailsAsync(string token);
        Task<bool> ProcessAgreementResponseAsync(AgreementResponseDto agreementResponseDto);
        Task<bool> IsTokenValidAsync(string token);
    }
}