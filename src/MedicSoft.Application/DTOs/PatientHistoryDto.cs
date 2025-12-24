using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO representing a patient's appointment history entry
    /// </summary>
    public class PatientAppointmentHistoryDto
    {
        public Guid AppointmentId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? DoctorName { get; set; }
        public string? DoctorSpecialty { get; set; }
        public string? DoctorProfessionalId { get; set; } // CRM, CRO, etc.
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        // Payment information
        public PaymentHistoryDto? Payment { get; set; }
        
        // Medical record information (only if user has permission)
        public MedicalRecordSummaryDto? MedicalRecord { get; set; }
    }

    /// <summary>
    /// DTO representing payment information for history
    /// </summary>
    public class PaymentHistoryDto
    {
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty; // Cash, CreditCard, DebitCard, Pix, etc.
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? PixKey { get; set; }
    }

    /// <summary>
    /// DTO representing a summary of medical record (limited info for history)
    /// </summary>
    public class MedicalRecordSummaryDto
    {
        public Guid MedicalRecordId { get; set; }
        public string Diagnosis { get; set; } = string.Empty;
        public int ConsultationDurationMinutes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// DTO representing a patient's procedure history entry
    /// </summary>
    public class PatientProcedureHistoryDto
    {
        public Guid ProcedureId { get; set; }
        public Guid AppointmentId { get; set; }
        public string ProcedureName { get; set; } = string.Empty;
        public string ProcedureCode { get; set; } = string.Empty;
        public string ProcedureCategory { get; set; } = string.Empty;
        public decimal PriceCharged { get; set; }
        public DateTime PerformedAt { get; set; }
        public string? Notes { get; set; }
        
        // Doctor information
        public string? DoctorName { get; set; }
        public string? DoctorSpecialty { get; set; }
        
        // Payment information
        public PaymentHistoryDto? Payment { get; set; }
    }

    /// <summary>
    /// Complete patient history with appointments and procedures
    /// </summary>
    public class PatientCompleteHistoryDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public List<PatientAppointmentHistoryDto> Appointments { get; set; } = new();
        public List<PatientProcedureHistoryDto> Procedures { get; set; } = new();
    }
}
