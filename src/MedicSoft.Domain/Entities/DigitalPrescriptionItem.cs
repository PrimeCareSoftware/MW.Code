using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an individual medication item in a digital prescription.
    /// Compliant with CFM 1.643/2002 and ANVISA 344/1998.
    /// </summary>
    public class DigitalPrescriptionItem : BaseEntity
    {
        public Guid DigitalPrescriptionId { get; private set; }
        public Guid MedicationId { get; private set; }
        
        // Medication details (denormalized for prescription integrity)
        public string MedicationName { get; private set; }
        public string? GenericName { get; private set; } // DCB/DCI (Denominação Comum Brasileira/Internacional)
        public string? ActiveIngredient { get; private set; }
        
        // Controlled substance information (ANVISA 344/98)
        public bool IsControlledSubstance { get; private set; }
        public ControlledSubstanceList? ControlledList { get; private set; }
        public string? AnvisaRegistration { get; private set; }
        
        // Prescription details
        public string Dosage { get; private set; } // Ex: "500mg"
        public string PharmaceuticalForm { get; private set; } // Ex: "Comprimido", "Cápsula"
        public string Frequency { get; private set; } // Ex: "8 em 8 horas", "2x ao dia"
        public int DurationDays { get; private set; }
        public int Quantity { get; private set; } // Total quantity prescribed
        public string? AdministrationRoute { get; private set; } // Ex: "Via oral", "Uso tópico"
        public string? Instructions { get; private set; } // Special instructions
        
        // SNGPC tracking for controlled substances
        public string? BatchNumber { get; private set; }
        public DateTime? ManufactureDate { get; private set; }
        public DateTime? ExpiryDate { get; private set; }
        
        // Navigation properties
        public DigitalPrescription? DigitalPrescription { get; private set; }
        public Medication? Medication { get; private set; }

        private DigitalPrescriptionItem()
        {
            // EF Constructor
            MedicationName = null!;
            Dosage = null!;
            PharmaceuticalForm = null!;
            Frequency = null!;
        }

        public DigitalPrescriptionItem(
            Guid digitalPrescriptionId,
            Guid medicationId,
            string medicationName,
            string dosage,
            string pharmaceuticalForm,
            string frequency,
            int durationDays,
            int quantity,
            string tenantId,
            string? genericName = null,
            string? activeIngredient = null,
            bool isControlledSubstance = false,
            ControlledSubstanceList? controlledList = null,
            string? anvisaRegistration = null,
            string? administrationRoute = null,
            string? instructions = null) : base(tenantId)
        {
            if (digitalPrescriptionId == Guid.Empty)
                throw new ArgumentException("Digital prescription ID cannot be empty", nameof(digitalPrescriptionId));
            
            if (medicationId == Guid.Empty)
                throw new ArgumentException("Medication ID cannot be empty", nameof(medicationId));
            
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name is required", nameof(medicationName));
            
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage is required (CFM 1.643/2002)", nameof(dosage));
            
            if (string.IsNullOrWhiteSpace(pharmaceuticalForm))
                throw new ArgumentException("Pharmaceutical form is required", nameof(pharmaceuticalForm));
            
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency is required", nameof(frequency));
            
            if (durationDays <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationDays));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
            // Validate controlled substance data
            if (isControlledSubstance && controlledList == null)
                throw new ArgumentException("Controlled list must be specified for controlled substances", nameof(controlledList));

            DigitalPrescriptionId = digitalPrescriptionId;
            MedicationId = medicationId;
            
            MedicationName = medicationName.Trim();
            GenericName = genericName?.Trim();
            ActiveIngredient = activeIngredient?.Trim();
            
            IsControlledSubstance = isControlledSubstance;
            ControlledList = controlledList;
            AnvisaRegistration = anvisaRegistration?.Trim();
            
            Dosage = dosage.Trim();
            PharmaceuticalForm = pharmaceuticalForm.Trim();
            Frequency = frequency.Trim();
            DurationDays = durationDays;
            Quantity = quantity;
            AdministrationRoute = administrationRoute?.Trim();
            Instructions = instructions?.Trim();
        }

        public void Update(
            string dosage,
            string frequency,
            int durationDays,
            int quantity,
            string? administrationRoute = null,
            string? instructions = null)
        {
            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage is required", nameof(dosage));
            
            if (string.IsNullOrWhiteSpace(frequency))
                throw new ArgumentException("Frequency is required", nameof(frequency));
            
            if (durationDays <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationDays));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Dosage = dosage.Trim();
            Frequency = frequency.Trim();
            DurationDays = durationDays;
            Quantity = quantity;
            AdministrationRoute = administrationRoute?.Trim();
            Instructions = instructions?.Trim();
            
            UpdateTimestamp();
        }

        public void SetBatchInformation(string batchNumber, DateTime manufactureDate, DateTime expiryDate)
        {
            if (string.IsNullOrWhiteSpace(batchNumber))
                throw new ArgumentException("Batch number cannot be empty", nameof(batchNumber));
            
            if (manufactureDate >= expiryDate)
                throw new ArgumentException("Manufacture date must be before expiry date");
            
            if (expiryDate < DateTime.UtcNow)
                throw new ArgumentException("Medication is expired");

            BatchNumber = batchNumber.Trim();
            ManufactureDate = manufactureDate;
            ExpiryDate = expiryDate;
            
            UpdateTimestamp();
        }

        public bool IsExpired()
        {
            return ExpiryDate.HasValue && DateTime.UtcNow > ExpiryDate.Value;
        }

        public int DaysUntilExpiry()
        {
            if (!ExpiryDate.HasValue)
                return int.MaxValue;
            
            if (IsExpired())
                return 0;
            
            return (int)(ExpiryDate.Value - DateTime.UtcNow).TotalDays;
        }

        /// <summary>
        /// Calculates total daily dose based on frequency and dosage.
        /// Note: This is a simplified calculation and may need adjustment based on actual frequency patterns.
        /// </summary>
        public string GetDailyDoseDescription()
        {
            // This would need more sophisticated parsing based on frequency patterns
            // For now, return a simple description
            return $"{Dosage} - {Frequency}";
        }
    }
}
