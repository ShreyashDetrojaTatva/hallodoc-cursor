using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace HalloDoc.Repositories.DTOs
{
    public class RequestCreateDto
    {
        // Request fields
        public int RequestType { get; set; }
        public int RequestorType { get; set; }
        public string? RequestorFirstName { get; set; }
        public string? RequestorLastName { get; set; }
        public string? RequestorEmail { get; set; }
        public string? RequestorPhone { get; set; }
        public string? RelationWithPatient { get; set; }
        public string? HotelName { get; set; }
        public string? PropertyName { get; set; }
        public string? CaseNumber { get; set; }
        public int? PatientId { get; set; }
        // RequestClient fields
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime? DOB { get; set; }
        public string Email { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public string ZipCode { get; set; } = null!;
        public string? RoomNo { get; set; }
        public string Symptoms { get; set; } = null!;
        public int? UserId { get; set; }
        // File uploads
        public List<IFormFile>? Files { get; set; }
    }
} 