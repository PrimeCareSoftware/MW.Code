using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SoapRecordConfiguration : IEntityTypeConfiguration<SoapRecord>
    {
        public void Configure(EntityTypeBuilder<SoapRecord> builder)
        {
            builder.ToTable("SoapRecords");

            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Id)
                .ValueGeneratedNever();

            builder.Property(sr => sr.AppointmentId)
                .IsRequired();

            builder.Property(sr => sr.PatientId)
                .IsRequired();

            builder.Property(sr => sr.DoctorId)
                .IsRequired();

            builder.Property(sr => sr.RecordDate)
                .IsRequired();

            builder.Property(sr => sr.IsComplete)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(sr => sr.CompletionDate);

            builder.Property(sr => sr.IsLocked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(sr => sr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Configure owned types for value objects with JSON storage
            builder.OwnsOne(sr => sr.Subjective, subjective =>
            {
                subjective.ToJson();
                subjective.Property(s => s.ChiefComplaint).IsRequired();
                subjective.Property(s => s.HistoryOfPresentIllness).IsRequired();
            });

            builder.OwnsOne(sr => sr.Objective, objective =>
            {
                objective.ToJson();
                objective.OwnsOne(o => o.VitalSigns);
                objective.OwnsOne(o => o.PhysicalExam);
            });

            builder.OwnsOne(sr => sr.Assessment, assessment =>
            {
                assessment.ToJson();
                assessment.OwnsMany(a => a.DifferentialDiagnoses);
            });

            builder.OwnsOne(sr => sr.Plan, plan =>
            {
                plan.ToJson();
                plan.OwnsMany(p => p.Prescriptions);
                plan.OwnsMany(p => p.ExamRequests);
                plan.OwnsMany(p => p.Procedures);
                plan.OwnsMany(p => p.Referrals);
            });

            // Relationships
            builder.HasOne(sr => sr.Patient)
                .WithMany()
                .HasForeignKey(sr => sr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Appointment)
                .WithMany()
                .HasForeignKey(sr => sr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sr => sr.Doctor)
                .WithMany()
                .HasForeignKey(sr => sr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sr => new { sr.TenantId, sr.AppointmentId })
                .HasDatabaseName("IX_SoapRecords_TenantId_Appointment")
                .IsUnique();

            builder.HasIndex(sr => new { sr.TenantId, sr.PatientId })
                .HasDatabaseName("IX_SoapRecords_TenantId_Patient");

            builder.HasIndex(sr => new { sr.TenantId, sr.DoctorId })
                .HasDatabaseName("IX_SoapRecords_TenantId_Doctor");

            builder.HasIndex(sr => new { sr.TenantId, sr.IsComplete })
                .HasDatabaseName("IX_SoapRecords_TenantId_IsComplete");

            builder.HasIndex(sr => new { sr.TenantId, sr.IsLocked })
                .HasDatabaseName("IX_SoapRecords_TenantId_IsLocked");

            builder.HasIndex(sr => sr.TenantId)
                .HasDatabaseName("IX_SoapRecords_TenantId");
        }
    }
}
