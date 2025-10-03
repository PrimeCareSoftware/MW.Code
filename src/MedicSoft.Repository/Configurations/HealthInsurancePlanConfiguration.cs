using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class HealthInsurancePlanConfiguration : IEntityTypeConfiguration<HealthInsurancePlan>
    {
        public void Configure(EntityTypeBuilder<HealthInsurancePlan> builder)
        {
            builder.ToTable("HealthInsurancePlans");

            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .ValueGeneratedNever();

            builder.Property(h => h.InsuranceName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.PlanNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.PlanType)
                .HasMaxLength(100);

            builder.Property(h => h.HolderName)
                .HasMaxLength(200);

            builder.Property(h => h.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.ValidFrom)
                .IsRequired();

            builder.Property(h => h.ValidUntil);

            builder.Property(h => h.IsActive)
                .IsRequired();

            // Relationship with Patient
            builder.HasOne(h => h.Patient)
                .WithMany(p => p.HealthInsurancePlans)
                .HasForeignKey(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(h => new { h.TenantId, h.PatientId })
                .HasDatabaseName("IX_HealthInsurancePlans_TenantId_PatientId");

            builder.HasIndex(h => h.TenantId)
                .HasDatabaseName("IX_HealthInsurancePlans_TenantId");

            builder.HasIndex(h => h.PlanNumber)
                .HasDatabaseName("IX_HealthInsurancePlans_PlanNumber");
        }
    }
}
