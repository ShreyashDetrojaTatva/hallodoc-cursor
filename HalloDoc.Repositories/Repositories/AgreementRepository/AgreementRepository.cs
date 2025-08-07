using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using HalloDoc.Common.Constants;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.AgreementRepository
{
    public class AgreementRepository : IAgreementRepository
    {
        private readonly ApplicationDbContext _context;

        public AgreementRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AgreementToken?> GetByTokenAsync(string token)
        {
            return await _context.AgreementTokens
                .Include(a => a.Request)
                .ThenInclude(r => r!.Patient)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.Token == token);
        }

        public async Task<AgreementToken?> GetByRequestIdAsync(int requestId)
        {
            return await _context.AgreementTokens
                .Include(a => a.Request)
                .ThenInclude(r => r!.RequestClients)
                .Include(a => a.Request)
                .ThenInclude(r => r!.Patient)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(a => a.RequestId == requestId);
        }

        public async Task<AgreementToken> CreateAsync(AgreementToken agreementToken)
        {
            _context.AgreementTokens.Add(agreementToken);
            await _context.SaveChangesAsync();
            return agreementToken;
        }

        public async Task<bool> UpdateAsync(AgreementToken agreementToken)
        {
            _context.AgreementTokens.Update(agreementToken);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var agreementToken = await _context.AgreementTokens.FindAsync(id);
            if (agreementToken == null)
                return false;

            _context.AgreementTokens.Remove(agreementToken);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> IsTokenValidAsync(string token)
        {
            var agreementToken = await _context.AgreementTokens
                .FirstOrDefaultAsync(a => a.Token == token);

            if (agreementToken == null)
                return false;

            // Check if token is expired
            if (agreementToken.Expiry < DateTime.Now)
                return false;

            // Check if token is already used
            if (agreementToken.Used)
                return false;

            return true;
        }

        public async Task<bool> IsRequestEligibleForAgreementAsync(int requestId)
        {
            var request = await _context.Requests
                .Include(r => r.Patient)
                .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(r => r.RequestId == requestId);

            if (request == null)
                return false;

            // Check if request is in Accepted state
            if (request.RequestStatus != (int)RequestStatus.Accepted)
                return false;

            // Check if patient exists and has email
            if (request.Patient?.User?.Email == null)
                return false;

            // Check if there's already an active agreement token for this request
            var existingToken = await _context.AgreementTokens
                .FirstOrDefaultAsync(a => a.RequestId == requestId && !a.Used && a.Expiry > DateTime.Now);

            return existingToken == null;
        }
    }
}