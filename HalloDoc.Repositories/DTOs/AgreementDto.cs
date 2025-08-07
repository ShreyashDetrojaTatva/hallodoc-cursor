using System;

namespace HalloDoc.Repositories.DTOs
{
    public class SendAgreementDto
    {
        public int RequestId { get; set; }
        public string PatientEmail { get; set; } = null!;
    }

    public class AgreementResponseDto
    {
        public string Token { get; set; } = null!;
        public bool IsAccepted { get; set; }
        public string? CancellationReason { get; set; }
    }

    public class AgreementDetailsDto
    {
        public int RequestId { get; set; }
        public string PatientName { get; set; } = null!;
        public string PatientEmail { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string RequestorName { get; set; } = null!;
        public string RequestorEmail { get; set; } = null!;
        public string RequestorPhone { get; set; } = null!;
        public string? Symptoms { get; set; }
        public bool IsExpired { get; set; }
        public bool IsUsed { get; set; }
    }

    public class AgreementTokenDto
    {
        public int Id { get; set; }
        public int RequestId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expiry { get; set; }
        public bool Used { get; set; }
        public DateTime? UsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 