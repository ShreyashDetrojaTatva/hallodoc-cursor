using HalloDoc.Entities.Data.Entities;
using System.Threading.Tasks;
using HalloDoc.Repositories.DTOs;

namespace HalloDoc.Repositories.Repositories.RequestRepository
{
    public interface IRequestRepository
    {
        Task<Request> CreateRequestAsync(RequestCreateDto dto);
    }
} 