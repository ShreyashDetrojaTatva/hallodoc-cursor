using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class RoleMenus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleMenuId { get; set; }

        [Required]
        public int RoleId { get; set; }
        [ForeignKey("RoleId")]
        public virtual Roles Role { get; set; } = null!;

        [Required]
        public int MenuId { get; set; }
        [ForeignKey("MenuId")]
        public virtual Menus Menu { get; set; } = null!;
    }
} 