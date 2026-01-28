using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class WebhookDeliveryConfiguration : IEntityTypeConfiguration<WebhookDelivery>
    {
        public void Configure(EntityTypeBuilder<WebhookDelivery> builder)
        {
            builder.ToTable("WebhookDeliveries", schema: "crm");

            builder.HasKey(wd => wd.Id);

            builder.Property(wd => wd.Id)
                .ValueGeneratedNever();

            builder.Property(wd => wd.SubscriptionId)
                .IsRequired();

            builder.Property(wd => wd.Event)
                .IsRequired();

            builder.Property(wd => wd.Payload)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(wd => wd.Status)
                .IsRequired();

            builder.Property(wd => wd.TargetUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(wd => wd.AttemptCount)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(wd => wd.ResponseBody)
                .HasColumnType("text");

            builder.Property(wd => wd.ErrorMessage)
                .HasColumnType("text");

            builder.Property(wd => wd.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(wd => wd.CreatedAt)
                .IsRequired();

            builder.Property(wd => wd.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne<WebhookSubscription>()
                .WithMany()
                .HasForeignKey(wd => wd.SubscriptionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(wd => wd.SubscriptionId);
            builder.HasIndex(wd => wd.Status);
            builder.HasIndex(wd => wd.NextRetryAt);
            builder.HasIndex(wd => wd.TenantId);
            builder.HasIndex(wd => wd.CreatedAt);
        }
    }
}
