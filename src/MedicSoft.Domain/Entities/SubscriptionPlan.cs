using System;
using System.Linq;
using System.Text.Json;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a subscription plan available for clinics.
    /// Includes free trial and paid plans with different features.
    /// </summary>
    public class SubscriptionPlan : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal MonthlyPrice { get; private set; }
        public int TrialDays { get; private set; }
        public int MaxUsers { get; private set; }
        public int MaxPatients { get; private set; }
        public int MaxClinics { get; private set; }
        public bool HasReports { get; private set; }
        public bool HasWhatsAppIntegration { get; private set; }
        public bool HasSMSNotifications { get; private set; }
        public bool HasTissExport { get; private set; }
        public bool IsActive { get; private set; }
        public SubscriptionPlanType Type { get; private set; }
        public string? EnabledModules { get; private set; } // JSON array of enabled modules

        private SubscriptionPlan()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
        }

        public SubscriptionPlan(string name, string description, decimal monthlyPrice,
            int trialDays, int maxUsers, int maxPatients, SubscriptionPlanType type, string tenantId,
            bool hasReports = false, bool hasWhatsAppIntegration = false,
            bool hasSMSNotifications = false, bool hasTissExport = false, int maxClinics = 1) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (monthlyPrice < 0)
                throw new ArgumentException("Monthly price cannot be negative", nameof(monthlyPrice));

            if (trialDays < 0)
                throw new ArgumentException("Trial days cannot be negative", nameof(trialDays));

            if (maxUsers <= 0)
                throw new ArgumentException("Max users must be greater than zero", nameof(maxUsers));

            if (maxPatients <= 0)
                throw new ArgumentException("Max patients must be greater than zero", nameof(maxPatients));

            if (maxClinics <= 0)
                throw new ArgumentException("Max clinics must be greater than zero", nameof(maxClinics));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            MonthlyPrice = monthlyPrice;
            TrialDays = trialDays;
            MaxUsers = maxUsers;
            MaxPatients = maxPatients;
            MaxClinics = maxClinics;
            Type = type;
            HasReports = hasReports;
            HasWhatsAppIntegration = hasWhatsAppIntegration;
            HasSMSNotifications = hasSMSNotifications;
            HasTissExport = hasTissExport;
            IsActive = true;
        }

        public void Update(string name, string description, decimal monthlyPrice,
            int maxUsers, int maxPatients, bool hasReports, bool hasWhatsAppIntegration,
            bool hasSMSNotifications, bool hasTissExport, int maxClinics = 1)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (monthlyPrice < 0)
                throw new ArgumentException("Monthly price cannot be negative", nameof(monthlyPrice));

            if (maxUsers <= 0)
                throw new ArgumentException("Max users must be greater than zero", nameof(maxUsers));

            if (maxPatients <= 0)
                throw new ArgumentException("Max patients must be greater than zero", nameof(maxPatients));

            if (maxClinics <= 0)
                throw new ArgumentException("Max clinics must be greater than zero", nameof(maxClinics));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            MonthlyPrice = monthlyPrice;
            MaxUsers = maxUsers;
            MaxPatients = maxPatients;
            MaxClinics = maxClinics;
            HasReports = hasReports;
            HasWhatsAppIntegration = hasWhatsAppIntegration;
            HasSMSNotifications = hasSMSNotifications;
            HasTissExport = hasTissExport;
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

        /// <summary>
        /// Set enabled modules for this plan
        /// </summary>
        public void SetEnabledModules(string[] modules)
        {
            // Validate module names (optional - could be moved to service layer)
            var validModules = SystemModules.GetAllModules();
            var invalidModules = modules.Where(m => !validModules.Contains(m)).ToArray();
            
            if (invalidModules.Length > 0)
            {
                throw new ArgumentException($"Invalid module names: {string.Join(", ", invalidModules)}");
            }

            EnabledModules = JsonSerializer.Serialize(modules);
            UpdateTimestamp();
        }

        /// <summary>
        /// Get enabled modules for this plan
        /// </summary>
        public string[] GetEnabledModules()
        {
            if (string.IsNullOrEmpty(EnabledModules))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(EnabledModules) ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                // Log error and return empty array if JSON is corrupted
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Check if a module is available in this plan
        /// </summary>
        public bool HasModule(string moduleName)
        {
            // Check JSON-based enabled modules first
            var enabledModules = GetEnabledModules();
            if (enabledModules.Length > 0)
                return enabledModules.Contains(moduleName);

            // Fallback to legacy boolean properties
            return moduleName switch
            {
                "Reports" => HasReports,
                "WhatsAppIntegration" => HasWhatsAppIntegration,
                "SMSNotifications" => HasSMSNotifications,
                "TissExport" => HasTissExport,
                _ => true // Core modules are available in all plans
            };
        }
    }

    public enum SubscriptionPlanType
    {
        Trial,      // Free trial
        Basic,      // Basic paid plan
        Standard,   // Standard paid plan
        Premium,    // Premium paid plan
        Enterprise  // Enterprise plan
    }
}
