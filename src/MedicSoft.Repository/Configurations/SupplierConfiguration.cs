using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(s => s.TradeName)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(s => s.DocumentNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(s => s.Email)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(s => s.Phone)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(s => s.Address)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(s => s.City)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(s => s.State)
                .HasMaxLength(2)
                .IsRequired(false);

            builder.Property(s => s.ZipCode)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(s => s.BankName)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(s => s.BankAccount)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(s => s.PixKey)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.Property(s => s.Notes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(s => s.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Indexes
            builder.HasIndex(s => s.Name);
            builder.HasIndex(s => s.DocumentNumber);
            builder.HasIndex(s => s.IsActive);
            builder.HasIndex(s => s.TenantId);
        }
    }
}
