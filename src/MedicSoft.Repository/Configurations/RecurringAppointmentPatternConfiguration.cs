using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class RecurringAppointmentPatternConfiguration : IEntityTypeConfiguration<RecurringAppointmentPattern>
    {
        public void Configure(EntityTypeBuilder<RecurringAppointmentPattern> builder)
        {
            builder.ToTable("RecurringAppointmentPatterns");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Frequency)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(r => r.Interval)
                .IsRequired()
                .HasDefaultValue(1);

            builder.Property(r => r.DaysOfWeek)
                .IsRequired(false)
                .HasConversion<int>();

            builder.Property(r => r.DayOfMonth)
                .IsRequired(false);

            builder.Property(r => r.StartDate)
                .IsRequired();

            builder.Property(r => r.EndDate)
                .IsRequired(false);

            builder.Property(r => r.EffectiveEndDate)
                .IsRequired(false);

            builder.Property(r => r.OccurrencesCount)
                .IsRequired(false);

            builder.Property(r => r.StartTime)
                .IsRequired();

            builder.Property(r => r.EndTime)
                .IsRequired();

            builder.Property(r => r.DurationMinutes)
                .IsRequired(false);

            builder.Property(r => r.AppointmentType)
                .IsRequired(false)
                .HasConversion<int>();

            builder.Property(r => r.BlockedSlotType)
                .IsRequired(false)
                .HasConversion<int>();

            builder.Property(r => r.Notes)
                .HasMaxLength(1000);

            builder.Property(r => r.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(r => r.ParentPatternId)
                .IsRequired(false);

            builder.Property(r => r.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(r => r.Clinic)
                .WithMany()
                .HasForeignKey(r => r.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Professional)
                .WithMany()
                .HasForeignKey(r => r.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Self-referencing relationship for pattern splits
            builder.HasOne(r => r.ParentPattern)
                .WithMany()
                .HasForeignKey(r => r.ParentPatternId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(r => new { r.TenantId, r.ClinicId, r.IsActive })
                .HasDatabaseName("IX_RecurringAppointmentPatterns_TenantId_Clinic_Active");

            builder.HasIndex(r => new { r.TenantId, r.ProfessionalId, r.IsActive })
                .HasDatabaseName("IX_RecurringAppointmentPatterns_TenantId_Professional_Active");

            builder.HasIndex(r => new { r.TenantId, r.PatientId, r.IsActive })
                .HasDatabaseName("IX_RecurringAppointmentPatterns_TenantId_Patient_Active");

            builder.HasIndex(r => new { r.TenantId, r.ParentPatternId })
                .HasDatabaseName("IX_RecurringAppointmentPatterns_TenantId_ParentPatternId");

            builder.HasIndex(r => r.TenantId)
                .HasDatabaseName("IX_RecurringAppointmentPatterns_TenantId");
        }
    }
}
