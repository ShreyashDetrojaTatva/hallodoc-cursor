using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Repositories.DTOs
{
    public class AcceptRequestDto
    {
        [Required]
        public int RequestId { get; set; }
        
        public string? Notes { get; set; }
    }
} 