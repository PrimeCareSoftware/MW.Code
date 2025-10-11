using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ProcedureMaterialConfiguration : IEntityTypeConfiguration<ProcedureMaterial>
    {
        public void Configure(EntityTypeBuilder<ProcedureMaterial> builder)
        {
            builder.ToTable("ProcedureMaterials");

            builder.HasKey(pm => pm.Id);

            builder.Property(pm => pm.Id)
                .ValueGeneratedNever();

            builder.Property(pm => pm.ProcedureId)
                .IsRequired();

            builder.Property(pm => pm.MaterialId)
                .IsRequired();

            builder.Property(pm => pm.Quantity)
                .IsRequired();

            builder.Property(pm => pm.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pm => pm.CreatedAt)
                .IsRequired();

            builder.Property(pm => pm.UpdatedAt);

            // Navigation properties
            builder.HasOne(pm => pm.Procedure)
                .WithMany(p => p.Materials)
                .HasForeignKey(pm => pm.ProcedureId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(pm => pm.Material)
                .WithMany()
                .HasForeignKey(pm => pm.MaterialId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(pm => new { pm.ProcedureId, pm.MaterialId, pm.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_ProcedureMaterials_ProcedureId_MaterialId_TenantId");
        }
    }
}
