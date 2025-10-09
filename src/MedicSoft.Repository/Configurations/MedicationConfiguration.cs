using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MedicationConfiguration : IEntityTypeConfiguration<Medication>
    {
        public void Configure(EntityTypeBuilder<Medication> builder)
        {
            builder.ToTable("Medications");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(m => m.GenericName)
                .HasMaxLength(300);

            builder.Property(m => m.Manufacturer)
                .HasMaxLength(200);

            builder.Property(m => m.ActiveIngredient)
                .HasMaxLength(300);

            builder.Property(m => m.Dosage)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.PharmaceuticalForm)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Concentration)
                .HasMaxLength(100);

            builder.Property(m => m.AdministrationRoute)
                .HasMaxLength(100);

            builder.Property(m => m.Category)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(m => m.RequiresPrescription)
                .IsRequired();

            builder.Property(m => m.IsControlled)
                .IsRequired();

            builder.Property(m => m.AnvisaRegistration)
                .HasMaxLength(50);

            builder.Property(m => m.Barcode)
                .HasMaxLength(50);

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.IsActive)
                .IsRequired();

            builder.Property(m => m.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.UpdatedAt);

            // Indexes
            builder.HasIndex(m => new { m.TenantId, m.Name })
                .HasDatabaseName("IX_Medications_TenantId_Name");

            builder.HasIndex(m => m.TenantId)
                .HasDatabaseName("IX_Medications_TenantId");

            builder.HasIndex(m => m.Category)
                .HasDatabaseName("IX_Medications_Category");

            builder.HasIndex(m => m.IsActive)
                .HasDatabaseName("IX_Medications_IsActive");
        }
    }
}
