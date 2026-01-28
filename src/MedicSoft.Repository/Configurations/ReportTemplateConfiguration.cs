using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework Core configuration for ReportTemplate entity
    /// </summary>
    public class ReportTemplateConfiguration : IEntityTypeConfiguration<ReportTemplate>
    {
        public void Configure(EntityTypeBuilder<ReportTemplate> builder)
        {
            builder.ToTable("ReportTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.Configuration)
                .HasColumnType("TEXT");

            builder.Property(t => t.Query)
                .HasColumnType("TEXT");

            builder.Property(t => t.IsSystem)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.Icon)
                .HasMaxLength(50);

            builder.Property(t => t.SupportedFormats)
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            // Relationships
            builder.HasMany(t => t.ScheduledReports)
                .WithOne(s => s.ReportTemplate)
                .HasForeignKey(s => s.ReportTemplateId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(t => t.Category)
                .HasDatabaseName("IX_ReportTemplates_Category");

            builder.HasIndex(t => t.IsSystem)
                .HasDatabaseName("IX_ReportTemplates_IsSystem");
        }
    }
}
