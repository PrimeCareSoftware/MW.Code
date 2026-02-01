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
        
        // Campaign fields
        public string? CampaignName { get; private set; }
        public string? CampaignDescription { get; private set; }
        public decimal? OriginalPrice { get; private set; }
        public decimal? CampaignPrice { get; private set; }
        public DateTime? CampaignStartDate { get; private set; }
        public DateTime? CampaignEndDate { get; private set; }
        public int? MaxEarlyAdopters { get; private set; }
        public int CurrentEarlyAdopters { get; private set; }
        public string? EarlyAdopterBenefits { get; private set; } // JSON array
        public string? FeaturesAvailable { get; private set; } // JSON array
        public string? FeaturesInDevelopment { get; private set; } // JSON array

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

        /// <summary>
        /// Set campaign pricing and details
        /// </summary>
        public void SetCampaignPricing(string campaignName, string campaignDescription,
            decimal originalPrice, decimal campaignPrice, DateTime? startDate = null,
            DateTime? endDate = null, int? maxEarlyAdopters = null)
        {
            if (string.IsNullOrWhiteSpace(campaignName))
                throw new ArgumentException("Campaign name cannot be empty", nameof(campaignName));

            if (originalPrice < 0)
                throw new ArgumentException("Original price cannot be negative", nameof(originalPrice));

            if (campaignPrice < 0)
                throw new ArgumentException("Campaign price cannot be negative", nameof(campaignPrice));

            if (campaignPrice > originalPrice)
                throw new ArgumentException("Campaign price cannot be higher than original price", nameof(campaignPrice));

            if (maxEarlyAdopters.HasValue && maxEarlyAdopters.Value <= 0)
                throw new ArgumentException("Max early adopters must be greater than zero", nameof(maxEarlyAdopters));

            CampaignName = campaignName.Trim();
            CampaignDescription = campaignDescription?.Trim() ?? string.Empty;
            OriginalPrice = originalPrice;
            CampaignPrice = campaignPrice;
            CampaignStartDate = startDate ?? DateTime.UtcNow;
            CampaignEndDate = endDate; // null means lifetime campaign
            MaxEarlyAdopters = maxEarlyAdopters;
            // Only reset counter when creating a new campaign (not when updating existing one)
            // Caller should preserve CurrentEarlyAdopters if updating
            if (CurrentEarlyAdopters == 0)
            {
                CurrentEarlyAdopters = 0; // Explicitly set for new campaigns
            }
            
            // Update MonthlyPrice to reflect campaign price
            MonthlyPrice = campaignPrice;
            
            UpdateTimestamp();
        }

        /// <summary>
        /// Clear campaign pricing (return to normal pricing)
        /// </summary>
        public void ClearCampaignPricing()
        {
            // If there was a campaign with original price, restore it
            if (OriginalPrice.HasValue)
            {
                MonthlyPrice = OriginalPrice.Value;
            }

            CampaignName = null;
            CampaignDescription = null;
            OriginalPrice = null;
            CampaignPrice = null;
            CampaignStartDate = null;
            CampaignEndDate = null;
            MaxEarlyAdopters = null;
            CurrentEarlyAdopters = 0;
            EarlyAdopterBenefits = null;
            FeaturesAvailable = null;
            FeaturesInDevelopment = null;
            
            UpdateTimestamp();
        }

        /// <summary>
        /// Check if campaign is currently active
        /// </summary>
        public bool IsCampaignActive()
        {
            if (string.IsNullOrEmpty(CampaignName) || !CampaignPrice.HasValue)
                return false;

            var now = DateTime.UtcNow;

            // Check if campaign has started
            if (CampaignStartDate.HasValue && now < CampaignStartDate.Value)
                return false;

            // Check if campaign has ended
            if (CampaignEndDate.HasValue && now > CampaignEndDate.Value)
                return false;

            // Check if early adopter slots are full
            if (MaxEarlyAdopters.HasValue && CurrentEarlyAdopters >= MaxEarlyAdopters.Value)
                return false;

            return true;
        }

        /// <summary>
        /// Check if a new subscriber can join the campaign
        /// </summary>
        public bool CanJoinCampaign()
        {
            if (!IsCampaignActive())
                return false;

            // Check if there are available early adopter slots
            if (MaxEarlyAdopters.HasValue)
                return CurrentEarlyAdopters < MaxEarlyAdopters.Value;

            return true;
        }

        /// <summary>
        /// Increment early adopter count when someone joins the campaign
        /// </summary>
        public void IncrementEarlyAdopters()
        {
            if (!CanJoinCampaign())
                throw new InvalidOperationException("Cannot join campaign - campaign is not active or slots are full");

            CurrentEarlyAdopters++;
            UpdateTimestamp();
        }

        /// <summary>
        /// Get the effective price (campaign price if active, otherwise regular price)
        /// </summary>
        public decimal GetEffectivePrice()
        {
            if (IsCampaignActive() && CampaignPrice.HasValue)
                return CampaignPrice.Value;

            return MonthlyPrice;
        }

        /// <summary>
        /// Calculate savings percentage if campaign is active
        /// </summary>
        public int GetSavingsPercentage()
        {
            if (!IsCampaignActive() || !OriginalPrice.HasValue || !CampaignPrice.HasValue)
                return 0;

            if (OriginalPrice.Value == 0)
                return 0;

            return (int)Math.Round(((OriginalPrice.Value - CampaignPrice.Value) / OriginalPrice.Value) * 100);
        }

        /// <summary>
        /// Set early adopter benefits
        /// </summary>
        public void SetEarlyAdopterBenefits(string[] benefits)
        {
            if (benefits == null || benefits.Length == 0)
            {
                EarlyAdopterBenefits = null;
            }
            else
            {
                EarlyAdopterBenefits = JsonSerializer.Serialize(benefits);
            }
            UpdateTimestamp();
        }

        /// <summary>
        /// Get early adopter benefits
        /// </summary>
        public string[] GetEarlyAdopterBenefits()
        {
            if (string.IsNullOrEmpty(EarlyAdopterBenefits))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(EarlyAdopterBenefits) ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Set available features
        /// </summary>
        public void SetFeaturesAvailable(string[] features)
        {
            if (features == null || features.Length == 0)
            {
                FeaturesAvailable = null;
            }
            else
            {
                FeaturesAvailable = JsonSerializer.Serialize(features);
            }
            UpdateTimestamp();
        }

        /// <summary>
        /// Get available features
        /// </summary>
        public string[] GetFeaturesAvailable()
        {
            if (string.IsNullOrEmpty(FeaturesAvailable))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(FeaturesAvailable) ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                return Array.Empty<string>();
            }
        }

        /// <summary>
        /// Set features in development
        /// </summary>
        public void SetFeaturesInDevelopment(string[] features)
        {
            if (features == null || features.Length == 0)
            {
                FeaturesInDevelopment = null;
            }
            else
            {
                FeaturesInDevelopment = JsonSerializer.Serialize(features);
            }
            UpdateTimestamp();
        }

        /// <summary>
        /// Get features in development
        /// </summary>
        public string[] GetFeaturesInDevelopment()
        {
            if (string.IsNullOrEmpty(FeaturesInDevelopment))
                return Array.Empty<string>();

            try
            {
                return JsonSerializer.Deserialize<string[]>(FeaturesInDevelopment) ?? Array.Empty<string>();
            }
            catch (JsonException)
            {
                return Array.Empty<string>();
            }
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
