using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;
using System.Collections.Generic;
using System.Linq;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class WebhookSubscriptionConfiguration : IEntityTypeConfiguration<WebhookSubscription>
    {
        public void Configure(EntityTypeBuilder<WebhookSubscription> builder)
        {
            builder.ToTable("WebhookSubscriptions", schema: "crm");

            builder.HasKey(ws => ws.Id);

            builder.Property(ws => ws.Id)
                .ValueGeneratedNever();

            builder.Property(ws => ws.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(ws => ws.Description)
                .HasMaxLength(500);

            builder.Property(ws => ws.TargetUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(ws => ws.IsActive)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ws => ws.Secret)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ws => ws.SubscribedEvents)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => (IReadOnlyCollection<WebhookEvent>)(System.Text.Json.JsonSerializer.Deserialize<List<WebhookEvent>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<WebhookEvent>())
                )
                .HasColumnType("jsonb")
                .IsRequired()
                .Metadata.SetValueComparer(new Microsoft.EntityFrameworkCore.ChangeTracking.ValueComparer<IReadOnlyCollection<WebhookEvent>>(
                    (c1, c2) => c1!.SequenceEqual(c2!),
                    c => c!.Aggregate(0, (a, v) => System.HashCode.Combine(a, v.GetHashCode())),
                    c => c!.ToList()));

            builder.Property(ws => ws.MaxRetries)
                .IsRequired()
                .HasDefaultValue(3);

            builder.Property(ws => ws.RetryDelaySeconds)
                .IsRequired()
                .HasDefaultValue(60);

            builder.Property(ws => ws.TotalDeliveries)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(ws => ws.SuccessfulDeliveries)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(ws => ws.FailedDeliveries)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(ws => ws.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ws => ws.CreatedAt)
                .IsRequired();

            builder.Property(ws => ws.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(ws => ws.TenantId);
            builder.HasIndex(ws => ws.IsActive);
        }
    }
}
