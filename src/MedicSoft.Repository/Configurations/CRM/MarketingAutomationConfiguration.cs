using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class MarketingAutomationConfiguration : IEntityTypeConfiguration<MarketingAutomation>
    {
        public void Configure(EntityTypeBuilder<MarketingAutomation> builder)
        {
            builder.ToTable("MarketingAutomations", schema: "crm");

            builder.HasKey(ma => ma.Id);

            builder.Property(ma => ma.Id)
                .ValueGeneratedNever();

            builder.Property(ma => ma.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ma => ma.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(ma => ma.IsActive)
                .IsRequired();

            builder.Property(ma => ma.TriggerType)
                .IsRequired();

            builder.Property(ma => ma.TriggerStage);

            builder.Property(ma => ma.TriggerEvent)
                .HasMaxLength(200);

            builder.Property(ma => ma.DelayMinutes);

            builder.Property(ma => ma.SegmentFilter)
                .HasMaxLength(4000);

            builder.Property(ma => ma.TimesExecuted)
                .IsRequired();

            builder.Property(ma => ma.LastExecutedAt);

            builder.Property(ma => ma.SuccessRate)
                .IsRequired();

            builder.Property(ma => ma.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ma => ma.CreatedAt)
                .IsRequired();

            builder.Property(ma => ma.UpdatedAt)
                .IsRequired();

            // Configure Tags as JSON or separate table
            builder.Property<List<string>>("_tags")
                .HasColumnName("Tags")
                .HasColumnType("jsonb");

            builder.HasMany<AutomationAction>()
                .WithOne()
                .HasForeignKey("MarketingAutomationId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ma => ma.IsActive);
            builder.HasIndex(ma => ma.TriggerType);
            builder.HasIndex(ma => ma.TriggerStage);
            builder.HasIndex(ma => ma.TenantId);
        }
    }
}
