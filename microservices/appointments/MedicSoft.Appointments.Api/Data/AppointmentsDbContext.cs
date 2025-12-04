using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Appointments.Api.Data;

public class AppointmentsDbContext : DbContext
{
    public AppointmentsDbContext(DbContextOptions<AppointmentsDbContext> options) : base(options)
    {
    }

    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
    public DbSet<WaitingQueueEntryEntity> WaitingQueueEntries => Set<WaitingQueueEntryEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<AppointmentEntity>(entity =>
        {
            entity.ToTable("Appointments");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<WaitingQueueEntryEntity>(entity =>
        {
            entity.ToTable("WaitingQueueEntries");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });
    }
}

public class AppointmentEntity
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public int Status { get; set; } // 0=Scheduled, 1=Confirmed, 2=Arrived, 3=InProgress, 4=Completed, 5=Cancelled, 6=NoShow
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class WaitingQueueEntryEntity
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid PatientId { get; set; }
    public int Position { get; set; }
    public int Status { get; set; } // 0=Waiting, 1=Called, 2=InService, 3=Completed, 4=NoShow
    public DateTime CheckInTime { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? ServiceStartedAt { get; set; }
    public DateTime? ServiceEndedAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
