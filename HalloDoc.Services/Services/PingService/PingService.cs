using HalloDoc.Repositories.Repositories.PingRepository;
using HalloDoc.Entities.Data.Entities;

namespace HalloDoc.Services.Services.PingService
{
    public class PingService : IPingService
    {
        private readonly IPingRepository _pingRepository;
        public PingService(IPingRepository pingRepository)
        {
            _pingRepository = pingRepository;
        }

        public async Task<string> GetPingAsync()
        {
            return await _pingRepository.GetPingAsync();
        }

        public async Task<string> GetFirstPingMessageAsync()
        {
            var pings = await _pingRepository.GetAllPingsAsync();
            return pings.FirstOrDefault()?.Message ?? "No records found";
        }

        public async Task<List<Ping>> GetAllPings()
        {
            return await _pingRepository.GetAllPingsAsync();
        }
    }
} 