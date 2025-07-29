using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HalloDoc.Common.Constants;
using HalloDoc.Entities.Data.Entities;

namespace HalloDoc.Entities.Data.Entities
{
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        public int AccountType { get; set; }

        public bool IsActive { get; set; } = true;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? LastLoginAt { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<Admin>? Admins { get; set; }
        public virtual ICollection<Physician>? Physicians { get; set; }
        public virtual ICollection<Patient>? Patients { get; set; }
    }
} 