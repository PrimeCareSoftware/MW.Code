using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SubscriptionPlanConfiguration : IEntityTypeConfiguration<SubscriptionPlan>
    {
        public void Configure(EntityTypeBuilder<SubscriptionPlan> builder)
        {
            builder.ToTable("SubscriptionPlans");

            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sp => sp.Description)
                .HasMaxLength(500);

            builder.Property(sp => sp.MonthlyPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(sp => sp.TrialDays)
                .IsRequired();

            builder.Property(sp => sp.MaxUsers)
                .IsRequired();

            builder.Property(sp => sp.MaxPatients)
                .IsRequired();

            builder.Property(sp => sp.HasReports)
                .IsRequired();

            builder.Property(sp => sp.HasWhatsAppIntegration)
                .IsRequired();

            builder.Property(sp => sp.HasSMSNotifications)
                .IsRequired();

            builder.Property(sp => sp.HasTissExport)
                .IsRequired();

            builder.Property(sp => sp.IsActive)
                .IsRequired();

            builder.Property(sp => sp.Type)
                .IsRequired();

            builder.Property(sp => sp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sp => sp.CreatedAt)
                .IsRequired();

            builder.Property(sp => sp.UpdatedAt)
                .IsRequired();

            // Indexes
            builder.HasIndex(sp => sp.Type);
            builder.HasIndex(sp => sp.IsActive);
            builder.HasIndex(sp => new { sp.TenantId, sp.Type });
        }
    }
}
