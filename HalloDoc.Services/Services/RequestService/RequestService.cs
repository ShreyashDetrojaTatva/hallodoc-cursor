using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.RequestRepository;
using System.Threading.Tasks;
using HalloDoc.Repositories.Repositories.DocumentRepository;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;
using System;
using HalloDoc.Services.Helpers;

namespace HalloDoc.Services.Services.RequestService
{
    public class RequestService : IRequestService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IDocumentRepository _documentRepository;
        public RequestService(IRequestRepository requestRepository, IDocumentRepository documentRepository)
        {
            _requestRepository = requestRepository;
            _documentRepository = documentRepository;
        }

        public async Task<Request> CreateRequestAsync(RequestCreateDto dto)
        {
            var request = await _requestRepository.CreateRequestAsync(dto);

            if (dto.Files != null && dto.Files.Count > 0)
            {
                var savedPaths = await FileHelper.SaveRequestFilesAsync(request.RequestId, dto.Files);
                var documents = new List<Document>();
                for (int i = 0; i < savedPaths.Count; i++)
                {
                    var fileName = Path.GetFileName(savedPaths[i]);
                    documents.Add(new Document
                    {
                        RequestId = request.RequestId,
                        FileName = fileName,
                        FilePath = savedPaths[i],
                        UploadedAt = DateTime.Now,
                        UploadedBy = null // Set as needed
                    });
                }
                await _documentRepository.AddDocumentsAsync(documents);
            }
            return request;
        }
    }
} 