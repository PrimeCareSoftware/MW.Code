using System;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class MedicationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Manufacturer { get; set; }
        public string? ActiveIngredient { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string PharmaceuticalForm { get; set; } = string.Empty;
        public string? Concentration { get; set; }
        public string? AdministrationRoute { get; set; }
        public MedicationCategory Category { get; set; }
        public bool RequiresPrescription { get; set; }
        public bool IsControlled { get; set; }
        public string? AnvisaRegistration { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class MedicationAutocompleteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string PharmaceuticalForm { get; set; } = string.Empty;
        public string? AdministrationRoute { get; set; }
        public string DisplayText => $"{Name} {Dosage} - {PharmaceuticalForm}";
    }

    public class CreateMedicationDto
    {
        public string Name { get; set; } = string.Empty;
        public string? GenericName { get; set; }
        public string? Manufacturer { get; set; }
        public string? ActiveIngredient { get; set; }
        public string Dosage { get; set; } = string.Empty;
        public string PharmaceuticalForm { get; set; } = string.Empty;
        public string? Concentration { get; set; }
        public string? AdministrationRoute { get; set; }
        public MedicationCategory Category { get; set; }
        public bool RequiresPrescription { get; set; }
        public bool IsControlled { get; set; }
        public string? AnvisaRegistration { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
    }

    public class UpdateMedicationDto
    {
        public string? Name { get; set; }
        public string? GenericName { get; set; }
        public string? Manufacturer { get; set; }
        public string? ActiveIngredient { get; set; }
        public string? Dosage { get; set; }
        public string? PharmaceuticalForm { get; set; }
        public string? Concentration { get; set; }
        public string? AdministrationRoute { get; set; }
        public MedicationCategory? Category { get; set; }
        public bool? RequiresPrescription { get; set; }
        public bool? IsControlled { get; set; }
        public string? AnvisaRegistration { get; set; }
        public string? Barcode { get; set; }
        public string? Description { get; set; }
    }
}
