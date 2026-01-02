using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class InformedConsentConfiguration : IEntityTypeConfiguration<InformedConsent>
    {
        public void Configure(EntityTypeBuilder<InformedConsent> builder)
        {
            builder.ToTable("InformedConsents");

            builder.HasKey(ic => ic.Id);

            builder.Property(ic => ic.Id)
                .ValueGeneratedNever();

            builder.Property(ic => ic.ConsentText)
                .IsRequired()
                .HasMaxLength(10000);

            builder.Property(ic => ic.IsAccepted)
                .IsRequired();

            builder.Property(ic => ic.AcceptedAt);

            builder.Property(ic => ic.IPAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(ic => ic.DigitalSignature)
                .HasMaxLength(500);

            builder.Property(ic => ic.RegisteredByUserId);

            builder.Property(ic => ic.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(ic => ic.MedicalRecord)
                .WithMany(mr => mr.Consents)
                .HasForeignKey(ic => ic.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ic => ic.Patient)
                .WithMany()
                .HasForeignKey(ic => ic.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ic => new { ic.TenantId, ic.MedicalRecordId })
                .HasDatabaseName("IX_InformedConsents_TenantId_MedicalRecord");

            builder.HasIndex(ic => new { ic.TenantId, ic.PatientId })
                .HasDatabaseName("IX_InformedConsents_TenantId_Patient");

            builder.HasIndex(ic => new { ic.TenantId, ic.IsAccepted })
                .HasDatabaseName("IX_InformedConsents_TenantId_IsAccepted");

            builder.HasIndex(ic => ic.TenantId)
                .HasDatabaseName("IX_InformedConsents_TenantId");
        }
    }
}
