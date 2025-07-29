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
    }
} 