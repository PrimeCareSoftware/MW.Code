using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ClinicCustomizationConfiguration : IEntityTypeConfiguration<ClinicCustomization>
    {
        public void Configure(EntityTypeBuilder<ClinicCustomization> builder)
        {
            builder.ToTable("ClinicCustomizations");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.ClinicId)
                .IsRequired();

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.LogoUrl)
                .HasMaxLength(500);

            builder.Property(c => c.BackgroundImageUrl)
                .HasMaxLength(500);

            builder.Property(c => c.PrimaryColor)
                .HasMaxLength(20);

            builder.Property(c => c.SecondaryColor)
                .HasMaxLength(20);

            builder.Property(c => c.FontColor)
                .HasMaxLength(20);

            builder.Property(c => c.CreatedAt)
                .IsRequired();

            builder.Property(c => c.UpdatedAt)
                .IsRequired();

            // Relationship with Clinic
            builder.HasOne(c => c.Clinic)
                .WithMany()
                .HasForeignKey(c => c.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Index for faster lookups
            builder.HasIndex(c => c.ClinicId)
                .IsUnique();

            builder.HasIndex(c => c.TenantId);
        }
    }
}
