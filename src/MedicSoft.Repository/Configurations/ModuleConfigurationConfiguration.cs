using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ModuleConfigurationConfiguration : IEntityTypeConfiguration<ModuleConfiguration>
    {
        public void Configure(EntityTypeBuilder<ModuleConfiguration> builder)
        {
            builder.ToTable("ModuleConfigurations");

            builder.HasKey(mc => mc.Id);

            builder.Property(mc => mc.ClinicId)
                .IsRequired();

            builder.Property(mc => mc.ModuleName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mc => mc.IsEnabled)
                .IsRequired();

            builder.Property(mc => mc.Configuration)
                .HasMaxLength(2000);

            builder.Property(mc => mc.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(mc => mc.CreatedAt)
                .IsRequired();

            builder.Property(mc => mc.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(mc => mc.Clinic)
                .WithMany()
                .HasForeignKey(mc => mc.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(mc => mc.ClinicId);
            builder.HasIndex(mc => new { mc.ClinicId, mc.ModuleName }).IsUnique();
            builder.HasIndex(mc => new { mc.TenantId, mc.IsEnabled });
        }
    }
}
