using HalloDoc.Repositories.DTOs;
using HalloDoc.Repositories.Repositories.DocumentRepository;
using HalloDoc.Repositories.Repositories.AuthRepository;
using HalloDoc.Repositories.Repositories.PhysicianRepository;
using HalloDoc.Repositories.Repositories.RequestRepository;
using HalloDoc.Entities.Data.Entities;
using HalloDoc.Common.Helpers;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using System.IO.Compression;
using HalloDoc.Services.Helpers;

namespace HalloDoc.Services.Services
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IAuthRepository _authRepository;
        private readonly IPhysicianRepository _physicianRepository;
        private readonly IRequestRepository _requestRepository;
        private readonly IWorkContext _workContext;
        public DocumentService(
            IDocumentRepository documentRepository,
            IAuthRepository authRepository,
            IPhysicianRepository physicianRepository,
            IRequestRepository requestRepository,
            IWorkContext workContext)
        {
            _documentRepository = documentRepository;
            _authRepository = authRepository;
            _physicianRepository = physicianRepository;
            _requestRepository = requestRepository;
            _workContext = workContext;
        }

        public async Task<List<DocumentDto>> GetRequestDocumentsForAdminAsync(int requestId)
        {
            return await _documentRepository.GetRequestDocumentsForAdminAsync(requestId);
        }

        public async Task<List<DocumentDto>> GetRequestDocumentsForPhysicianAsync(int requestId)
        {
            var currentUser = _workContext.CurrentUser();
            var userId = currentUser?.UserId;
            var physician = await _physicianRepository.GetByUserIdAsync(userId ?? 0);
            return await _documentRepository.GetRequestDocumentsForPhysicianAsync(requestId, physician?.PhysicianId ?? 0);
        }

        public async Task<(byte[] FileContents, string ContentType, string FileName)> GetDocumentFileAsync(int documentId, bool isAdmin)
        {
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                throw new InvalidOperationException("Document not found.");
            }
            var currentUser = _workContext.CurrentUser();
            var userId = currentUser?.UserId;
            // For admin, allow access to any document
            if (!isAdmin)
            {
                // For physician, check if they have access to this document
                var physician = await _physicianRepository.GetByUserIdAsync(userId ?? 0);
                if (physician == null || !await _documentRepository.IsDocumentAccessibleByPhysicianAsync(documentId, physician.PhysicianId))
                {
                    throw new InvalidOperationException("Access denied.");
                }
            }

            // Read the file
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.FilePath);
            if (!File.Exists(filePath))
            {
                throw new InvalidOperationException("File not found on server.");
            }

            var fileContents = await File.ReadAllBytesAsync(filePath);
            var contentType = GetContentType(document.FileName);

            return (fileContents, contentType, document.FileName);
        }

        public async Task<bool> UploadDocumentAsync(int requestId, IFormFile file)
        {
            try
            {
                var currentUser = _workContext.CurrentUser();
                // Save file to disk
                var fileName = Path.GetFileName(file.FileName);
                var files = new List<IFormFile> { file };
                var filePath = await FileHelper.SaveRequestFilesAsync(requestId, files);

                // Create document entity
                var document = new Document
                {
                    RequestId = requestId,
                    FileName = fileName,
                    FilePath = filePath[0],
                    UploadedAt = DateTime.Now,
                    UploadedBy = currentUser?.UserId
                };
                var documents = new List<Document> { document };
                await _documentRepository.AddDocumentsAsync(documents);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteDocumentAsync(int documentId, bool isAdmin)
        {
            var currentUser = _workContext.CurrentUser();
            var userId = currentUser?.UserId;
            var document = await _documentRepository.GetDocumentByIdAsync(documentId);
            if (document == null)
            {
                return false;
            }

            // For admin, allow deletion of any document
            if (!isAdmin)
            {
                // For physician, check if they have access to this document
                var physician = await _physicianRepository.GetByUserIdAsync(userId ?? 0);
                if (physician == null || !await _documentRepository.IsDocumentAccessibleByPhysicianAsync(documentId, physician.PhysicianId))
                {
                    return false;
                }
            }

            // Delete file from disk
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return await _documentRepository.DeleteDocumentAsync(documentId);
        }

        public async Task<bool> EmailDocumentsAsync(EmailDocumentsDto emailDto, bool isAdmin)
        {
            // For now, just return true as email functionality will be implemented later
            // This is a placeholder for the email functionality
            return true;
        }

        public async Task<byte[]> DownloadMultipleDocumentsAsync(List<int> documentIds, bool isAdmin)
        {
            var currentUser = _workContext.CurrentUser();
            var userId = currentUser?.UserId;

            // Get documents
            var documents = await _documentRepository.GetDocumentsByIdsAsync(documentIds);
            if (!documents.Any())
            {
                throw new InvalidOperationException("No documents found.");
            }

            // For admin, allow access to any documents
            if (!isAdmin)
            {
                // For physician, check if they have access to all documents
                var physician = await _physicianRepository.GetByUserIdAsync(userId ?? 0);
                if (physician == null)
                {
                    throw new InvalidOperationException("Access denied.");
                }

                foreach (var document in documents)
                {
                    if (!await _documentRepository.IsDocumentAccessibleByPhysicianAsync(document.DocumentId, physician.PhysicianId))
                    {
                        throw new InvalidOperationException($"Access denied to document: {document.FileName}");
                    }
                }
            }

            // Create ZIP file in memory
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (var document in documents)
                    {
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), document.FilePath);
                        if (!File.Exists(filePath))
                        {
                            continue; // Skip files that don't exist
                        }

                        var entry = archive.CreateEntry(document.FileName);
                        using (var entryStream = entry.Open())
                        using (var fileStream = File.OpenRead(filePath))
                        {
                            await fileStream.CopyToAsync(entryStream);
                        }
                    }
                }

                return memoryStream.ToArray();
            }
        }
        private string GetContentType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
} 