namespace HalloDoc.Services.Services.PingService
{
    using HalloDoc.Entities.Data.Entities;
    public interface IPingService
    {
        Task<string> GetPingAsync();
        Task<string> GetFirstPingMessageAsync();
        Task<List<Ping>> GetAllPings();
    }
} 