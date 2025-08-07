using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.AgreementRepository
{
    public interface IAgreementRepository
    {
        Task<AgreementToken?> GetByTokenAsync(string token);
        Task<AgreementToken?> GetByRequestIdAsync(int requestId);
        Task<AgreementToken> CreateAsync(AgreementToken agreementToken);
        Task<bool> UpdateAsync(AgreementToken agreementToken);
        Task<bool> DeleteAsync(int id);
        Task<bool> IsTokenValidAsync(string token);
        Task<bool> IsRequestEligibleForAgreementAsync(int requestId);
    }
}