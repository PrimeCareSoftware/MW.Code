using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;
using System.Text.Json;

namespace MedicSoft.Repository.Configurations
{
    public class TwoFactorAuthConfiguration : IEntityTypeConfiguration<TwoFactorAuth>
    {
        public void Configure(EntityTypeBuilder<TwoFactorAuth> builder)
        {
            builder.ToTable("TwoFactorAuth");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.UserId)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.IsEnabled)
                .IsRequired();

            builder.Property(t => t.Method)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(t => t.SecretKey)
                .HasMaxLength(500); // Encrypted

            builder.Property(t => t.EnabledAt);

            builder.Property(t => t.EnabledByIp)
                .HasMaxLength(50);

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            // Configure BackupCodes as owned entity collection
            builder.OwnsMany(t => t.BackupCodes, bc =>
            {
                bc.ToTable("TwoFactorBackupCodes");
                
                bc.WithOwner().HasForeignKey("TwoFactorAuthId");
                
                bc.Property<int>("Id")
                    .ValueGeneratedOnAdd();
                
                bc.HasKey("Id");

                bc.Property(b => b.Code)
                    .IsRequired()
                    .HasMaxLength(50);

                bc.Property(b => b.HashedCode)
                    .IsRequired()
                    .HasMaxLength(500);

                bc.Property(b => b.IsUsed)
                    .IsRequired();

                bc.Property(b => b.UsedAt);
            });

            // Indexes for performance
            builder.HasIndex(t => t.UserId);
            builder.HasIndex(t => t.TenantId);
            builder.HasIndex(t => new { t.UserId, t.TenantId });
            builder.HasIndex(t => new { t.UserId, t.IsEnabled });
        }
    }
}
