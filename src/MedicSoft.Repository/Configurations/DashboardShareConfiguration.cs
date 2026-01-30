using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for DashboardShare entity
    /// Category 4.1: Dashboard Sharing
    /// </summary>
    public class DashboardShareConfiguration : IEntityTypeConfiguration<DashboardShare>
    {
        public void Configure(EntityTypeBuilder<DashboardShare> builder)
        {
            builder.ToTable("DashboardShares");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.DashboardId)
                .IsRequired();

            builder.Property(s => s.SharedWithUserId)
                .HasMaxLength(450);

            builder.Property(s => s.SharedWithRole)
                .HasMaxLength(100);

            builder.Property(s => s.PermissionLevel)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.SharedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(s => s.ExpiresAt);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt);

            // Relationships
            builder.HasOne(s => s.Dashboard)
                .WithMany()
                .HasForeignKey(s => s.DashboardId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes for performance
            builder.HasIndex(s => s.DashboardId)
                .HasDatabaseName("IX_DashboardShares_DashboardId");

            builder.HasIndex(s => s.SharedWithUserId)
                .HasDatabaseName("IX_DashboardShares_SharedWithUserId");

            builder.HasIndex(s => s.SharedWithRole)
                .HasDatabaseName("IX_DashboardShares_SharedWithRole");

            builder.HasIndex(s => s.ExpiresAt)
                .HasDatabaseName("IX_DashboardShares_ExpiresAt");

            // Composite index for common queries
            builder.HasIndex(s => new { s.SharedWithUserId, s.ExpiresAt })
                .HasDatabaseName("IX_DashboardShares_User_Expires");

            builder.HasIndex(s => new { s.SharedWithRole, s.ExpiresAt })
                .HasDatabaseName("IX_DashboardShares_Role_Expires");
        }
    }
}
