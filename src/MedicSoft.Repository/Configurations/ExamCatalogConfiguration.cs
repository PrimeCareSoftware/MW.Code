using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ExamCatalogConfiguration : IEntityTypeConfiguration<ExamCatalog>
    {
        public void Configure(EntityTypeBuilder<ExamCatalog> builder)
        {
            builder.ToTable("ExamCatalogs");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(e => e.Description)
                .HasMaxLength(1000);

            builder.Property(e => e.ExamType)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.Category)
                .HasMaxLength(100);

            builder.Property(e => e.Preparation)
                .HasMaxLength(500);

            builder.Property(e => e.Synonyms)
                .HasMaxLength(500);

            builder.Property(e => e.TussCode)
                .HasMaxLength(50);

            builder.Property(e => e.IsActive)
                .IsRequired();

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.UpdatedAt);

            // Indexes
            builder.HasIndex(e => new { e.TenantId, e.Name })
                .HasDatabaseName("IX_ExamCatalogs_TenantId_Name");

            builder.HasIndex(e => e.TenantId)
                .HasDatabaseName("IX_ExamCatalogs_TenantId");

            builder.HasIndex(e => e.ExamType)
                .HasDatabaseName("IX_ExamCatalogs_ExamType");

            builder.HasIndex(e => e.IsActive)
                .HasDatabaseName("IX_ExamCatalogs_IsActive");

            builder.HasIndex(e => e.Category)
                .HasDatabaseName("IX_ExamCatalogs_Category");
        }
    }
}
