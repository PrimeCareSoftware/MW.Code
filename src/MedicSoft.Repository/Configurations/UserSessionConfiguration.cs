using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class UserSessionConfiguration : IEntityTypeConfiguration<UserSession>
    {
        public void Configure(EntityTypeBuilder<UserSession> builder)
        {
            builder.HasKey(us => us.Id);

            builder.Property(us => us.Id)
                .ValueGeneratedNever();

            builder.Property(us => us.UserId)
                .IsRequired();

            builder.Property(us => us.SessionId)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(us => us.TenantId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(us => us.CreatedAt)
                .IsRequired();

            builder.Property(us => us.ExpiresAt)
                .IsRequired();

            builder.Property(us => us.LastActivityAt)
                .IsRequired();

            builder.Property(us => us.UserAgent)
                .HasMaxLength(500);

            builder.Property(us => us.IpAddress)
                .HasMaxLength(45); // IPv6 can be up to 45 characters

            // Indexes for common queries
            builder.HasIndex(us => new { us.UserId, us.SessionId })
                .HasDatabaseName("idx_usersession_userid_sessionid")
                .IsUnique();

            builder.HasIndex(us => us.ExpiresAt)
                .HasDatabaseName("idx_usersession_expiresat");

            builder.HasIndex(us => us.TenantId)
                .HasDatabaseName("idx_usersession_tenantid");

            // Foreign key
            builder.HasOne(us => us.User)
                .WithMany()
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table name
            builder.ToTable("user_sessions", schema: "public");
        }
    }
}
