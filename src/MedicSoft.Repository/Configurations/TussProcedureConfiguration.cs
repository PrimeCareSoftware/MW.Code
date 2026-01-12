using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TussProcedureConfiguration : IEntityTypeConfiguration<TussProcedure>
    {
        public void Configure(EntityTypeBuilder<TussProcedure> builder)
        {
            builder.ToTable("TussProcedures");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.Code)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.ReferencePrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(p => new { p.TenantId, p.Code })
                .IsUnique()
                .HasDatabaseName("IX_TussProcedures_TenantId_Code");

            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_TussProcedures_TenantId");

            builder.HasIndex(p => p.Category)
                .HasDatabaseName("IX_TussProcedures_Category");

            builder.HasIndex(p => p.Description)
                .HasDatabaseName("IX_TussProcedures_Description");
        }
    }
}
