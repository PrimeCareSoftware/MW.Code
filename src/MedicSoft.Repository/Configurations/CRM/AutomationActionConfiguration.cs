using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class AutomationActionConfiguration : IEntityTypeConfiguration<AutomationAction>
    {
        public void Configure(EntityTypeBuilder<AutomationAction> builder)
        {
            builder.ToTable("AutomationActions", schema: "crm");

            builder.HasKey(aa => aa.Id);

            builder.Property(aa => aa.Id)
                .ValueGeneratedNever();

            builder.Property(aa => aa.MarketingAutomationId)
                .IsRequired();

            builder.Property(aa => aa.Type)
                .IsRequired();

            builder.Property(aa => aa.Order)
                .IsRequired();

            builder.Property(aa => aa.Channel)
                .HasMaxLength(100);

            builder.Property(aa => aa.EmailTemplateId);

            builder.Property(aa => aa.MessageTemplate)
                .HasMaxLength(2000);

            builder.Property(aa => aa.TagToAdd)
                .HasMaxLength(100);

            builder.Property(aa => aa.ScoreChange);

            builder.Property(aa => aa.Condition)
                .HasMaxLength(1000);

            builder.Property(aa => aa.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(aa => aa.CreatedAt)
                .IsRequired();

            builder.Property(aa => aa.UpdatedAt)
                .IsRequired();

            builder.HasOne(aa => aa.MarketingAutomation)
                .WithMany(ma => ma.Actions)
                .HasForeignKey(aa => aa.MarketingAutomationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(aa => aa.EmailTemplate)
                .WithMany()
                .HasForeignKey(aa => aa.EmailTemplateId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(aa => aa.Type);
            builder.HasIndex(aa => aa.MarketingAutomationId);
            builder.HasIndex(aa => aa.TenantId);
        }
    }
}
