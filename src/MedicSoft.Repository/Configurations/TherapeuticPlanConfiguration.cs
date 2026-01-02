using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TherapeuticPlanConfiguration : IEntityTypeConfiguration<TherapeuticPlan>
    {
        public void Configure(EntityTypeBuilder<TherapeuticPlan> builder)
        {
            builder.ToTable("TherapeuticPlans");

            builder.HasKey(tp => tp.Id);

            builder.Property(tp => tp.Id)
                .ValueGeneratedNever();

            builder.Property(tp => tp.Treatment)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(tp => tp.MedicationPrescription)
                .HasMaxLength(5000);

            builder.Property(tp => tp.ExamRequests)
                .HasMaxLength(3000);

            builder.Property(tp => tp.Referrals)
                .HasMaxLength(2000);

            builder.Property(tp => tp.PatientGuidance)
                .HasMaxLength(3000);

            builder.Property(tp => tp.ReturnDate);

            builder.Property(tp => tp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(tp => tp.MedicalRecord)
                .WithMany(mr => mr.Plans)
                .HasForeignKey(tp => tp.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(tp => new { tp.TenantId, tp.MedicalRecordId })
                .HasDatabaseName("IX_TherapeuticPlans_TenantId_MedicalRecord");

            builder.HasIndex(tp => new { tp.TenantId, tp.ReturnDate })
                .HasDatabaseName("IX_TherapeuticPlans_TenantId_ReturnDate");

            builder.HasIndex(tp => tp.TenantId)
                .HasDatabaseName("IX_TherapeuticPlans_TenantId");
        }
    }
}
