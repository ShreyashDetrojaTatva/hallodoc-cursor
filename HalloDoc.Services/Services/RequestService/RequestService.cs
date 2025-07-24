using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.RequestRepository;
using System.Threading.Tasks;

namespace HalloDoc.Services.Services.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        public RequestService(IRequestRepository requestRepository)
        {
            _requestRepository = requestRepository;
        }

        public async Task<Request> CreateRequestAsync(RequestCreateDto dto)
        {
            return await _requestRepository.CreateRequestAsync(dto);
        }
    }
} 