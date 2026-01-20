using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the active consultation form configuration for a clinic
    /// Allows clinic owners to customize which fields are shown/hidden and add custom fields
    /// </summary>
    public class ConsultationFormConfiguration : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid? ProfileId { get; private set; } // Reference to the profile used as base
        public string ConfigurationName { get; private set; }
        public bool IsActive { get; private set; }
        
        // Field visibility controls (CFM 1.821 mandatory fields)
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

        // Navigation properties
        public Clinic Clinic { get; private set; } = null!;
        public ConsultationFormProfile? Profile { get; private set; }

        private ConsultationFormConfiguration()
        {
            // EF Constructor
            ConfigurationName = null!;
        }

        public ConsultationFormConfiguration(
            Guid clinicId,
            string configurationName,
            string tenantId,
            Guid? profileId = null,
            bool showChiefComplaint = true,
            bool showHistoryOfPresentIllness = true,
            bool showPastMedicalHistory = true,
            bool showFamilyHistory = true,
            bool showLifestyleHabits = true,
            bool showCurrentMedications = true,
            List<CustomField>? customFields = null) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (string.IsNullOrWhiteSpace(configurationName))
                throw new ArgumentException("Configuration name cannot be empty", nameof(configurationName));

            ClinicId = clinicId;
            ProfileId = profileId;
            ConfigurationName = configurationName.Trim();
            IsActive = true;
            
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
            string configurationName,
            bool showChiefComplaint,
            bool showHistoryOfPresentIllness,
            bool showPastMedicalHistory,
            bool showFamilyHistory,
            bool showLifestyleHabits,
            bool showCurrentMedications,
            List<CustomField>? customFields = null)
        {
            if (string.IsNullOrWhiteSpace(configurationName))
                throw new ArgumentException("Configuration name cannot be empty", nameof(configurationName));

            ConfigurationName = configurationName.Trim();
            
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

        public void UpdateFromProfile(Guid profileId)
        {
            ProfileId = profileId;
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

        public void AddCustomField(CustomField customField)
        {
            var fields = GetCustomFields();
            
            // Check for duplicate field keys
            if (fields.Any(f => f.FieldKey == customField.FieldKey))
                throw new InvalidOperationException($"A field with key '{customField.FieldKey}' already exists");
            
            fields.Add(customField);
            CustomFieldsJson = JsonSerializer.Serialize(fields);
            UpdateTimestamp();
        }

        public void RemoveCustomField(string fieldKey)
        {
            var fields = GetCustomFields();
            fields = fields.Where(f => f.FieldKey != fieldKey).ToList();
            CustomFieldsJson = JsonSerializer.Serialize(fields);
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
}
