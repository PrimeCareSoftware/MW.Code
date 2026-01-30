using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class EncryptionKeyConfiguration : IEntityTypeConfiguration<EncryptionKey>
    {
        public void Configure(EntityTypeBuilder<EncryptionKey> builder)
        {
            builder.ToTable("EncryptionKeys");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.KeyId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.KeyVersion)
                .IsRequired();

            builder.Property(e => e.Algorithm)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Purpose)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.EncryptedKeyMaterial)
                .HasMaxLength(2000);

            builder.Property(e => e.Description)
                .HasMaxLength(500);

            builder.Property(e => e.TenantId)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(e => new { e.KeyId, e.KeyVersion, e.TenantId })
                .IsUnique();

            builder.HasIndex(e => new { e.IsActive, e.TenantId });
            
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
