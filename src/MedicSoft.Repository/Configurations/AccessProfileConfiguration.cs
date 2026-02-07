using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AccessProfileConfiguration : IEntityTypeConfiguration<AccessProfile>
    {
        public void Configure(EntityTypeBuilder<AccessProfile> builder)
        {
            builder.ToTable("AccessProfiles");

            builder.HasKey(ap => ap.Id);

            builder.Property(ap => ap.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ap => ap.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(ap => ap.IsDefault)
                .IsRequired();

            builder.Property(ap => ap.IsActive)
                .IsRequired();

            builder.Property(ap => ap.ClinicId);

            builder.Property(ap => ap.ConsultationFormProfileId);

            builder.Property(ap => ap.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(ap => ap.CreatedAt)
                .IsRequired();

            builder.Property(ap => ap.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(ap => ap.Clinic)
                .WithMany()
                .HasForeignKey(ap => ap.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(ap => ap.ConsultationFormProfile)
                .WithMany()
                .HasForeignKey(ap => ap.ConsultationFormProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(ap => ap.Permissions)
                .WithOne(pp => pp.Profile)
                .HasForeignKey(pp => pp.ProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ap => new { ap.TenantId, ap.ClinicId, ap.Name });
            builder.HasIndex(ap => new { ap.TenantId, ap.IsActive });
            builder.HasIndex(ap => ap.IsDefault);
        }
    }
}
