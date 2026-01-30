using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable("Tags");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Color)
                .IsRequired()
                .HasMaxLength(7);

            builder.Property(t => t.IsAutomatic)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.AutomationRules)
                .HasColumnType("text");

            builder.Property(t => t.Order)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(t => t.TenantId)
                .HasDatabaseName("IX_Tags_TenantId");

            builder.HasIndex(t => t.Category)
                .HasDatabaseName("IX_Tags_Category");

            builder.HasIndex(t => t.IsAutomatic)
                .HasDatabaseName("IX_Tags_IsAutomatic");
        }
    }
}
