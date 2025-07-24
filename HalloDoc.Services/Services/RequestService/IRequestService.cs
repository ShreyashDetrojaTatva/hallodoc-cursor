using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using System.Threading.Tasks;

namespace HalloDoc.Services.Services.RequestService
{
    public interface IRequestService
    {
        Task<Request> CreateRequestAsync(RequestCreateDto dto);
    }
} 