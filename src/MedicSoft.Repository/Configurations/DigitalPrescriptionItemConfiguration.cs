using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class DigitalPrescriptionItemConfiguration : IEntityTypeConfiguration<DigitalPrescriptionItem>
    {
        public void Configure(EntityTypeBuilder<DigitalPrescriptionItem> builder)
        {
            builder.ToTable("DigitalPrescriptionItems");

            builder.HasKey(dpi => dpi.Id);

            builder.Property(dpi => dpi.Id)
                .ValueGeneratedNever();

            builder.Property(dpi => dpi.DigitalPrescriptionId)
                .IsRequired();

            builder.Property(dpi => dpi.MedicationId)
                .IsRequired();

            builder.Property(dpi => dpi.MedicationName)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(dpi => dpi.GenericName)
                .HasMaxLength(300);

            builder.Property(dpi => dpi.ActiveIngredient)
                .HasMaxLength(300);

            builder.Property(dpi => dpi.IsControlledSubstance)
                .IsRequired();

            builder.Property(dpi => dpi.ControlledList)
                .HasConversion<int?>();

            builder.Property(dpi => dpi.AnvisaRegistration)
                .HasMaxLength(50);

            builder.Property(dpi => dpi.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dpi => dpi.PharmaceuticalForm)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dpi => dpi.Frequency)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(dpi => dpi.DurationDays)
                .IsRequired();

            builder.Property(dpi => dpi.Quantity)
                .IsRequired();

            builder.Property(dpi => dpi.AdministrationRoute)
                .HasMaxLength(100);

            builder.Property(dpi => dpi.Instructions)
                .HasMaxLength(500);

            builder.Property(dpi => dpi.BatchNumber)
                .HasMaxLength(50);

            builder.Property(dpi => dpi.ManufactureDate);

            builder.Property(dpi => dpi.ExpiryDate);

            builder.Property(dpi => dpi.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(dpi => dpi.CreatedAt)
                .IsRequired();

            builder.Property(dpi => dpi.UpdatedAt);

            // Relationships
            builder.HasOne(dpi => dpi.DigitalPrescription)
                .WithMany(dp => dp.Items)
                .HasForeignKey(dpi => dpi.DigitalPrescriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(dpi => dpi.Medication)
                .WithMany()
                .HasForeignKey(dpi => dpi.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(dpi => new { dpi.TenantId, dpi.DigitalPrescriptionId })
                .HasDatabaseName("IX_DigitalPrescriptionItems_TenantId_PrescriptionId");

            builder.HasIndex(dpi => dpi.MedicationId)
                .HasDatabaseName("IX_DigitalPrescriptionItems_MedicationId");

            builder.HasIndex(dpi => new { dpi.TenantId, dpi.IsControlledSubstance })
                .HasDatabaseName("IX_DigitalPrescriptionItems_TenantId_Controlled");
        }
    }
}
