using HalloDoc.Entities.Data.Context;
using HalloDoc.Entities.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.DocumentRepository
{
    public class DocumentRepository : IDocumentRepository
    {
        private readonly ApplicationDbContext _context;
        public DocumentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddDocumentsAsync(List<Document> documents)
        {
            await _context.Documents.AddRangeAsync(documents);
            await _context.SaveChangesAsync();
        }
    }
} 