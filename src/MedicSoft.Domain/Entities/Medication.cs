using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a medication/medicine in the system.
    /// Used for prescription autocomplete and medication database.
    /// </summary>
    public class Medication : BaseEntity
    {
        public string Name { get; private set; }
        public string? GenericName { get; private set; }
        public string? Manufacturer { get; private set; }
        public string? ActiveIngredient { get; private set; }
        public string Dosage { get; private set; }
        public string PharmaceuticalForm { get; private set; } // Comprimido, Cápsula, Xarope, Pomada, etc.
        public string? Concentration { get; private set; }
        public string? AdministrationRoute { get; private set; } // Oral, Intravenosa, Tópica, etc.
        public MedicationCategory Category { get; private set; }
        public bool RequiresPrescription { get; private set; }
        public bool IsControlled { get; private set; } // Controlled substance (Portaria 344/98)
        public ControlledSubstanceList? ControlledList { get; private set; } // ANVISA Portaria 344/98 classification
        public string? AnvisaRegistration { get; private set; } // ANVISA registration number
        public string? Barcode { get; private set; } // EAN-13 barcode
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }

        private Medication()
        {
            // EF Constructor
            Name = null!;
            Dosage = null!;
            PharmaceuticalForm = null!;
        }

        public Medication(string name, string dosage, string pharmaceuticalForm,
            MedicationCategory category, bool requiresPrescription, string tenantId,
            string? genericName = null, string? manufacturer = null,
            string? activeIngredient = null, string? concentration = null,
            string? administrationRoute = null, bool isControlled = false,
            ControlledSubstanceList? controlledList = null,
            string? anvisaRegistration = null, string? barcode = null,
            string? description = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage cannot be empty", nameof(dosage));

            if (string.IsNullOrWhiteSpace(pharmaceuticalForm))
                throw new ArgumentException("Pharmaceutical form cannot be empty", nameof(pharmaceuticalForm));

            Name = name.Trim();
            GenericName = genericName?.Trim();
            Manufacturer = manufacturer?.Trim();
            ActiveIngredient = activeIngredient?.Trim();
            Dosage = dosage.Trim();
            PharmaceuticalForm = pharmaceuticalForm.Trim();
            Concentration = concentration?.Trim();
            AdministrationRoute = administrationRoute?.Trim();
            Category = category;
            RequiresPrescription = requiresPrescription;
            IsControlled = isControlled;
            ControlledList = controlledList;
            
            // Validate controlled substance data
            if (isControlled && controlledList == null)
                throw new ArgumentException("Controlled list must be specified for controlled substances", nameof(controlledList));
            
            AnvisaRegistration = anvisaRegistration?.Trim();
            Barcode = barcode?.Trim();
            Description = description?.Trim();
            IsActive = true;
        }

        public void Update(string name, string dosage, string pharmaceuticalForm,
            MedicationCategory category, bool requiresPrescription,
            string? genericName = null, string? manufacturer = null,
            string? activeIngredient = null, string? concentration = null,
            string? administrationRoute = null, bool isControlled = false,
            ControlledSubstanceList? controlledList = null,
            string? anvisaRegistration = null, string? barcode = null,
            string? description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(dosage))
                throw new ArgumentException("Dosage cannot be empty", nameof(dosage));

            if (string.IsNullOrWhiteSpace(pharmaceuticalForm))
                throw new ArgumentException("Pharmaceutical form cannot be empty", nameof(pharmaceuticalForm));

            Name = name.Trim();
            GenericName = genericName?.Trim();
            Manufacturer = manufacturer?.Trim();
            ActiveIngredient = activeIngredient?.Trim();
            Dosage = dosage.Trim();
            PharmaceuticalForm = pharmaceuticalForm.Trim();
            Concentration = concentration?.Trim();
            AdministrationRoute = administrationRoute?.Trim();
            Category = category;
            RequiresPrescription = requiresPrescription;
            IsControlled = isControlled;
            ControlledList = controlledList;
            
            // Validate controlled substance data
            if (isControlled && controlledList == null)
                throw new ArgumentException("Controlled list must be specified for controlled substances", nameof(controlledList));
            
            AnvisaRegistration = anvisaRegistration?.Trim();
            Barcode = barcode?.Trim();
            Description = description?.Trim();
            UpdateTimestamp();
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
    }

    public enum MedicationCategory
    {
        Analgesic,              // Analgésico
        Antibiotic,             // Antibiótico
        AntiInflammatory,       // Anti-inflamatório
        Antihypertensive,       // Anti-hipertensivo
        Antihistamine,          // Anti-histamínico
        Antidiabetic,           // Antidiabético
        Antidepressant,         // Antidepressivo
        Anxiolytic,             // Ansiolítico
        Antacid,                // Antiácido
        Bronchodilator,         // Broncodilatador
        Diuretic,               // Diurético
        Anticoagulant,          // Anticoagulante
        Corticosteroid,         // Corticosteroide
        Vitamin,                // Vitamina
        Supplement,             // Suplemento
        Vaccine,                // Vacina
        Contraceptive,          // Anticoncepcional
        Antifungal,             // Antifúngico
        Antiviral,              // Antiviral
        Antiparasitic,          // Antiparasitário
        Other                   // Outros
    }
}
