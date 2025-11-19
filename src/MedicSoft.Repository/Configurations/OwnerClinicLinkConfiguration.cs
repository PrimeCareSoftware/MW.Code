using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class OwnerClinicLinkConfiguration : IEntityTypeConfiguration<OwnerClinicLink>
    {
        public void Configure(EntityTypeBuilder<OwnerClinicLink> builder)
        {
            builder.ToTable("OwnerClinicLinks");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.LinkedDate)
                .IsRequired();

            builder.Property(l => l.IsActive)
                .IsRequired();

            builder.Property(l => l.IsPrimaryOwner)
                .IsRequired();

            builder.Property(l => l.Role)
                .HasMaxLength(100);

            builder.Property(l => l.OwnershipPercentage)
                .HasPrecision(5, 2);

            builder.Property(l => l.InactivationReason)
                .HasMaxLength(500);

            builder.Property(l => l.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(l => l.Owner)
                .WithMany()
                .HasForeignKey(l => l.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(l => l.Clinic)
                .WithMany()
                .HasForeignKey(l => l.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(l => new { l.OwnerId, l.ClinicId })
                .IsUnique()
                .HasDatabaseName("IX_OwnerClinicLinks_Owner_Clinic");

            builder.HasIndex(l => new { l.TenantId, l.ClinicId })
                .HasDatabaseName("IX_OwnerClinicLinks_TenantId_ClinicId");

            builder.HasIndex(l => l.OwnerId)
                .HasDatabaseName("IX_OwnerClinicLinks_OwnerId");

            builder.HasIndex(l => new { l.ClinicId, l.IsPrimaryOwner })
                .HasDatabaseName("IX_OwnerClinicLinks_ClinicId_IsPrimaryOwner");
        }
    }
}
