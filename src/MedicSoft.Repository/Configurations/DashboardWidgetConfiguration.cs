using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for DashboardWidget entity
    /// </summary>
    public class DashboardWidgetConfiguration : IEntityTypeConfiguration<DashboardWidget>
    {
        public void Configure(EntityTypeBuilder<DashboardWidget> builder)
        {
            builder.ToTable("DashboardWidgets");

            builder.HasKey(w => w.Id);

            builder.Property(w => w.DashboardId)
                .IsRequired();

            builder.Property(w => w.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(w => w.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(w => w.Config)
                .HasColumnType("TEXT");

            builder.Property(w => w.Query)
                .HasColumnType("TEXT");

            builder.Property(w => w.RefreshInterval)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(w => w.GridX)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(w => w.GridY)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(w => w.GridWidth)
                .IsRequired()
                .HasDefaultValue(4);

            builder.Property(w => w.GridHeight)
                .IsRequired()
                .HasDefaultValue(3);

            // Relationships
            builder.HasOne(w => w.Dashboard)
                .WithMany(d => d.Widgets)
                .HasForeignKey(w => w.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(w => w.DashboardId)
                .HasDatabaseName("IX_DashboardWidgets_DashboardId");

            builder.HasIndex(w => w.Type)
                .HasDatabaseName("IX_DashboardWidgets_Type");
        }
    }
}
