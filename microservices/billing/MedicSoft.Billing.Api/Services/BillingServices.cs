using Microsoft.EntityFrameworkCore;
using MedicSoft.Billing.Api.Data;
using MedicSoft.Billing.Api.Models;

namespace MedicSoft.Billing.Api.Services;

public interface ISubscriptionService
{
    Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();
    Task<ClinicSubscriptionDto?> GetClinicSubscriptionAsync(Guid clinicId, string tenantId);
}

public interface IPaymentService
{
    Task<IEnumerable<PaymentDto>> GetPaymentsByClinicAsync(Guid clinicId, string tenantId);
    Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, Guid clinicId, string tenantId);
}

public interface IExpenseService
{
    Task<IEnumerable<ExpenseDto>> GetExpensesByClinicAsync(Guid clinicId, string tenantId);
    Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, Guid clinicId, string tenantId);
}

public class SubscriptionService : ISubscriptionService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<SubscriptionService> _logger;

    public SubscriptionService(BillingDbContext context, ILogger<SubscriptionService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
    {
        var plans = await _context.SubscriptionPlans
            .Where(p => p.IsActive)
            .OrderBy(p => p.MonthlyPrice)
            .ToListAsync();

        return plans.Select(p => new SubscriptionPlanDto
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            MonthlyPrice = p.MonthlyPrice,
            YearlyPrice = p.YearlyPrice,
            MaxUsers = p.MaxUsers,
            MaxPatients = p.MaxPatients,
            HasAdvancedReports = p.HasAdvancedReports,
            HasTelemedicine = p.HasTelemedicine,
            IsActive = p.IsActive
        });
    }

    public async Task<ClinicSubscriptionDto?> GetClinicSubscriptionAsync(Guid clinicId, string tenantId)
    {
        var subscription = await _context.ClinicSubscriptions
            .Where(s => s.ClinicId == clinicId && s.TenantId == tenantId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();

        if (subscription == null)
            return null;

        var plan = await _context.SubscriptionPlans
            .FirstOrDefaultAsync(p => p.Id == subscription.SubscriptionPlanId);

        return new ClinicSubscriptionDto
        {
            Id = subscription.Id,
            ClinicId = subscription.ClinicId,
            SubscriptionPlanId = subscription.SubscriptionPlanId,
            PlanName = plan?.Name ?? "Unknown",
            Status = GetStatusName(subscription.Status),
            StartDate = subscription.StartDate,
            TrialEndDate = subscription.TrialEndDate,
            NextPaymentDate = subscription.NextPaymentDate,
            CurrentPrice = subscription.CurrentPrice,
            HasManualOverride = subscription.HasManualOverride
        };
    }

    private static string GetStatusName(int status)
    {
        return status switch
        {
            0 => "Trial",
            1 => "Active",
            2 => "PastDue",
            3 => "Cancelled",
            4 => "Suspended",
            _ => "Unknown"
        };
    }
}

public class PaymentService : IPaymentService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(BillingDbContext context, ILogger<PaymentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<PaymentDto>> GetPaymentsByClinicAsync(Guid clinicId, string tenantId)
    {
        var payments = await _context.Payments
            .Where(p => p.TenantId == tenantId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return payments.Select(p => new PaymentDto
        {
            Id = p.Id,
            InvoiceId = p.InvoiceId,
            PatientId = p.PatientId,
            AppointmentId = p.AppointmentId,
            Amount = p.Amount,
            PaymentMethod = GetPaymentMethodName(p.PaymentMethod),
            Status = GetPaymentStatusName(p.Status),
            TransactionId = p.TransactionId,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt
        });
    }

    public async Task<PaymentDto> CreatePaymentAsync(CreatePaymentDto dto, Guid clinicId, string tenantId)
    {
        var payment = new PaymentEntity
        {
            Id = Guid.NewGuid(),
            InvoiceId = dto.InvoiceId,
            PatientId = dto.PatientId,
            AppointmentId = dto.AppointmentId,
            Amount = dto.Amount,
            PaymentMethod = dto.PaymentMethod,
            Status = 1, // Completed
            Notes = dto.Notes,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Payments.Add(payment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created payment: {PaymentId} for amount: {Amount}", payment.Id, payment.Amount);

        return new PaymentDto
        {
            Id = payment.Id,
            InvoiceId = payment.InvoiceId,
            PatientId = payment.PatientId,
            AppointmentId = payment.AppointmentId,
            Amount = payment.Amount,
            PaymentMethod = GetPaymentMethodName(payment.PaymentMethod),
            Status = GetPaymentStatusName(payment.Status),
            TransactionId = payment.TransactionId,
            Notes = payment.Notes,
            CreatedAt = payment.CreatedAt
        };
    }

    private static string GetPaymentMethodName(int method)
    {
        return method switch
        {
            0 => "Cash",
            1 => "CreditCard",
            2 => "DebitCard",
            3 => "PIX",
            4 => "BankTransfer",
            _ => "Unknown"
        };
    }

    private static string GetPaymentStatusName(int status)
    {
        return status switch
        {
            0 => "Pending",
            1 => "Completed",
            2 => "Failed",
            3 => "Refunded",
            _ => "Unknown"
        };
    }
}

public class ExpenseService : IExpenseService
{
    private readonly BillingDbContext _context;
    private readonly ILogger<ExpenseService> _logger;

    public ExpenseService(BillingDbContext context, ILogger<ExpenseService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<ExpenseDto>> GetExpensesByClinicAsync(Guid clinicId, string tenantId)
    {
        var expenses = await _context.Expenses
            .Where(e => e.ClinicId == clinicId && e.TenantId == tenantId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();

        return expenses.Select(e => new ExpenseDto
        {
            Id = e.Id,
            Description = e.Description,
            Amount = e.Amount,
            Category = GetCategoryName(e.Category),
            ExpenseDate = e.ExpenseDate,
            Vendor = e.Vendor,
            InvoiceNumber = e.InvoiceNumber,
            IsPaid = e.IsPaid,
            PaidAt = e.PaidAt,
            CreatedAt = e.CreatedAt
        });
    }

    public async Task<ExpenseDto> CreateExpenseAsync(CreateExpenseDto dto, Guid clinicId, string tenantId)
    {
        var expense = new ExpenseEntity
        {
            Id = Guid.NewGuid(),
            ClinicId = clinicId,
            Description = dto.Description,
            Amount = dto.Amount,
            Category = dto.Category,
            ExpenseDate = dto.ExpenseDate,
            Vendor = dto.Vendor,
            InvoiceNumber = dto.InvoiceNumber,
            IsPaid = false,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Expenses.Add(expense);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created expense: {ExpenseId} for amount: {Amount}", expense.Id, expense.Amount);

        return new ExpenseDto
        {
            Id = expense.Id,
            Description = expense.Description,
            Amount = expense.Amount,
            Category = GetCategoryName(expense.Category),
            ExpenseDate = expense.ExpenseDate,
            Vendor = expense.Vendor,
            InvoiceNumber = expense.InvoiceNumber,
            IsPaid = expense.IsPaid,
            PaidAt = expense.PaidAt,
            CreatedAt = expense.CreatedAt
        };
    }

    private static string GetCategoryName(int category)
    {
        return category switch
        {
            0 => "Supplies",
            1 => "Utilities",
            2 => "Rent",
            3 => "Salaries",
            4 => "Equipment",
            5 => "Other",
            _ => "Unknown"
        };
    }
}
