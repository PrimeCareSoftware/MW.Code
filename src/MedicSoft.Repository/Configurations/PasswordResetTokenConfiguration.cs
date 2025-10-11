using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class PasswordResetTokenConfiguration : IEntityTypeConfiguration<PasswordResetToken>
    {
        public void Configure(EntityTypeBuilder<PasswordResetToken> builder)
        {
            builder.ToTable("PasswordResetTokens");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.UserId)
                .IsRequired();

            builder.Property(t => t.Token)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.VerificationCode)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(t => t.Method)
                .IsRequired();

            builder.Property(t => t.Destination)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.ExpiresAt)
                .IsRequired();

            builder.Property(t => t.IsUsed)
                .IsRequired();

            builder.Property(t => t.IsVerified)
                .IsRequired();

            builder.Property(t => t.VerifiedAt);

            builder.Property(t => t.UsedAt);

            builder.Property(t => t.VerificationAttempts)
                .IsRequired();

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            // Relationships
            builder.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(t => t.Token).IsUnique();
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => new { t.TenantId, t.IsUsed, t.ExpiresAt });
        }
    }
}
