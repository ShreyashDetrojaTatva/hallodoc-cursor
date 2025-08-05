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
        
        // New methods for admin/physician document management
        Task<bool> DeleteDocumentAsync(int documentId);
        Task<List<DocumentDto>> GetRequestDocumentsForAdminAsync(int requestId);
        Task<List<DocumentDto>> GetRequestDocumentsForPhysicianAsync(int requestId, int physicianId);
        Task<bool> IsDocumentAccessibleByPhysicianAsync(int documentId, int physicianId);
        Task<List<DocumentDto>> GetDocumentsByIdsAsync(List<int> documentIds);
    }
} 