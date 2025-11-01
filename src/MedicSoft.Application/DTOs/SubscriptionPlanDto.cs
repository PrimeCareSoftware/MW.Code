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
        public bool HasReports { get; set; }
        public bool HasWhatsAppIntegration { get; set; }
        public bool HasSMSNotifications { get; set; }
        public bool HasTissExport { get; set; }
        public bool IsActive { get; set; }
        public int Type { get; set; }
        public List<string> Features { get; set; } = new();
        public bool IsRecommended { get; set; }
    }
}
