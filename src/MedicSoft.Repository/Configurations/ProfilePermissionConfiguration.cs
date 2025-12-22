using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ProfilePermissionConfiguration : IEntityTypeConfiguration<ProfilePermission>
    {
        public void Configure(EntityTypeBuilder<ProfilePermission> builder)
        {
            builder.ToTable("ProfilePermissions");

            builder.HasKey(pp => pp.Id);

            builder.Property(pp => pp.ProfileId)
                .IsRequired();

            builder.Property(pp => pp.PermissionKey)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pp => pp.IsActive)
                .IsRequired();

            builder.Property(pp => pp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pp => pp.CreatedAt)
                .IsRequired();

            builder.Property(pp => pp.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(pp => pp.Profile)
                .WithMany(ap => ap.Permissions)
                .HasForeignKey(pp => pp.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pp => new { pp.ProfileId, pp.PermissionKey }).IsUnique();
            builder.HasIndex(pp => pp.TenantId);
        }
    }
}
