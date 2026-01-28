using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for WidgetTemplate entity
    /// </summary>
    public class WidgetTemplateConfiguration : IEntityTypeConfiguration<WidgetTemplate>
    {
        public void Configure(EntityTypeBuilder<WidgetTemplate> builder)
        {
            builder.ToTable("WidgetTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.DefaultConfig)
                .HasColumnType("TEXT");

            builder.Property(t => t.DefaultQuery)
                .HasColumnType("TEXT");

            builder.Property(t => t.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.Icon)
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(t => t.Category)
                .HasDatabaseName("IX_WidgetTemplates_Category");

            builder.HasIndex(t => t.Type)
                .HasDatabaseName("IX_WidgetTemplates_Type");

            builder.HasIndex(t => t.IsSystem)
                .HasDatabaseName("IX_WidgetTemplates_IsSystem");
        }
    }
}
