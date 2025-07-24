using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public int RequestType { get; set; } // enum as int
        public int RequestStatus { get; set; } // enum as int
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }
        public int? CreatedBy { get; set; } // FK to Users (nullable)

        public int? PatientId { get; set; } // FK to Patient
        [ForeignKey("PatientId")]
        public virtual Patient? Patient { get; set; }

        // Generic requestor info
        [MaxLength(100)]
        public string? RequestorFirstName { get; set; }
        [MaxLength(100)]
        public string? RequestorLastName { get; set; }
        [MaxLength(100)]
        public string? RequestorEmail { get; set; }
        [MaxLength(20)]
        public string? RequestorPhone { get; set; }
        public int RequestorType { get; set; } // enum as int

        // Type-specific fields
        [MaxLength(100)]
        public string? RelationWithPatient { get; set; } // Family
        [MaxLength(100)]
        public string? HotelName { get; set; } // Concierge
        [MaxLength(100)]
        public string? PropertyName { get; set; } // Business/Concierge
        [MaxLength(100)]
        public string? CaseNumber { get; set; } // Business

        // Navigation
        public virtual RequestClient? RequestClient { get; set; }
        public virtual ICollection<Document>? Documents { get; set; }
    }
} 