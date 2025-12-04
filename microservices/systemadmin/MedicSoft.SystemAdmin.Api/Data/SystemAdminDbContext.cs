using Microsoft.EntityFrameworkCore;

namespace MedicSoft.SystemAdmin.Api.Data;

public class SystemAdminDbContext : DbContext
{
    public SystemAdminDbContext(DbContextOptions<SystemAdminDbContext> options) : base(options)
    {
    }

    public DbSet<ClinicEntity> Clinics => Set<ClinicEntity>();
    public DbSet<OwnerEntity> Owners => Set<OwnerEntity>();
    public DbSet<UserEntity> Users => Set<UserEntity>();
    public DbSet<SubscriptionPlanEntity> SubscriptionPlans => Set<SubscriptionPlanEntity>();
    public DbSet<ClinicSubscriptionEntity> ClinicSubscriptions => Set<ClinicSubscriptionEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ClinicEntity>(entity =>
        {
            entity.ToTable("Clinics");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<OwnerEntity>(entity =>
        {
            entity.ToTable("Owners");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<UserEntity>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Username).IsRequired().HasMaxLength(100);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

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
    }
}

public class ClinicEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? TradeName { get; set; }
    public string Document { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public TimeSpan WorkStartTime { get; set; }
    public TimeSpan WorkEndTime { get; set; }
    public int DefaultAppointmentDuration { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class OwnerEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public Guid? ClinicId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class UserEntity
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public int Role { get; set; }
    public bool IsActive { get; set; } = true;
    public Guid? ClinicId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
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
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
}

public class ClinicSubscriptionEntity
{
    public Guid Id { get; set; }
    public Guid ClinicId { get; set; }
    public Guid SubscriptionPlanId { get; set; }
    public int Status { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? NextPaymentDate { get; set; }
    public decimal CurrentPrice { get; set; }
    public bool HasManualOverride { get; set; }
    public string? ManualOverrideReason { get; set; }
    public string? ManualOverrideSetBy { get; set; }
    public DateTime? ManualOverrideSetAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
