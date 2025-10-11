using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MaterialConfiguration : IEntityTypeConfiguration<Material>
    {
        public void Configure(EntityTypeBuilder<Material> builder)
        {
            builder.ToTable("Materials");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(m => m.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(m => new { m.Code, m.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_Materials_Code_TenantId");

            builder.Property(m => m.Description)
                .HasMaxLength(1000);

            builder.Property(m => m.Unit)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.UnitPrice)
                .IsRequired()
                .HasPrecision(18, 2);

            builder.Property(m => m.StockQuantity)
                .IsRequired();

            builder.Property(m => m.MinimumStock)
                .IsRequired();

            builder.Property(m => m.IsActive)
                .IsRequired();

            builder.Property(m => m.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            builder.Property(m => m.UpdatedAt);
        }
    }
}
