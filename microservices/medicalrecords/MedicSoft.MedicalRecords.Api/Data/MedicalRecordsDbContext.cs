using Microsoft.EntityFrameworkCore;

namespace MedicSoft.MedicalRecords.Api.Data;

public class MedicalRecordsDbContext : DbContext
{
    public MedicalRecordsDbContext(DbContextOptions<MedicalRecordsDbContext> options) : base(options)
    {
    }

    public DbSet<MedicalRecordEntity> MedicalRecords => Set<MedicalRecordEntity>();
    public DbSet<MedicationEntity> Medications => Set<MedicationEntity>();
    public DbSet<PrescriptionItemEntity> PrescriptionItems => Set<PrescriptionItemEntity>();
    public DbSet<ExamRequestEntity> ExamRequests => Set<ExamRequestEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<MedicalRecordEntity>(entity =>
        {
            entity.ToTable("MedicalRecords");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<MedicationEntity>(entity =>
        {
            entity.ToTable("Medications");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PrescriptionItemEntity>(entity =>
        {
            entity.ToTable("PrescriptionItems");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<ExamRequestEntity>(entity =>
        {
            entity.ToTable("ExamRequests");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });
    }
}

public class MedicalRecordEntity
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public Guid? DoctorId { get; set; }
    public string ChiefComplaint { get; set; } = string.Empty;
    public string? HistoryOfPresentIllness { get; set; }
    public string? PhysicalExamination { get; set; }
    public string? Diagnosis { get; set; }
    public string? TreatmentPlan { get; set; }
    public string? Notes { get; set; }
    public string? VitalSigns { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class MedicationEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? GenericName { get; set; }
    public string? Manufacturer { get; set; }
    public string? ActiveIngredient { get; set; }
    public string Dosage { get; set; } = string.Empty;
    public string PharmaceuticalForm { get; set; } = string.Empty;
    public string? Concentration { get; set; }
    public string? AdministrationRoute { get; set; }
    public int Category { get; set; }
    public bool RequiresPrescription { get; set; }
    public bool IsControlled { get; set; }
    public string? AnvisaRegistration { get; set; }
    public string? Barcode { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PrescriptionItemEntity
{
    public Guid Id { get; set; }
    public Guid MedicalRecordId { get; set; }
    public Guid MedicationId { get; set; }
    public string MedicationName { get; set; } = string.Empty;
    public string Dosage { get; set; } = string.Empty;
    public string Frequency { get; set; } = string.Empty;
    public int DurationDays { get; set; }
    public string? Instructions { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class ExamRequestEntity
{
    public Guid Id { get; set; }
    public Guid MedicalRecordId { get; set; }
    public Guid? AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string? ExamCode { get; set; }
    public string? Instructions { get; set; }
    public string? ClinicalIndication { get; set; }
    public int Status { get; set; } // 0=Requested, 1=Scheduled, 2=Completed, 3=Cancelled
    public DateTime? ResultDate { get; set; }
    public string? ResultNotes { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
