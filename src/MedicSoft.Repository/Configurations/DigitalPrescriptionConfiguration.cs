using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class DigitalPrescriptionConfiguration : IEntityTypeConfiguration<DigitalPrescription>
    {
        public void Configure(EntityTypeBuilder<DigitalPrescription> builder)
        {
            builder.ToTable("DigitalPrescriptions");

            builder.HasKey(dp => dp.Id);

            builder.Property(dp => dp.Id)
                .ValueGeneratedNever();

            builder.Property(dp => dp.MedicalRecordId)
                .IsRequired();

            builder.Property(dp => dp.PatientId)
                .IsRequired();

            builder.Property(dp => dp.DoctorId)
                .IsRequired();

            builder.Property(dp => dp.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(dp => dp.SequenceNumber)
                .HasMaxLength(50);

            builder.Property(dp => dp.IssuedAt)
                .IsRequired();

            builder.Property(dp => dp.ExpiresAt)
                .IsRequired();

            builder.Property(dp => dp.IsActive)
                .IsRequired();

            builder.Property(dp => dp.DoctorName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(dp => dp.DoctorCRM)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(dp => dp.DoctorCRMState)
                .IsRequired()
                .HasMaxLength(2);

            builder.Property(dp => dp.PatientName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(dp => dp.PatientDocument)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(dp => dp.DigitalSignature)
                .HasMaxLength(2000);

            builder.Property(dp => dp.SignedAt);

            builder.Property(dp => dp.SignatureCertificate)
                .HasMaxLength(100);

            builder.Property(dp => dp.VerificationCode)
                .HasMaxLength(50);

            builder.Property(dp => dp.RequiresSNGPCReport)
                .IsRequired();

            builder.Property(dp => dp.ReportedToSNGPCAt);

            builder.Property(dp => dp.Notes)
                .HasMaxLength(1000);

            builder.Property(dp => dp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dp => dp.CreatedAt)
                .IsRequired();

            builder.Property(dp => dp.UpdatedAt);

            // Relationships
            builder.HasOne(dp => dp.MedicalRecord)
                .WithMany()
                .HasForeignKey(dp => dp.MedicalRecordId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dp => dp.Patient)
                .WithMany()
                .HasForeignKey(dp => dp.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(dp => dp.Doctor)
                .WithMany()
                .HasForeignKey(dp => dp.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(dp => new { dp.TenantId, dp.PatientId })
                .HasDatabaseName("IX_DigitalPrescriptions_TenantId_PatientId");

            builder.HasIndex(dp => new { dp.TenantId, dp.DoctorId })
                .HasDatabaseName("IX_DigitalPrescriptions_TenantId_DoctorId");

            builder.HasIndex(dp => new { dp.TenantId, dp.MedicalRecordId })
                .HasDatabaseName("IX_DigitalPrescriptions_TenantId_MedicalRecordId");

            builder.HasIndex(dp => new { dp.TenantId, dp.Type })
                .HasDatabaseName("IX_DigitalPrescriptions_TenantId_Type");

            builder.HasIndex(dp => dp.VerificationCode)
                .IsUnique()
                .HasDatabaseName("IX_DigitalPrescriptions_VerificationCode");

            builder.HasIndex(dp => new { dp.TenantId, dp.SequenceNumber })
                .IsUnique()
                .HasDatabaseName("IX_DigitalPrescriptions_TenantId_SequenceNumber")
                .HasFilter("\"SequenceNumber\" IS NOT NULL");

            builder.HasIndex(dp => new { dp.TenantId, dp.RequiresSNGPCReport, dp.ReportedToSNGPCAt })
                .HasDatabaseName("IX_DigitalPrescriptions_SNGPC_Reporting");

            builder.HasIndex(dp => new { dp.TenantId, dp.IsActive, dp.ExpiresAt })
                .HasDatabaseName("IX_DigitalPrescriptions_Active_Expiration");
        }
    }
}
