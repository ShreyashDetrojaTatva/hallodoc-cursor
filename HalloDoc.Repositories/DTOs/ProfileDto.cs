using System;

namespace HalloDoc.Repositories.DTOs
{
    public class ProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? RegionId { get; set; }
        public string? ZipCode { get; set; }
    }

    public class UpdateProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public int? RegionId { get; set; }
        public string? ZipCode { get; set; }
    }
} 