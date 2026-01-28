using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SystemAdmin
{
    public enum HealthStatus
    {
        Healthy = 0,
        NeedsAttention = 1,
        AtRisk = 2
    }

    public enum ExportFormat
    {
        Csv = 0,
        Excel = 1,
        Pdf = 2
    }

    /// <summary>
    /// Detailed clinic information with related data
    /// </summary>
    public class ClinicDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? Subdomain { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Subscription info
        public SubscriptionInfoDto? CurrentSubscription { get; set; }
        
        // User counts
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        
        // Support tickets
        public int OpenTickets { get; set; }
        public int TotalTickets { get; set; }
        
        // Tags
        public List<TagDto> Tags { get; set; } = new();
    }

    public class SubscriptionInfoDto
    {
        public string PlanName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime? TrialEndDate { get; set; }
        public decimal CurrentPrice { get; set; }
    }

    /// <summary>
    /// Health score calculation for a clinic
    /// </summary>
    public class ClinicHealthScoreDto
    {
        public Guid ClinicId { get; set; }
        public int UsageScore { get; set; } // 0-30 points
        public int UserEngagementScore { get; set; } // 0-25 points
        public int SupportScore { get; set; } // 0-20 points
        public int PaymentScore { get; set; } // 0-25 points
        public int TotalScore { get; set; } // 0-100 points
        public HealthStatus HealthStatus { get; set; }
        public DateTime CalculatedAt { get; set; }
        
        // Additional details
        public DateTime? LastActivity { get; set; }
        public int DaysSinceActivity { get; set; }
        public int ActiveUsersCount { get; set; }
        public int TotalUsersCount { get; set; }
        public int OpenTicketsCount { get; set; }
        public bool HasPaymentIssues { get; set; }
    }

    /// <summary>
    /// Timeline event for clinic history
    /// </summary>
    public class ClinicTimelineEventDto
    {
        public string Type { get; set; } = string.Empty; // subscription, ticket, audit, user, payment
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Icon { get; set; } = string.Empty; // Material icon name
        public string? Metadata { get; set; } // JSON for additional data
    }

    /// <summary>
    /// Usage metrics for a clinic
    /// </summary>
    public class ClinicUsageMetricsDto
    {
        public Guid ClinicId { get; set; }
        
        // Time-based metrics
        public int Last7DaysLogins { get; set; }
        public int Last30DaysLogins { get; set; }
        public DateTime? LastLoginDate { get; set; }
        
        // Feature usage
        public int AppointmentsCreated { get; set; }
        public int PatientsRegistered { get; set; }
        public int DocumentsGenerated { get; set; }
        
        // Period
        public DateTime MetricsPeriodStart { get; set; }
        public DateTime MetricsPeriodEnd { get; set; }
    }

    /// <summary>
    /// Filter criteria for clinic queries
    /// </summary>
    public class ClinicFilterDto
    {
        public string? SearchTerm { get; set; }
        public bool? IsActive { get; set; }
        public List<string>? Tags { get; set; }
        public HealthStatus? HealthStatus { get; set; }
        public string? SubscriptionStatus { get; set; }
        public DateTime? CreatedAfter { get; set; }
        public DateTime? CreatedBefore { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SortBy { get; set; } // name, createdAt, lastActivity
        public bool SortDescending { get; set; } = false;
    }

    /// <summary>
    /// Tag DTO
    /// </summary>
    public class TagDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public bool IsAutomatic { get; set; }
        public int Order { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateTagDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = "#3B82F6";
        public bool IsAutomatic { get; set; }
        public string? AutomationRules { get; set; }
        public int Order { get; set; }
    }

    public class UpdateTagDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int Order { get; set; }
    }

    /// <summary>
    /// Assign or remove tags from clinics
    /// </summary>
    public class AssignTagDto
    {
        public Guid TagId { get; set; }
        public List<Guid> ClinicIds { get; set; } = new();
    }

    /// <summary>
    /// Bulk action on multiple clinics
    /// </summary>
    public class BulkActionDto
    {
        public List<Guid> ClinicIds { get; set; } = new();
        public string Action { get; set; } = string.Empty; // activate, deactivate, addTag, removeTag, changePlan, sendEmail
        public Dictionary<string, string>? Parameters { get; set; }
    }

    /// <summary>
    /// Cross-tenant user DTO
    /// </summary>
    public class CrossTenantUserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Clinic info
        public Guid? ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public string? ClinicSubdomain { get; set; }
    }

    public class CrossTenantUserFilterDto
    {
        public string? SearchTerm { get; set; }
        public string? Role { get; set; }
        public bool? IsActive { get; set; }
        public Guid? ClinicId { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    /// <summary>
    /// Reset password request
    /// </summary>
    public class ResetPasswordDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }
}
