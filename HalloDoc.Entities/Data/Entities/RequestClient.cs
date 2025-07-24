using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class RequestClient
    {
        [Key]
        public int RequestClientId { get; set; }

        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; } = null!;

        [MaxLength(100)]
        public string FirstName { get; set; } = null!;
        [MaxLength(100)]
        public string LastName { get; set; } = null!;
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? DOB { get; set; }
        [MaxLength(100)]
        public string Email { get; set; } = null!;
        [MaxLength(20)]
        public string Phone { get; set; } = null!;
        [MaxLength(200)]
        public string Street { get; set; } = null!;
        [MaxLength(100)]
        public string City { get; set; } = null!;
        [MaxLength(100)]
        public string State { get; set; } = null!;
        [MaxLength(20)]
        public string ZipCode { get; set; } = null!;
        [MaxLength(100)]
        public string? RoomNo { get; set; }
        [MaxLength(500)]
        public string Symptoms { get; set; } = null!;
        public int? UserId { get; set; } // Link to Users if patient is registered
    }
} 