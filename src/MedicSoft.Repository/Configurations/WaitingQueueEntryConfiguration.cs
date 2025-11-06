using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class WaitingQueueEntryConfiguration : IEntityTypeConfiguration<WaitingQueueEntry>
    {
        public void Configure(EntityTypeBuilder<WaitingQueueEntry> builder)
        {
            builder.ToTable("WaitingQueueEntries");

            builder.HasKey(wqe => wqe.Id);

            builder.Property(wqe => wqe.Id)
                .ValueGeneratedNever();

            builder.Property(wqe => wqe.Priority)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(wqe => wqe.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(wqe => wqe.Position)
                .IsRequired();

            builder.Property(wqe => wqe.EstimatedWaitTimeMinutes)
                .IsRequired();

            builder.Property(wqe => wqe.TriageNotes)
                .HasMaxLength(1000);

            builder.Property(wqe => wqe.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(wqe => wqe.Appointment)
                .WithMany()
                .HasForeignKey(wqe => wqe.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wqe => wqe.Patient)
                .WithMany()
                .HasForeignKey(wqe => wqe.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(wqe => wqe.Clinic)
                .WithMany()
                .HasForeignKey(wqe => wqe.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(wqe => new { wqe.TenantId, wqe.ClinicId, wqe.Status })
                .HasDatabaseName("IX_WaitingQueueEntries_TenantId_Clinic_Status");

            builder.HasIndex(wqe => new { wqe.TenantId, wqe.CheckInTime })
                .HasDatabaseName("IX_WaitingQueueEntries_TenantId_CheckInTime");

            builder.HasIndex(wqe => new { wqe.TenantId, wqe.Position })
                .HasDatabaseName("IX_WaitingQueueEntries_TenantId_Position");

            builder.HasIndex(wqe => wqe.AppointmentId)
                .HasDatabaseName("IX_WaitingQueueEntries_AppointmentId")
                .IsUnique();

            builder.HasIndex(wqe => wqe.TenantId)
                .HasDatabaseName("IX_WaitingQueueEntries_TenantId");
        }
    }
}
