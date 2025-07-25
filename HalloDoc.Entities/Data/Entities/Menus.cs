using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class Menus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MenuId { get; set; }

        [Required]
        [MaxLength(100)]
        public string MenuName { get; set; } = null!;

        [Required]
        [MaxLength(255)]
        public string Path { get; set; } = null!;

        [Required]
        public int AccountType { get; set; }

        public virtual ICollection<RoleMenus>? RoleMenus { get; set; }

        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UpdatedAt { get; set; }
    }
} 