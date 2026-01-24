using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ControlledMedicationRegistryConfiguration : IEntityTypeConfiguration<ControlledMedicationRegistry>
    {
        public void Configure(EntityTypeBuilder<ControlledMedicationRegistry> builder)
        {
            builder.ToTable("ControlledMedicationRegistries");

            builder.HasKey(cmr => cmr.Id);

            builder.Property(cmr => cmr.Id)
                .ValueGeneratedNever();

            builder.Property(cmr => cmr.Date)
                .IsRequired();

            builder.Property(cmr => cmr.RegistryType)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(cmr => cmr.MedicationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cmr => cmr.ActiveIngredient)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cmr => cmr.AnvisaList)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(cmr => cmr.Concentration)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cmr => cmr.PharmaceuticalForm)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cmr => cmr.QuantityIn)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(cmr => cmr.QuantityOut)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(cmr => cmr.Balance)
                .IsRequired()
                .HasPrecision(18, 3);

            builder.Property(cmr => cmr.DocumentType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(cmr => cmr.DocumentNumber)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cmr => cmr.DocumentDate);

            builder.Property(cmr => cmr.PrescriptionId);

            builder.Property(cmr => cmr.PatientName)
                .HasMaxLength(200);

            builder.Property(cmr => cmr.PatientCPF)
                .HasMaxLength(14);

            builder.Property(cmr => cmr.DoctorName)
                .HasMaxLength(200);

            builder.Property(cmr => cmr.DoctorCRM)
                .HasMaxLength(20);

            builder.Property(cmr => cmr.SupplierName)
                .HasMaxLength(200);

            builder.Property(cmr => cmr.SupplierCNPJ)
                .HasMaxLength(18);

            builder.Property(cmr => cmr.RegisteredByUserId)
                .IsRequired();

            builder.Property(cmr => cmr.RegisteredAt)
                .IsRequired();

            builder.Property(cmr => cmr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cmr => cmr.CreatedAt)
                .IsRequired();

            builder.Property(cmr => cmr.UpdatedAt);

            // Relationships
            builder.HasOne(cmr => cmr.Prescription)
                .WithMany()
                .HasForeignKey(cmr => cmr.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cmr => cmr.RegisteredBy)
                .WithMany()
                .HasForeignKey(cmr => cmr.RegisteredByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(cmr => new { cmr.TenantId, cmr.Date })
                .HasDatabaseName("IX_ControlledMedicationRegistries_TenantId_Date");

            builder.HasIndex(cmr => new { cmr.TenantId, cmr.MedicationName })
                .HasDatabaseName("IX_ControlledMedicationRegistries_TenantId_MedicationName");

            builder.HasIndex(cmr => new { cmr.TenantId, cmr.AnvisaList })
                .HasDatabaseName("IX_ControlledMedicationRegistries_TenantId_AnvisaList");

            builder.HasIndex(cmr => new { cmr.TenantId, cmr.PrescriptionId })
                .HasDatabaseName("IX_ControlledMedicationRegistries_TenantId_PrescriptionId");

            builder.HasIndex(cmr => new { cmr.TenantId, cmr.RegistryType, cmr.Date })
                .HasDatabaseName("IX_ControlledMedicationRegistries_TenantId_Type_Date");

            builder.HasIndex(cmr => cmr.DocumentNumber)
                .HasDatabaseName("IX_ControlledMedicationRegistries_DocumentNumber");
        }
    }
}
