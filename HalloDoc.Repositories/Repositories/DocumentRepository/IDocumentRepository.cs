using HalloDoc.Entities.Data.Entities;
using HalloDoc.Repositories.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.DocumentRepository
{
    public interface IDocumentRepository
    {
        Task AddDocumentsAsync(List<Document> documents);
        Task<List<DocumentDto>> GetRequestDocumentsAsync(int requestId);
        Task<Document?> GetDocumentByIdAsync(int documentId);
    }
} 