using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class OwnerSessionConfiguration : IEntityTypeConfiguration<OwnerSession>
    {
        public void Configure(EntityTypeBuilder<OwnerSession> builder)
        {
            builder.HasKey(os => os.Id);

            builder.Property(os => os.Id)
                .ValueGeneratedNever();

            builder.Property(os => os.OwnerId)
                .IsRequired();

            builder.Property(os => os.SessionId)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(os => os.TenantId)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(os => os.CreatedAt)
                .IsRequired();

            builder.Property(os => os.ExpiresAt)
                .IsRequired();

            builder.Property(os => os.LastActivityAt)
                .IsRequired();

            builder.Property(os => os.UserAgent)
                .HasMaxLength(500);

            builder.Property(os => os.IpAddress)
                .HasMaxLength(45); // IPv6 can be up to 45 characters

            // Indexes for common queries
            builder.HasIndex(os => new { os.OwnerId, os.SessionId })
                .HasDatabaseName("idx_ownersession_ownerid_sessionid")
                .IsUnique();

            builder.HasIndex(os => os.ExpiresAt)
                .HasDatabaseName("idx_ownersession_expiresat");

            builder.HasIndex(os => os.TenantId)
                .HasDatabaseName("idx_ownersession_tenantid");

            // Foreign key
            builder.HasOne(os => os.Owner)
                .WithMany()
                .HasForeignKey(os => os.OwnerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Table name
            builder.ToTable("owner_sessions", schema: "public");
        }
    }
}
