using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an item in a prescription linking a medication to a medical record.
    /// </summary>
    public class PrescriptionItem : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public Guid MedicationId { get; private set; }
        public string Dosage { get; private set; }
        public string Frequency { get; private set; }
        public int DurationDays { get; private set; }
        public string? Instructions { get; private set; }
        public int Quantity { get; private set; }

        // Navigation properties
        public MedicalRecord? MedicalRecord { get; private set; }
        public Medication? Medication { get; private set; }

        private PrescriptionItem()
        {
            // EF Constructor
            Dosage = null!;
            Frequency = null!;
        }

        public PrescriptionItem(Guid medicalRecordId, Guid medicationId,
            string dosage, string frequency, int durationDays,
            int quantity, string tenantId, string? instructions = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));

            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication ID cannot be empty", nameof(medicationId));

            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage cannot be empty", nameof(dosage));

            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency cannot be empty", nameof(frequency));

            if (durationDays <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationDays));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            MedicalRecordId = medicalRecordId;
            MedicationId = medicationId;
            Dosage = dosage.Trim();
            Frequency = frequency.Trim();
            DurationDays = durationDays;
            Quantity = quantity;
            Instructions = instructions?.Trim();
        }

        public void Update(string dosage, string frequency, int durationDays,
            int quantity, string? instructions = null)
        {
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage cannot be empty", nameof(dosage));

            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency cannot be empty", nameof(frequency));

            if (durationDays <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationDays));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Dosage = dosage.Trim();
            Frequency = frequency.Trim();
            DurationDays = durationDays;
            Quantity = quantity;
            Instructions = instructions?.Trim();
            UpdateTimestamp();
        }
    }
}
