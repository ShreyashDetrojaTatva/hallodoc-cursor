using System.ComponentModel.DataAnnotations;

namespace HalloDoc.Repositories.DTOs
{
    public class RequestDetailsDto
    {
        public int RequestId { get; set; }
        
        // Patient Information
        public string PatientFirstName { get; set; } = string.Empty;
        public string PatientLastName { get; set; } = string.Empty;
        public DateTime? PatientDOB { get; set; }
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string PatientStreet { get; set; } = string.Empty;
        public string PatientCity { get; set; } = string.Empty;
        public string PatientState { get; set; } = string.Empty;
        public string PatientZipCode { get; set; } = string.Empty;
        
        // Request Information
        public int RequestType { get; set; }
        public int RequestStatus { get; set; }
        public string Symptoms { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        
        // Requestor Information
        public string RequestorFirstName { get; set; } = string.Empty;
        public string RequestorLastName { get; set; } = string.Empty;
        public string RequestorPhone { get; set; } = string.Empty;
        public string RequestorEmail { get; set; } = string.Empty;
        public string RequestorRelation { get; set; } = string.Empty;
    }

    public class UpdateRequestDto
    {
        [Required]
        public int RequestId { get; set; }
        
        // Patient Information
        [Required]
        public string PatientFirstName { get; set; } = string.Empty;
        [Required]
        public string PatientLastName { get; set; } = string.Empty;
        [Required]
        public DateTime? PatientDOB { get; set; }
        [Required]
        public string PatientPhone { get; set; } = string.Empty;
        [Required]
        [EmailAddress]
        public string PatientEmail { get; set; } = string.Empty;
        [Required]
        public string PatientStreet { get; set; } = string.Empty;
        [Required]
        public string PatientCity { get; set; } = string.Empty;
        [Required]
        public string PatientState { get; set; } = string.Empty;
        [Required]
        public string PatientZipCode { get; set; } = string.Empty;
        
        // Request Information
        [Required]
        public string Symptoms { get; set; } = string.Empty;
        
        // Requestor Information (optional for patient requests)
        public string RequestorFirstName { get; set; } = string.Empty;
        public string RequestorLastName { get; set; } = string.Empty;
        public string RequestorPhone { get; set; } = string.Empty;
        public string RequestorEmail { get; set; } = string.Empty;
        public string RequestorRelation { get; set; } = string.Empty;
    }
} 