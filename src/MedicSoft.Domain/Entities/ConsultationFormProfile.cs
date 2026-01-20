using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a predefined profile template for consultation forms
    /// Templates for different specialties (doctors, psychologists, nutritionists, etc.)
    /// </summary>
    public class ConsultationFormProfile : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ProfessionalSpecialty Specialty { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsSystemDefault { get; private set; } // System profiles cannot be deleted
        
        // Default field visibility for this profile
        public bool ShowChiefComplaint { get; private set; }
        public bool ShowHistoryOfPresentIllness { get; private set; }
        public bool ShowPastMedicalHistory { get; private set; }
        public bool ShowFamilyHistory { get; private set; }
        public bool ShowLifestyleHabits { get; private set; }
        public bool ShowCurrentMedications { get; private set; }
        
        // Custom fields serialized as JSON
        private string _customFieldsJson = "[]";
        public string CustomFieldsJson 
        { 
            get => _customFieldsJson;
            private set => _customFieldsJson = value ?? "[]";
        }

        // Navigation property
        private readonly List<ConsultationFormConfiguration> _configurations = new();
        public IReadOnlyCollection<ConsultationFormConfiguration> Configurations => _configurations.AsReadOnly();

        private ConsultationFormProfile()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
        }

        public ConsultationFormProfile(
            string name,
            string description,
            ProfessionalSpecialty specialty,
            string tenantId,
            bool showChiefComplaint = true,
            bool showHistoryOfPresentIllness = true,
            bool showPastMedicalHistory = true,
            bool showFamilyHistory = true,
            bool showLifestyleHabits = true,
            bool showCurrentMedications = true,
            List<CustomField>? customFields = null,
            bool isSystemDefault = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Specialty = specialty;
            IsActive = true;
            IsSystemDefault = isSystemDefault;
            
            ShowChiefComplaint = showChiefComplaint;
            ShowHistoryOfPresentIllness = showHistoryOfPresentIllness;
            ShowPastMedicalHistory = showPastMedicalHistory;
            ShowFamilyHistory = showFamilyHistory;
            ShowLifestyleHabits = showLifestyleHabits;
            ShowCurrentMedications = showCurrentMedications;
            
            if (customFields != null && customFields.Any())
            {
                CustomFieldsJson = JsonSerializer.Serialize(customFields);
            }
        }

        public void Update(
            string name,
            string description,
            bool showChiefComplaint,
            bool showHistoryOfPresentIllness,
            bool showPastMedicalHistory,
            bool showFamilyHistory,
            bool showLifestyleHabits,
            bool showCurrentMedications,
            List<CustomField>? customFields = null)
        {
            if (IsSystemDefault)
                throw new InvalidOperationException("System default profiles cannot be modified directly. Create a copy instead.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            
            ShowChiefComplaint = showChiefComplaint;
            ShowHistoryOfPresentIllness = showHistoryOfPresentIllness;
            ShowPastMedicalHistory = showPastMedicalHistory;
            ShowFamilyHistory = showFamilyHistory;
            ShowLifestyleHabits = showLifestyleHabits;
            ShowCurrentMedications = showCurrentMedications;
            
            if (customFields != null)
            {
                CustomFieldsJson = JsonSerializer.Serialize(customFields);
            }
            
            UpdateTimestamp();
        }

        public List<CustomField> GetCustomFields()
        {
            try
            {
                return JsonSerializer.Deserialize<List<CustomField>>(CustomFieldsJson) ?? new List<CustomField>();
            }
            catch
            {
                return new List<CustomField>();
            }
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            if (IsSystemDefault)
                throw new InvalidOperationException("System default profiles cannot be deactivated");
            
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
