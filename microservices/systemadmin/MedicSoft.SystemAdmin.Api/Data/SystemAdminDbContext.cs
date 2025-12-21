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
    public DbSet<SubdomainEntity> Subdomains => Set<SubdomainEntity>();
    public DbSet<TicketEntity> Tickets => Set<TicketEntity>();
    public DbSet<TicketCommentEntity> TicketComments => Set<TicketCommentEntity>();
    public DbSet<TicketAttachmentEntity> TicketAttachments => Set<TicketAttachmentEntity>();
    public DbSet<TicketHistoryEntity> TicketHistory => Set<TicketHistoryEntity>();

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

        modelBuilder.Entity<SubdomainEntity>(entity =>
        {
            entity.ToTable("Subdomains");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Subdomain).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => e.Subdomain).IsUnique();
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<TicketEntity>(entity =>
        {
            entity.ToTable("Tickets");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
            entity.HasIndex(e => new { e.TenantId, e.UserId });
            entity.HasIndex(e => new { e.TenantId, e.ClinicId });
            entity.HasIndex(e => new { e.Status, e.TenantId });
        });

        modelBuilder.Entity<TicketCommentEntity>(entity =>
        {
            entity.ToTable("TicketComments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Comment).IsRequired();
            entity.HasIndex(e => e.TicketId);
        });

        modelBuilder.Entity<TicketAttachmentEntity>(entity =>
        {
            entity.ToTable("TicketAttachments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(500);
            entity.HasIndex(e => e.TicketId);
        });

        modelBuilder.Entity<TicketHistoryEntity>(entity =>
        {
            entity.ToTable("TicketHistory");
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TicketId);
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
    public int TrialDays { get; set; } = 14;
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
    public DateTime? ManualOverrideExpiresAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class SubdomainEntity
{
    public Guid Id { get; set; }
    public string Subdomain { get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class TicketEntity
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Type { get; set; } // TicketType enum
    public int Status { get; set; } // TicketStatus enum
    public int Priority { get; set; } // TicketPriority enum
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public Guid? ClinicId { get; set; }
    public string? ClinicName { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? LastStatusChangeAt { get; set; }
}

public class TicketCommentEntity
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string Comment { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public bool IsInternal { get; set; }
    public bool IsSystemOwner { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class TicketAttachmentEntity
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime UploadedAt { get; set; }
}

public class TicketHistoryEntity
{
    public Guid Id { get; set; }
    public Guid TicketId { get; set; }
    public int OldStatus { get; set; }
    public int NewStatus { get; set; }
    public Guid ChangedById { get; set; }
    public string ChangedByName { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime ChangedAt { get; set; }
}
