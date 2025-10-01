using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            builder.ToTable("Appointments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.DurationMinutes)
                .IsRequired();

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Notes)
                .HasMaxLength(1000);

            builder.Property(a => a.CancellationReason)
                .HasMaxLength(500);

            builder.Property(a => a.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Clinic)
                .WithMany()
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => new { a.TenantId, a.ScheduledDate, a.ClinicId })
                .HasDatabaseName("IX_Appointments_TenantId_Date_Clinic");

            builder.HasIndex(a => new { a.TenantId, a.PatientId })
                .HasDatabaseName("IX_Appointments_TenantId_Patient");

            builder.HasIndex(a => new { a.TenantId, a.Status })
                .HasDatabaseName("IX_Appointments_TenantId_Status");

            builder.HasIndex(a => a.TenantId)
                .HasDatabaseName("IX_Appointments_TenantId");
        }
    }
}