using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework configuration for SngpcAlert entity.
    /// Configures table, columns, relationships, and indexes for SNGPC alert persistence.
    /// </summary>
    public class SngpcAlertConfiguration : IEntityTypeConfiguration<SngpcAlert>
    {
        public void Configure(EntityTypeBuilder<SngpcAlert> builder)
        {
            builder.ToTable("SngpcAlerts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.Type)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Severity)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.RelatedReportId);

            builder.Property(a => a.RelatedRegistryId);

            builder.Property(a => a.RelatedBalanceId);

            builder.Property(a => a.RelatedMedication)
                .HasMaxLength(500);

            builder.Property(a => a.AdditionalData)
                .HasColumnType("text");

            builder.Property(a => a.AcknowledgedAt);

            builder.Property(a => a.AcknowledgedByUserId);

            builder.Property(a => a.AcknowledgmentNotes)
                .HasMaxLength(1000);

            builder.Property(a => a.ResolvedAt);

            builder.Property(a => a.ResolvedByUserId);

            builder.Property(a => a.Resolution)
                .HasMaxLength(1000);

            builder.Property(a => a.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            builder.Property(a => a.UpdatedAt);

            // Relationships
            builder.HasOne(a => a.RelatedReport)
                .WithMany()
                .HasForeignKey(a => a.RelatedReportId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.RelatedRegistry)
                .WithMany()
                .HasForeignKey(a => a.RelatedRegistryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.RelatedBalance)
                .WithMany()
                .HasForeignKey(a => a.RelatedBalanceId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(a => a.AcknowledgedBy)
                .WithMany()
                .HasForeignKey(a => a.AcknowledgedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.ResolvedBy)
                .WithMany()
                .HasForeignKey(a => a.ResolvedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes for performance
            builder.HasIndex(a => new { a.TenantId, a.ResolvedAt })
                .HasDatabaseName("IX_SngpcAlerts_TenantId_ResolvedAt");

            builder.HasIndex(a => new { a.TenantId, a.Type })
                .HasDatabaseName("IX_SngpcAlerts_TenantId_Type");

            builder.HasIndex(a => new { a.TenantId, a.Severity })
                .HasDatabaseName("IX_SngpcAlerts_TenantId_Severity");

            builder.HasIndex(a => a.RelatedReportId)
                .HasDatabaseName("IX_SngpcAlerts_RelatedReportId");

            builder.HasIndex(a => a.RelatedRegistryId)
                .HasDatabaseName("IX_SngpcAlerts_RelatedRegistryId");

            builder.HasIndex(a => a.RelatedBalanceId)
                .HasDatabaseName("IX_SngpcAlerts_RelatedBalanceId");

            builder.HasIndex(a => a.CreatedAt)
                .HasDatabaseName("IX_SngpcAlerts_CreatedAt");

            builder.HasIndex(a => a.AcknowledgedAt)
                .HasDatabaseName("IX_SngpcAlerts_AcknowledgedAt");
        }
    }
}
