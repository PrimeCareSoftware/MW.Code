using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class OwnerConfiguration : IEntityTypeConfiguration<Owner>
    {
        public void Configure(EntityTypeBuilder<Owner> builder)
        {
            builder.ToTable("Owners");

            builder.HasKey(o => o.Id);

            builder.Property(o => o.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(o => o.ClinicId)
                .IsRequired(false);

            builder.Property(o => o.IsActive)
                .IsRequired();

            builder.Property(o => o.LastLoginAt);

            builder.Property(o => o.CurrentSessionId)
                .HasMaxLength(200);

            builder.Property(o => o.ProfessionalId)
                .HasMaxLength(50);

            builder.Property(o => o.Specialty)
                .HasMaxLength(100);

            builder.Property(o => o.Document)
                .HasMaxLength(50);

            builder.Property(o => o.DocumentType)
                .HasConversion<string>()
                .HasMaxLength(10);

            builder.Property(o => o.IsEmailConfirmed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(o => o.EmailConfirmationToken)
                .HasMaxLength(128);

            builder.Property(o => o.EmailConfirmationTokenExpiresAt)
                .IsRequired(false);

            builder.Property(o => o.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(o => o.CreatedAt)
                .IsRequired();

            builder.Property(o => o.UpdatedAt)
                .IsRequired(false);

            // Relationships
            builder.HasOne(o => o.Clinic)
                .WithMany()
                .HasForeignKey(o => o.ClinicId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(o => o.Username).IsUnique();
            builder.HasIndex(o => o.Email);
            builder.HasIndex(o => o.ClinicId);
            builder.HasIndex(o => new { o.TenantId, o.IsActive });
        }
    }
}
