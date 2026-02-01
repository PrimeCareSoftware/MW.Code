using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class EmailVerificationTokenConfiguration : IEntityTypeConfiguration<EmailVerificationToken>
    {
        public void Configure(EntityTypeBuilder<EmailVerificationToken> builder)
        {
            builder.ToTable("EmailVerificationTokens");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.UserId)
                .IsRequired();

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(e => e.Purpose)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                .IsRequired();

            builder.Property(e => e.ExpiresAt)
                .IsRequired();

            builder.Property(e => e.IsUsed)
                .IsRequired();

            builder.Property(e => e.VerificationAttempts)
                .IsRequired()
                .HasDefaultValue(0);

            // Indexes for performance
            builder.HasIndex(e => new { e.Code, e.UserId, e.TenantId })
                .HasDatabaseName("IX_EmailVerificationTokens_Code_UserId_TenantId");

            builder.HasIndex(e => new { e.UserId, e.TenantId, e.CreatedAt })
                .HasDatabaseName("IX_EmailVerificationTokens_UserId_TenantId_CreatedAt");

            builder.HasIndex(e => new { e.TenantId, e.ExpiresAt })
                .HasDatabaseName("IX_EmailVerificationTokens_TenantId_ExpiresAt");

            // Navigation
            builder.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .HasPrincipalKey(u => u.Id)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
