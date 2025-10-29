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
    }
}
