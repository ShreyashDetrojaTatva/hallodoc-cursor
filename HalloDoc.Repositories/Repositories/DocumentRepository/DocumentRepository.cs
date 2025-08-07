using HalloDoc.Entities.Data.Entities;
using HalloDoc.Entities.Data.Context;
using HalloDoc.Repositories.DTOs;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace HalloDoc.Repositories.Repositories.DocumentRepository
{
    public class DocumentRepository : BaseRepository<Document>, IDocumentRepository
    {
        public DocumentRepository(ApplicationDbContext db) : base(db) { }

        public async Task AddDocumentsAsync(List<Document> documents)
        {
            await _db.Documents.AddRangeAsync(documents);
            await _db.SaveChangesAsync();
        }

        public async Task<List<DocumentDto>> GetRequestDocumentsAsync(int requestId)
        {
            return await _db.Documents
                .Where(d => d.RequestId == requestId)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentDto
                {
                    DocumentId = d.DocumentId,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();
        }

        public async Task<Document?> GetDocumentByIdAsync(int documentId)
        {
            return await _db.Documents.FindAsync(documentId);
        }

        // New methods for admin/physician document management
        public async Task<bool> DeleteDocumentAsync(int documentId)
        {
            var document = await _db.Documents.FindAsync(documentId);
            if (document == null)
                return false;

            _db.Documents.Remove(document);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<List<DocumentDto>> GetRequestDocumentsForPhysicianAsync(int requestId, int physicianId)
        {
            return await _db.Documents
                .Where(d => d.RequestId == requestId && d.Request.PhysicianId == physicianId)
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentDto
                {
                    DocumentId = d.DocumentId,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();
        }

        public async Task<bool> IsDocumentAccessibleByPhysicianAsync(int documentId, int physicianId)
        {
            return await _db.Documents
                .AnyAsync(d => d.DocumentId == documentId && d.Request.PhysicianId == physicianId);
        }

        public async Task<List<DocumentDto>> GetDocumentsByIdsAsync(List<int> documentIds)
        {
            return await _db.Documents
                .Where(d => documentIds.Contains(d.DocumentId))
                .OrderByDescending(d => d.UploadedAt)
                .Select(d => new DocumentDto
                {
                    DocumentId = d.DocumentId,
                    FileName = d.FileName,
                    FilePath = d.FilePath,
                    UploadedAt = d.UploadedAt
                })
                .ToListAsync();
        }
    }
}