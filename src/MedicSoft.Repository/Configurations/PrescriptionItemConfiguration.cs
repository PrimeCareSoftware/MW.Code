using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PrescriptionItemConfiguration : IEntityTypeConfiguration<PrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<PrescriptionItem> builder)
        {
            builder.ToTable("PrescriptionItems");

            builder.HasKey(pi => pi.Id);

            builder.Property(pi => pi.Id)
                .ValueGeneratedNever();

            builder.Property(pi => pi.MedicalRecordId)
                .IsRequired();

            builder.Property(pi => pi.MedicationId)
                .IsRequired();

            builder.Property(pi => pi.Dosage)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pi => pi.Frequency)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(pi => pi.DurationDays)
                .IsRequired();

            builder.Property(pi => pi.Quantity)
                .IsRequired();

            builder.Property(pi => pi.Instructions)
                .HasMaxLength(500);

            builder.Property(pi => pi.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pi => pi.CreatedAt)
                .IsRequired();

            builder.Property(pi => pi.UpdatedAt);

            // Relationships
            builder.HasOne(pi => pi.MedicalRecord)
                .WithMany(mr => mr.PrescriptionItems)
                .HasForeignKey(pi => pi.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pi => pi.Medication)
                .WithMany()
                .HasForeignKey(pi => pi.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(pi => new { pi.TenantId, pi.MedicalRecordId })
                .HasDatabaseName("IX_PrescriptionItems_TenantId_MedicalRecordId");

            builder.HasIndex(pi => pi.MedicationId)
                .HasDatabaseName("IX_PrescriptionItems_MedicationId");
        }
    }
}
