using Microsoft.EntityFrameworkCore;
using MedicSoft.Telemedicine.Domain.Entities;

namespace MedicSoft.Telemedicine.Infrastructure.Persistence;

/// <summary>
/// EF Core DbContext for telemedicine microservice
/// </summary>
public class TelemedicineDbContext : DbContext
{
    public TelemedicineDbContext(DbContextOptions<TelemedicineDbContext> options)
        : base(options)
    {
    }

    public DbSet<TelemedicineSession> Sessions { get; set; } = null!;
    public DbSet<TelemedicineConsent> Consents { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TelemedicineSession>(entity =>
        {
            entity.ToTable("TelemedicineSessions");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.RoomId)
                .IsRequired()
                .HasMaxLength(200);
                
            entity.Property(e => e.RoomUrl)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.RecordingUrl)
                .HasMaxLength(500);
                
            entity.Property(e => e.SessionNotes)
                .HasMaxLength(5000);
                
            entity.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();
                
            entity.Property(e => e.ConnectionQuality)
                .IsRequired()
                .HasConversion<string>();
                
            entity.Property(e => e.ConsentIpAddress)
                .HasMaxLength(50);
                
            entity.Property(e => e.FirstAppointmentJustification)
                .HasMaxLength(1000);
                
            // Value object mapping
            entity.OwnsOne(e => e.Duration, duration =>
            {
                duration.Property(d => d.StartTime)
                    .IsRequired();
                    
                duration.Property(d => d.EndTime);
            });
            
            // Indexes for common queries
            entity.HasIndex(e => new { e.TenantId, e.AppointmentId });
            entity.HasIndex(e => new { e.TenantId, e.ClinicId });
            entity.HasIndex(e => new { e.TenantId, e.ProviderId });
            entity.HasIndex(e => new { e.TenantId, e.PatientId });
            entity.HasIndex(e => new { e.TenantId, e.Status });
        });
        
        // Configure TelemedicineConsent entity
        modelBuilder.Entity<TelemedicineConsent>(entity =>
        {
            entity.ToTable("TelemedicineConsents");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);
                
            entity.Property(e => e.ConsentText)
                .IsRequired();
                
            entity.Property(e => e.IpAddress)
                .IsRequired()
                .HasMaxLength(50);
                
            entity.Property(e => e.UserAgent)
                .IsRequired()
                .HasMaxLength(500);
                
            entity.Property(e => e.DigitalSignature)
                .HasMaxLength(500);
                
            entity.Property(e => e.RevocationReason)
                .HasMaxLength(1000);
            
            // Indexes for common queries
            entity.HasIndex(e => new { e.TenantId, e.PatientId });
            entity.HasIndex(e => e.AppointmentId);
            entity.HasIndex(e => new { e.TenantId, e.PatientId, e.IsActive });
            entity.HasIndex(e => new { e.TenantId, e.ConsentDate });
        });
    }
}
