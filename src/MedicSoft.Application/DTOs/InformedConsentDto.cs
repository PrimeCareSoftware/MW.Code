using System;

namespace MedicSoft.Application.DTOs
{
    public class InformedConsentDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid PatientId { get; set; }
        
        public string ConsentText { get; set; } = string.Empty;
        public bool IsAccepted { get; set; }
        public DateTime? AcceptedAt { get; set; }
        public string? IPAddress { get; set; }
        public string? DigitalSignature { get; set; }
        public Guid? RegisteredByUserId { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateInformedConsentDto
    {
        public Guid MedicalRecordId { get; set; }
        public Guid PatientId { get; set; }
        
        public string ConsentText { get; set; } = string.Empty;
        public Guid? RegisteredByUserId { get; set; }
    }

    public class AcceptInformedConsentDto
    {
        public string? IPAddress { get; set; }
        public string? DigitalSignature { get; set; }
    }
}
