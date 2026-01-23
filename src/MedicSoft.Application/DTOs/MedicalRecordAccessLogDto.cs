using System;

namespace MedicSoft.Application.DTOs
{
    public class MedicalRecordAccessLogDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public string AccessType { get; set; } = string.Empty;
        public DateTime AccessedAt { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Details { get; set; }
    }
}
