using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Services.Services.RequestService
{
    public interface IRequestService
    {
        Task<Request> CreateRequestAsync(RequestCreateDto dto);
        Task<List<RequestListDto>> GetPatientRequestsAsync(int patientId, RequestFilterDto? filter = null);
        Task<List<DocumentDto>> GetRequestDocumentsAsync(int requestId, int userId);
        Task<(byte[] FileContents, string ContentType, string FileName)> GetDocumentFileAsync(int documentId, int userId);
    }
} 