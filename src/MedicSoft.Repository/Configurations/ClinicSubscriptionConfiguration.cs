using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ClinicSubscriptionConfiguration : IEntityTypeConfiguration<ClinicSubscription>
    {
        public void Configure(EntityTypeBuilder<ClinicSubscription> builder)
        {
            builder.ToTable("ClinicSubscriptions");

            builder.HasKey(cs => cs.Id);

            builder.Property(cs => cs.ClinicId)
                .IsRequired();

            builder.Property(cs => cs.SubscriptionPlanId)
                .IsRequired();

            builder.Property(cs => cs.StartDate)
                .IsRequired();

            builder.Property(cs => cs.EndDate);

            builder.Property(cs => cs.TrialEndDate);

            builder.Property(cs => cs.Status)
                .IsRequired();

            builder.Property(cs => cs.LastPaymentDate);

            builder.Property(cs => cs.NextPaymentDate);

            builder.Property(cs => cs.CurrentPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(cs => cs.CancellationReason)
                .HasMaxLength(500);

            builder.Property(cs => cs.CancellationDate);

            builder.Property(cs => cs.IsFrozen)
                .IsRequired();

            builder.Property(cs => cs.FrozenStartDate);

            builder.Property(cs => cs.FrozenEndDate);

            builder.Property(cs => cs.PendingPlanId);

            builder.Property(cs => cs.PendingPlanPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(cs => cs.PlanChangeDate);

            builder.Property(cs => cs.IsUpgrade)
                .IsRequired();

            builder.Property(cs => cs.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cs => cs.CreatedAt)
                .IsRequired();

            builder.Property(cs => cs.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(cs => cs.Clinic)
                .WithMany()
                .HasForeignKey(cs => cs.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.SubscriptionPlan)
                .WithMany()
                .HasForeignKey(cs => cs.SubscriptionPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(cs => cs.PendingPlan)
                .WithMany()
                .HasForeignKey(cs => cs.PendingPlanId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(cs => cs.ClinicId);
            builder.HasIndex(cs => cs.Status);
            builder.HasIndex(cs => cs.NextPaymentDate);
            builder.HasIndex(cs => new { cs.TenantId, cs.Status });
        }
    }
}
