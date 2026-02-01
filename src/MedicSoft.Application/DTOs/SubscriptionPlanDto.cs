using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for subscription plan information
    /// </summary>
    public class SubscriptionPlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal MonthlyPrice { get; set; }
        public int TrialDays { get; set; }
        public int MaxUsers { get; set; }
        public int MaxPatients { get; set; }
        public int MaxClinics { get; set; }
        public bool HasReports { get; set; }
        public bool HasWhatsAppIntegration { get; set; }
        public bool HasSMSNotifications { get; set; }
        public bool HasTissExport { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsRecommended { get; set; }
        
        // Campaign fields
        public string? CampaignName { get; set; }
        public string? CampaignDescription { get; set; }
        public decimal? OriginalPrice { get; set; }
        public decimal? CampaignPrice { get; set; }
        public DateTime? CampaignStartDate { get; set; }
        public DateTime? CampaignEndDate { get; set; }
        public int? MaxEarlyAdopters { get; set; }
        public int CurrentEarlyAdopters { get; set; }
        public bool IsCampaignActive { get; set; }
        public bool CanJoinCampaign { get; set; }
        public decimal EffectivePrice { get; set; }
        public int SavingsPercentage { get; set; }
        public List<string> EarlyAdopterBenefits { get; set; } = new();
        public List<string> FeaturesAvailable { get; set; } = new();
        public List<string> FeaturesInDevelopment { get; set; } = new();
    }
}
