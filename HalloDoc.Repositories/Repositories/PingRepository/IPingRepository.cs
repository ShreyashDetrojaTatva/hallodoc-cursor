namespace HalloDoc.Repositories.Repositories.PingRepository
{
    using HalloDoc.Entities.Data.Entities;
    public interface IPingRepository
    {
        Task<string> GetPingAsync();
        Task<List<Ping>> GetAllPingsAsync();
    }
} 