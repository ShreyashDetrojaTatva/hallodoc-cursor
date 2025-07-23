using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HalloDoc.Repositories.Repositories.PingRepository
{
    public class PingRepository : IPingRepository
    {
        private readonly ApplicationDbContext _context;
        public PingRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<string> GetPingAsync()
        {
            // For demonstration, just return "pong". In real use, query the DB.
            return await Task.FromResult("pong");
        }

        public async Task<List<Ping>> GetAllPingsAsync()
        {
            return await _context.Pings.ToListAsync();
        }
    }
} 