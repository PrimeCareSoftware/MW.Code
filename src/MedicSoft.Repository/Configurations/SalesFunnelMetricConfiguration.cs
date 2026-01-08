using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SalesFunnelMetricConfiguration : IEntityTypeConfiguration<SalesFunnelMetric>
    {
        public void Configure(EntityTypeBuilder<SalesFunnelMetric> builder)
        {
            builder.ToTable("SalesFunnelMetrics");

            builder.HasKey(m => m.Id);

            builder.Property(m => m.Id)
                .ValueGeneratedNever();

            builder.Property(m => m.SessionId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Step)
                .IsRequired();

            builder.Property(m => m.StepName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(m => m.CapturedData)
                .HasColumnType("text");

            builder.Property(m => m.PlanId)
                .HasMaxLength(100);

            builder.Property(m => m.IpAddress)
                .HasMaxLength(45); // IPv6 max length

            builder.Property(m => m.UserAgent)
                .HasMaxLength(500);

            builder.Property(m => m.Referrer)
                .HasMaxLength(500);

            builder.Property(m => m.Metadata)
                .HasColumnType("text");

            builder.Property(m => m.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(m => m.CreatedAt)
                .IsRequired();

            // Indexes for efficient querying
            builder.HasIndex(m => m.SessionId)
                .HasDatabaseName("IX_SalesFunnelMetrics_SessionId");

            builder.HasIndex(m => m.CreatedAt)
                .HasDatabaseName("IX_SalesFunnelMetrics_CreatedAt");

            builder.HasIndex(m => new { m.Step, m.CreatedAt })
                .HasDatabaseName("IX_SalesFunnelMetrics_Step_CreatedAt");

            builder.HasIndex(m => m.IsConverted)
                .HasDatabaseName("IX_SalesFunnelMetrics_IsConverted");

            builder.HasIndex(m => new { m.ClinicId })
                .HasDatabaseName("IX_SalesFunnelMetrics_ClinicId");

            builder.HasIndex(m => new { m.SessionId, m.CreatedAt })
                .HasDatabaseName("IX_SalesFunnelMetrics_SessionId_CreatedAt");
        }
    }
}
