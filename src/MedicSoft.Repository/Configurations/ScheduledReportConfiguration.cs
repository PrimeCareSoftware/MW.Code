using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for ScheduledReport entity
    /// </summary>
    public class ScheduledReportConfiguration : IEntityTypeConfiguration<ScheduledReport>
    {
        public void Configure(EntityTypeBuilder<ScheduledReport> builder)
        {
            builder.ToTable("ScheduledReports");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ReportTemplateId)
                .IsRequired();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .HasMaxLength(1000);

            builder.Property(s => s.CronExpression)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.OutputFormat)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.Recipients)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.Parameters)
                .HasColumnType("TEXT");

            builder.Property(s => s.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(s => s.CreatedBy)
                .IsRequired()
                .HasMaxLength(450);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt);

            builder.Property(s => s.LastRunAt);

            builder.Property(s => s.NextRunAt);

            builder.Property(s => s.LastRunStatus)
                .HasMaxLength(50);

            builder.Property(s => s.LastRunError)
                .HasMaxLength(2000);

            // Relationships
            builder.HasOne(s => s.ReportTemplate)
                .WithMany(t => t.ScheduledReports)
                .HasForeignKey(s => s.ReportTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(s => s.ReportTemplateId)
                .HasDatabaseName("IX_ScheduledReports_ReportTemplateId");

            builder.HasIndex(s => s.IsActive)
                .HasDatabaseName("IX_ScheduledReports_IsActive");

            builder.HasIndex(s => s.NextRunAt)
                .HasDatabaseName("IX_ScheduledReports_NextRunAt");

            builder.HasIndex(s => s.CreatedBy)
                .HasDatabaseName("IX_ScheduledReports_CreatedBy");
        }
    }
}
