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

            // NEW TISS Phase 1 fields
            builder.Property(h => h.PlanName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(h => h.PlanCode)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.RegisterNumber)
                .HasMaxLength(20);

            // LEGACY fields (deprecated but maintained for backward compatibility)
            builder.Property(h => h.InsuranceName)
                .HasMaxLength(200);

            builder.Property(h => h.PlanNumber)
                .HasMaxLength(100);

            builder.Property(h => h.OldPlanType)
                .HasColumnName("PlanType") // Keep same column name for backward compatibility
                .HasMaxLength(100);

            builder.Property(h => h.HolderName)
                .HasMaxLength(200);

            builder.Property(h => h.PatientId)
                .IsRequired(false); // Nullable for TISS plans

            builder.Property(h => h.ValidFrom);

            builder.Property(h => h.ValidUntil);

            builder.Property(h => h.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.IsActive)
                .IsRequired();

            // Relationship with Operator (NEW)
            builder.HasOne(h => h.Operator)
                .WithMany()
                .HasForeignKey(h => h.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with Patient (LEGACY - maintained for backward compatibility)
            builder.HasOne(h => h.Patient)
                .WithMany(p => p.HealthInsurancePlans)
                .HasForeignKey(h => h.PatientId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false); // Optional relationship

            // Indexes
            builder.HasIndex(h => new { h.TenantId, h.PatientId })
                .HasDatabaseName("IX_HealthInsurancePlans_TenantId_PatientId");

            builder.HasIndex(h => h.TenantId)
                .HasDatabaseName("IX_HealthInsurancePlans_TenantId");

            builder.HasIndex(h => h.PlanNumber)
                .HasDatabaseName("IX_HealthInsurancePlans_PlanNumber");
                
            builder.HasIndex(h => h.PlanCode)
                .HasDatabaseName("IX_HealthInsurancePlans_PlanCode");
                
            builder.HasIndex(h => h.OperatorId)
                .HasDatabaseName("IX_HealthInsurancePlans_OperatorId");
        }
    }
}
