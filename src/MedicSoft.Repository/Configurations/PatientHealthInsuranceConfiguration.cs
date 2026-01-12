using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PatientHealthInsuranceConfiguration : IEntityTypeConfiguration<PatientHealthInsurance>
    {
        public void Configure(EntityTypeBuilder<PatientHealthInsurance> builder)
        {
            builder.ToTable("PatientHealthInsurances");

            builder.HasKey(phi => phi.Id);

            builder.Property(phi => phi.Id)
                .ValueGeneratedNever();

            builder.Property(phi => phi.CardNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(phi => phi.CardValidationCode)
                .HasMaxLength(10);

            builder.Property(phi => phi.HolderName)
                .HasMaxLength(200);

            builder.Property(phi => phi.HolderDocument)
                .HasMaxLength(20);

            builder.Property(phi => phi.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(phi => phi.Patient)
                .WithMany()
                .HasForeignKey(phi => phi.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(phi => phi.HealthInsurancePlan)
                .WithMany()
                .HasForeignKey(phi => phi.HealthInsurancePlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(phi => new { phi.TenantId, phi.CardNumber })
                .IsUnique()
                .HasDatabaseName("IX_PatientHealthInsurances_TenantId_CardNumber");

            builder.HasIndex(phi => new { phi.TenantId, phi.PatientId })
                .HasDatabaseName("IX_PatientHealthInsurances_TenantId_PatientId");

            builder.HasIndex(phi => phi.TenantId)
                .HasDatabaseName("IX_PatientHealthInsurances_TenantId");
        }
    }
}
