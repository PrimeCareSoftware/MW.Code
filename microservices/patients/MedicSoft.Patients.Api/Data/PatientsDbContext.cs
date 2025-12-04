using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Patients.Api.Data;

public class PatientsDbContext : DbContext
{
    public PatientsDbContext(DbContextOptions<PatientsDbContext> options) : base(options)
    {
    }

    public DbSet<PatientEntity> Patients => Set<PatientEntity>();
    public DbSet<PatientClinicLinkEntity> PatientClinicLinks => Set<PatientClinicLinkEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PatientEntity>(entity =>
        {
            entity.ToTable("Patients");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Document).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });

        modelBuilder.Entity<PatientClinicLinkEntity>(entity =>
        {
            entity.ToTable("PatientClinicLinks");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TenantId).IsRequired().HasMaxLength(100);
        });
    }
}

public class PatientEntity
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Document { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateOfBirth { get; set; }
    public int Gender { get; set; }
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public Guid? GuardianId { get; set; }
    public bool IsActive { get; set; } = true;
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class PatientClinicLinkEntity
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid ClinicId { get; set; }
    public string TenantId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
