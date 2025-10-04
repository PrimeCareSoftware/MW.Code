using System;

namespace MedicSoft.Application.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string Diagnosis { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public int ConsultationDurationMinutes { get; set; }
        public DateTime ConsultationStartTime { get; set; }
        public DateTime? ConsultationEndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ConsultationStartTime { get; set; }
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public int? ConsultationDurationMinutes { get; set; }
    }

    public class CompleteMedicalRecordDto
    {
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
    }
}
