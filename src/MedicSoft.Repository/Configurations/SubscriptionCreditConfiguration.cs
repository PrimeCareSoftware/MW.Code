using MedicSoft.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MedicSoft.Repository.Configurations
{
    public class SubscriptionCreditConfiguration : IEntityTypeConfiguration<SubscriptionCredit>
    {
        public void Configure(EntityTypeBuilder<SubscriptionCredit> builder)
        {
            builder.ToTable("SubscriptionCredits");

            builder.HasKey(sc => sc.Id);

            builder.Property(sc => sc.SubscriptionId)
                .IsRequired();

            builder.Property(sc => sc.Days)
                .IsRequired();

            builder.Property(sc => sc.Reason)
                .HasMaxLength(500);

            builder.Property(sc => sc.GrantedAt)
                .IsRequired();

            builder.Property(sc => sc.GrantedBy)
                .IsRequired();

            builder.Property(sc => sc.TenantId)
                .IsRequired()
                .HasMaxLength(100)
                .HasDefaultValue(string.Empty);

            builder.Property(sc => sc.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("now()");

            // Relationships
            builder.HasOne(sc => sc.Subscription)
                .WithMany()
                .HasForeignKey(sc => sc.SubscriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(sc => sc.GrantedByUser)
                .WithMany()
                .HasForeignKey(sc => sc.GrantedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sc => sc.SubscriptionId)
                .HasDatabaseName("IX_SubscriptionCredits_SubscriptionId");

            builder.HasIndex(sc => sc.GrantedBy)
                .HasDatabaseName("IX_SubscriptionCredits_GrantedBy");

            builder.HasIndex(sc => sc.GrantedAt)
                .HasDatabaseName("IX_SubscriptionCredits_GrantedAt");
        }
    }
}
