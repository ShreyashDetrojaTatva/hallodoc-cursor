using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Repositories.DTOs
{
    public class CancelRequestDto
    {
        [Required]
        public int RequestId { get; set; }
        
        public string? CancellationReason { get; set; }
    }
} 