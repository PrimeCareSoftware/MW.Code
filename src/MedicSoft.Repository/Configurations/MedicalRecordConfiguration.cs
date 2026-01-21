using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MedicalRecordConfiguration : IEntityTypeConfiguration<MedicalRecord>
    {
        public void Configure(EntityTypeBuilder<MedicalRecord> builder)
        {
            builder.ToTable("MedicalRecords");

            builder.HasKey(mr => mr.Id);

            builder.Property(mr => mr.Id)
                .ValueGeneratedNever();

            // CFM 1.821 - Campos obrigatórios de anamnese
            builder.Property(mr => mr.ChiefComplaint)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(mr => mr.HistoryOfPresentIllness)
                .IsRequired()
                .HasMaxLength(5000);

            // CFM 1.821 - Campos recomendados de história clínica
            builder.Property(mr => mr.PastMedicalHistory)
                .HasMaxLength(3000);

            builder.Property(mr => mr.FamilyHistory)
                .HasMaxLength(2000);

            builder.Property(mr => mr.LifestyleHabits)
                .HasMaxLength(2000);

            builder.Property(mr => mr.CurrentMedications)
                .HasMaxLength(3000);

            // Campos legados (manter compatibilidade)
            builder.Property(mr => mr.Diagnosis)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(mr => mr.Prescription)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(mr => mr.Notes)
                .IsRequired()
                .HasMaxLength(3000);

            builder.Property(mr => mr.ConsultationDurationMinutes)
                .IsRequired();

            builder.Property(mr => mr.ConsultationStartTime)
                .IsRequired();

            // CFM 1.821 - Controle de fechamento
            builder.Property(mr => mr.IsClosed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(mr => mr.ClosedAt);

            builder.Property(mr => mr.ClosedByUserId);

            builder.Property(mr => mr.ProfessionalSignature)
                .HasMaxLength(500);

            builder.Property(mr => mr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(mr => mr.Patient)
                .WithMany()
                .HasForeignKey(mr => mr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(mr => mr.Appointment)
                .WithMany()
                .HasForeignKey(mr => mr.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(mr => new { mr.TenantId, mr.AppointmentId })
                .HasDatabaseName("IX_MedicalRecords_TenantId_Appointment")
                .IsUnique();

            builder.HasIndex(mr => new { mr.TenantId, mr.PatientId })
                .HasDatabaseName("IX_MedicalRecords_TenantId_Patient");

            builder.HasIndex(mr => new { mr.TenantId, mr.IsClosed })
                .HasDatabaseName("IX_MedicalRecords_TenantId_IsClosed");

            builder.HasIndex(mr => mr.TenantId)
                .HasDatabaseName("IX_MedicalRecords_TenantId");
        }
    }
}
