using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; } = null!;

        [MaxLength(255)]
        public string FileName { get; set; } = null!;
        [MaxLength(500)]
        public string FilePath { get; set; } = null!;
        [Column(TypeName = "timestamp without time zone")]
        public DateTime UploadedAt { get; set; }
        public int? UploadedBy { get; set; } // FK to Users (nullable)
    }
} 