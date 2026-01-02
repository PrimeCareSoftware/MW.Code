using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class DiagnosticHypothesisConfiguration : IEntityTypeConfiguration<DiagnosticHypothesis>
    {
        public void Configure(EntityTypeBuilder<DiagnosticHypothesis> builder)
        {
            builder.ToTable("DiagnosticHypotheses");

            builder.HasKey(dh => dh.Id);

            builder.Property(dh => dh.Id)
                .ValueGeneratedNever();

            builder.Property(dh => dh.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(dh => dh.ICD10Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(dh => dh.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(dh => dh.DiagnosedAt)
                .IsRequired();

            builder.Property(dh => dh.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(dh => dh.MedicalRecord)
                .WithMany(mr => mr.Diagnoses)
                .HasForeignKey(dh => dh.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(dh => new { dh.TenantId, dh.MedicalRecordId })
                .HasDatabaseName("IX_DiagnosticHypotheses_TenantId_MedicalRecord");

            builder.HasIndex(dh => new { dh.TenantId, dh.ICD10Code })
                .HasDatabaseName("IX_DiagnosticHypotheses_TenantId_ICD10Code");

            builder.HasIndex(dh => dh.TenantId)
                .HasDatabaseName("IX_DiagnosticHypotheses_TenantId");
        }
    }
}
