using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Repositories.DTOs
{
    public class PhysicianDto
    {
        public int PhysicianId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
    }

    public class AssignRequestDto
    {
        [Required]
        public int RequestId { get; set; }
        
        [Required]
        public int PhysicianId { get; set; }
    }
} 