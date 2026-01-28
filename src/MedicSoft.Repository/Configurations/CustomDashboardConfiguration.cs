using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for CustomDashboard entity
    /// </summary>
    public class CustomDashboardConfiguration : IEntityTypeConfiguration<CustomDashboard>
    {
        public void Configure(EntityTypeBuilder<CustomDashboard> builder)
        {
            builder.ToTable("CustomDashboards");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Description)
                .HasMaxLength(1000);

            builder.Property(d => d.Layout)
                .HasColumnType("TEXT");

            builder.Property(d => d.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(d => d.IsPublic)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(d => d.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(d => d.CreatedAt)
                .IsRequired();

            builder.Property(d => d.UpdatedAt);

            // Relationships
            builder.HasMany(d => d.Widgets)
                .WithOne(w => w.Dashboard)
                .HasForeignKey(w => w.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(d => d.CreatedBy)
                .HasDatabaseName("IX_CustomDashboards_CreatedBy");

            builder.HasIndex(d => d.IsDefault)
                .HasDatabaseName("IX_CustomDashboards_IsDefault");

            builder.HasIndex(d => d.IsPublic)
                .HasDatabaseName("IX_CustomDashboards_IsPublic");
        }
    }
}
