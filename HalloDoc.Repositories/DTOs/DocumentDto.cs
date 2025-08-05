using System;
using System.Collections.Generic;

namespace HalloDoc.Repositories.DTOs
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }

    public class UploadDocumentDto
    {
        public int RequestId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public int? UploadedBy { get; set; }
    }

    public class EmailDocumentsDto
    {
        public int RequestId { get; set; }
        public string Email { get; set; } = string.Empty;
        public List<int> DocumentIds { get; set; } = new List<int>();
    }

    public class DeleteDocumentDto
    {
        public int DocumentId { get; set; }
    }
} 