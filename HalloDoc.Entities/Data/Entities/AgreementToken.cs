using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HalloDoc.Entities.Data.Entities
{
    public class AgreementToken
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int RequestId { get; set; }
        [ForeignKey("RequestId")]
        public virtual Request Request { get; set; } = null!;

        [Required]
        [MaxLength(128)]
        public string Token { get; set; } = null!;

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime Expiry { get; set; }

        [Required]
        public bool Used { get; set; } = false;

        [Column(TypeName = "timestamp without time zone")]
        public DateTime? UsedAt { get; set; }

        [Required]
        [Column(TypeName = "timestamp without time zone")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}