using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class LoginAttemptConfiguration : IEntityTypeConfiguration<LoginAttempt>
    {
        public void Configure(EntityTypeBuilder<LoginAttempt> builder)
        {
            builder.ToTable("LoginAttempts");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.Username)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.AttemptTime)
                .IsRequired();

            builder.Property(l => l.WasSuccessful)
                .IsRequired();

            builder.Property(l => l.FailureReason)
                .HasMaxLength(500);

            builder.Property(l => l.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.CreatedAt)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(l => l.Username);
            builder.HasIndex(l => l.IpAddress);
            builder.HasIndex(l => l.AttemptTime);
            builder.HasIndex(l => l.TenantId);
            builder.HasIndex(l => new { l.Username, l.WasSuccessful, l.AttemptTime });
        }
    }

    public class AccountLockoutConfiguration : IEntityTypeConfiguration<AccountLockout>
    {
        public void Configure(EntityTypeBuilder<AccountLockout> builder)
        {
            builder.ToTable("AccountLockouts");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.UserId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.LockedAt)
                .IsRequired();

            builder.Property(a => a.UnlocksAt)
                .IsRequired();

            builder.Property(a => a.FailedAttempts)
                .IsRequired();

            builder.Property(a => a.IsActive)
                .IsRequired();

            builder.Property(a => a.UnlockedBy)
                .HasMaxLength(200);

            builder.Property(a => a.UnlockedAt);

            builder.Property(a => a.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.CreatedAt)
                .IsRequired();

            // Indexes for performance
            builder.HasIndex(a => a.UserId);
            builder.HasIndex(a => a.TenantId);
            builder.HasIndex(a => a.IsActive);
            builder.HasIndex(a => new { a.UserId, a.IsActive, a.UnlocksAt });
        }
    }
}
