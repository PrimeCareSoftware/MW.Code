using System;

namespace MedicSoft.Application.DTOs
{
    public class MedicalRecordVersionDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public int Version { get; set; }
        public string ChangeType { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
        public Guid ChangedByUserId { get; set; }
        public string? ChangedByUserName { get; set; }
        public string? ChangeReason { get; set; }
        public string? ChangesSummary { get; set; }
        public string ContentHash { get; set; } = string.Empty;
    }
}
