using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Mappers;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.RequestRepository
{
    public class RequestRepository : BaseRepository<Request>, IRequestRepository
    {
        private readonly ApplicationDbContext _context;
        public RequestRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Request> CreateRequestAsync(RequestCreateDto dto)
        {
            var request = RequestMapper.ToRequestEntity(dto);
            await AddAsync(request);
            await SaveChangesAsync();

            var requestClient = RequestMapper.ToRequestClientEntity(dto);
            requestClient.RequestId = request.RequestId;
            await _context.RequestClients.AddAsync(requestClient);
            await _context.SaveChangesAsync();

            return request;
        }
    }
} 