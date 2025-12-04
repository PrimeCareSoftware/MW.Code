using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Billing.Api.Data;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options) : base(options)
    {
    }

    public DbSet<SubscriptionPlanEntity> SubscriptionPlans => Set<SubscriptionPlanEntity>();
    public DbSet<ClinicSubscriptionEntity> ClinicSubscriptions => Set<ClinicSubscriptionEntity>();
    public DbSet<InvoiceEntity> Invoices => Set<InvoiceEntity>();
    public DbSet<PaymentEntity> Payments => Set<PaymentEntity>();
    public DbSet<ExpenseEntity> Expenses => Set<ExpenseEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SubscriptionPlanEntity>(entity =>
        {
            entity.ToTable("SubscriptionPlans");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ClinicSubscriptionEntity>(entity =>
        {
            entity.ToTable("ClinicSubscriptions");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<InvoiceEntity>(entity =>
        {
            entity.ToTable("Invoices");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PaymentEntity>(entity =>
        {
            entity.ToTable("Payments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ExpenseEntity>(entity =>
        {
            entity.ToTable("Expenses");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });
    }
}

public class SubscriptionPlanEntity
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ClinicSubscriptionEntity
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public int Status { get; set; } // 0=Trial, 1=Active, 2=PastDue, 3=Cancelled, 4=Suspended
    public DateTime StartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool HasManualOverride { get; set; }
    public string? ManualOverrideReason { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class InvoiceEntity
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Status { get; set; } // 0=Pending, 1=Paid, 2=Overdue, 3=Cancelled
    public DateTime DueDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public string? Description { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PaymentEntity
{
    public Guid Id { get; set; }
    public Guid? InvoiceId { get; set; }
    public Guid? PatientId { get; set; }
    public Guid? AppointmentId { get; set; }
    public decimal Amount { get; set; }
    public int PaymentMethod { get; set; } // 0=Cash, 1=CreditCard, 2=DebitCard, 3=PIX, 4=BankTransfer
    public int Status { get; set; } // 0=Pending, 1=Completed, 2=Failed, 3=Refunded
    public string? TransactionId { get; set; }
    public string? Notes { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class ExpenseEntity
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int Category { get; set; } // 0=Supplies, 1=Utilities, 2=Rent, 3=Salaries, 4=Equipment, 5=Other
    public DateTime ExpenseDate { get; set; }
    public string? Vendor { get; set; }
    public string? InvoiceNumber { get; set; }
    public bool IsPaid { get; set; }
    public DateTime? PaidAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
