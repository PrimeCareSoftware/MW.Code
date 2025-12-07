namespace MedicSoft.Billing.Api.Models;

public class SubscriptionPlanDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal MonthlyPrice { get; set; }
    public decimal YearlyPrice { get; set; }
    public int MaxUsers { get; set; }
    public int MaxPatients { get; set; }
    public bool HasAdvancedReports { get; set; }
    public bool HasTelemedicine { get; set; }
    public bool IsActive { get; set; }
}

public class ClinicSubscriptionDto
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool HasManualOverride { get; set; }
}

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreatePaymentDto
{
    public Guid? InvoiceId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; }
    public string? Notes { get; set; }
}

public class ExpenseDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime ExpenseDate { get; set; }
    public string? Vendor { get; set; }
    public string? InvoiceNumber { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateExpenseDto
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Category { get; set; }
    public DateTime ExpenseDate { get; set; }
    public string? Vendor { get; set; }
    public string? InvoiceNumber { get; set; }
}
