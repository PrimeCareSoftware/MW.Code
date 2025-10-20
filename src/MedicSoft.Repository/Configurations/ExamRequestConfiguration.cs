using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ExamRequestConfiguration : IEntityTypeConfiguration<ExamRequest>
    {
        public void Configure(EntityTypeBuilder<ExamRequest> builder)
        {
            builder.ToTable("ExamRequests");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.AppointmentId)
                .IsRequired();

            builder.Property(e => e.PatientId)
                .IsRequired();

            builder.Property(e => e.ExamType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.ExamName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(e => e.Urgency)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(e => e.RequestedDate)
                .IsRequired();

            builder.Property(e => e.ScheduledDate);

            builder.Property(e => e.CompletedDate);

            builder.Property(e => e.Results)
                .HasMaxLength(5000);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt);

            // Navigation properties
            builder.HasOne(e => e.Appointment)
                .WithMany()
                .HasForeignKey(e => e.AppointmentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => e.AppointmentId)
                .HasDatabaseName("IX_ExamRequests_AppointmentId");

            builder.HasIndex(e => e.PatientId)
                .HasDatabaseName("IX_ExamRequests_PatientId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_ExamRequests_Status");

            builder.HasIndex(e => new { e.Status, e.Urgency })
                .HasDatabaseName("IX_ExamRequests_Status_Urgency");
        }
    }
}
