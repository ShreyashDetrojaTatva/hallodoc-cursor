using System;

namespace HalloDoc.Repositories.DTOs
{
    public class DocumentDto
    {
        public int DocumentId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
} 