namespace MedicSoft.SystemAdmin.Api.Models;

public class ClinicSummaryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public string SubscriptionStatus { get; set; } = string.Empty;
    public string PlanName { get; set; } = string.Empty;
    public DateTime? NextBillingDate { get; set; }
}

public class ClinicDetailDto : ClinicSummaryDto
{
    public decimal PlanPrice { get; set; }
    public DateTime? TrialEndsAt { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
}

public class CreateClinicRequest
{
    public string Name { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string OwnerUsername { get; set; } = string.Empty;
    public string OwnerPassword { get; set; } = string.Empty;
    public string OwnerFullName { get; set; } = string.Empty;
    public string? PlanId { get; set; }
}

public class CreateSystemOwnerRequest
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
}

public class SystemAnalyticsDto
{
    public int TotalClinics { get; set; }
    public int ActiveClinics { get; set; }
    public int InactiveClinics { get; set; }
    public int TotalUsers { get; set; }
    public int ActiveUsers { get; set; }
    public int TotalPatients { get; set; }
    public decimal MonthlyRecurringRevenue { get; set; }
    public object? SubscriptionsByStatus { get; set; }
    public object? SubscriptionsByPlan { get; set; }
}

public class ManualOverrideRequest
{
    public string Reason { get; set; } = string.Empty;
}

public class UpdateSubscriptionRequest
{
    public Guid NewPlanId { get; set; }
    public int? Status { get; set; }
}
