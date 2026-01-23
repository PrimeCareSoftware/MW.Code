using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class UserClinicLinkConfiguration : IEntityTypeConfiguration<UserClinicLink>
    {
        public void Configure(EntityTypeBuilder<UserClinicLink> builder)
        {
            builder.ToTable("UserClinicLinks");

            builder.HasKey(ucl => ucl.Id);

            builder.Property(ucl => ucl.Id)
                .ValueGeneratedNever();

            builder.Property(ucl => ucl.UserId)
                .IsRequired();

            builder.Property(ucl => ucl.ClinicId)
                .IsRequired();

            builder.Property(ucl => ucl.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ucl => ucl.LinkedDate)
                .IsRequired();

            builder.Property(ucl => ucl.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(ucl => ucl.IsPreferredClinic)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ucl => ucl.InactivationReason)
                .HasMaxLength(500);

            // Relationships
            builder.HasOne(ucl => ucl.User)
                .WithMany(u => u.ClinicLinks)
                .HasForeignKey(ucl => ucl.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ucl => ucl.Clinic)
                .WithMany()
                .HasForeignKey(ucl => ucl.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ucl => new { ucl.UserId, ucl.ClinicId, ucl.TenantId })
                .IsUnique()
                .HasDatabaseName("IX_UserClinicLinks_UserId_ClinicId_TenantId");

            builder.HasIndex(ucl => ucl.UserId)
                .HasDatabaseName("IX_UserClinicLinks_UserId");

            builder.HasIndex(ucl => ucl.ClinicId)
                .HasDatabaseName("IX_UserClinicLinks_ClinicId");

            builder.HasIndex(ucl => ucl.TenantId)
                .HasDatabaseName("IX_UserClinicLinks_TenantId");

            builder.HasIndex(ucl => new { ucl.UserId, ucl.IsActive })
                .HasDatabaseName("IX_UserClinicLinks_UserId_IsActive");
        }
    }
}
