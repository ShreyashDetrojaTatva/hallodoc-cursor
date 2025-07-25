using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class Roles
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoleName { get; set; } = null!;

        [Required]
        public int AccountType { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedAt { get; set; }

        public virtual ICollection<Admin>? Admins { get; set; }
        public virtual ICollection<Physician>? Physicians { get; set; }
        public virtual ICollection<RoleMenus>? RoleMenus { get; set; }
    }
} 