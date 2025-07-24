using HalloDoc.Entities.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HalloDoc.Repositories.Repositories.DocumentRepository
{
    public interface IDocumentRepository
    {
        Task AddDocumentsAsync(List<Document> documents);
    }
} 