using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the link between a patient and a clinic.
    /// A patient can be linked to multiple clinics (N:N relationship).
    /// Each clinic can only access their own medical records for the patient.
    /// </summary>
    public class PatientClinicLink : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid ClinicId { get; private set; }
        public Guid? PrimaryDoctorId { get; private set; }  // Médico responsável pelo paciente nesta clínica
        public DateTime LinkedAt { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public Patient Patient { get; private set; } = null!;
        public Clinic Clinic { get; private set; } = null!;
        public User? PrimaryDoctor { get; private set; }

        private PatientClinicLink()
        {
            // EF Constructor
        }

        public PatientClinicLink(Guid patientId, Guid clinicId, string tenantId) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            PatientId = patientId;
            ClinicId = clinicId;
            LinkedAt = DateTime.UtcNow;
            IsActive = true;
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void SetPrimaryDoctor(Guid? primaryDoctorId)
        {
            PrimaryDoctorId = primaryDoctorId;
            UpdateTimestamp();
        }
    }
}
