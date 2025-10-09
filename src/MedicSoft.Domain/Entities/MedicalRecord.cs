using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class MedicalRecord : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        public string Diagnosis { get; private set; }
        public string Prescription { get; private set; }
        public string Notes { get; private set; }
        public int ConsultationDurationMinutes { get; private set; }
        public DateTime ConsultationStartTime { get; private set; }
        public DateTime? ConsultationEndTime { get; private set; }

        // Navigation properties
        public Appointment Appointment { get; private set; } = null!;
        public Patient Patient { get; private set; } = null!;
        
        // Navigation property for prescription items
        private readonly List<PrescriptionItem> _prescriptionItems = new();
        public IReadOnlyCollection<PrescriptionItem> PrescriptionItems => _prescriptionItems.AsReadOnly();

        private MedicalRecord() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            Diagnosis = null!;
            Prescription = null!;
            Notes = null!;
        }

        public MedicalRecord(Guid appointmentId, Guid patientId, string tenantId, 
            DateTime consultationStartTime, string? diagnosis = null, 
            string? prescription = null, string? notes = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            AppointmentId = appointmentId;
            PatientId = patientId;
            ConsultationStartTime = consultationStartTime;
            Diagnosis = diagnosis?.Trim() ?? string.Empty;
            Prescription = prescription?.Trim() ?? string.Empty;
            Notes = notes?.Trim() ?? string.Empty;
            ConsultationDurationMinutes = 0;
        }

        public void UpdateDiagnosis(string diagnosis)
        {
            Diagnosis = diagnosis?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }

        public void UpdatePrescription(string prescription)
        {
            Prescription = prescription?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim() ?? string.Empty;
            UpdateTimestamp();
        }

        public void CompleteConsultation(string? diagnosis = null, string? prescription = null, string? notes = null)
        {
            ConsultationEndTime = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(diagnosis))
                Diagnosis = diagnosis.Trim();
            
            if (!string.IsNullOrWhiteSpace(prescription))
                Prescription = prescription.Trim();
            
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes.Trim();

            if (ConsultationEndTime.HasValue)
            {
                var duration = ConsultationEndTime.Value - ConsultationStartTime;
                ConsultationDurationMinutes = (int)duration.TotalMinutes;
            }

            UpdateTimestamp();
        }

        public void UpdateConsultationTime(int durationMinutes)
        {
            if (durationMinutes < 0)
                throw new ArgumentException("Duration cannot be negative", nameof(durationMinutes));

            ConsultationDurationMinutes = durationMinutes;
            UpdateTimestamp();
        }
    }
}
