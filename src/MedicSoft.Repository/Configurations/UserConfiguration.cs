using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(u => u.FullName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(u => u.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(u => u.ClinicId);

            builder.Property(u => u.Role)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired();

            builder.Property(u => u.LastLoginAt);

            builder.Property(u => u.ProfessionalId)
                .HasMaxLength(50);

            builder.Property(u => u.Specialty)
                .HasMaxLength(100);

            builder.Property(u => u.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(u => u.Clinic)
                .WithMany()
                .HasForeignKey(u => u.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(u => u.Username).IsUnique();
            builder.HasIndex(u => u.Email);
            builder.HasIndex(u => u.ClinicId);
            builder.HasIndex(u => u.Role);
            builder.HasIndex(u => new { u.TenantId, u.IsActive });
        }
    }
}
