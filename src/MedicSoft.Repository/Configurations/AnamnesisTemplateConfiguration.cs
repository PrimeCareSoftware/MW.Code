using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class AnamnesisTemplateConfiguration : IEntityTypeConfiguration<AnamnesisTemplate>
    {
        public void Configure(EntityTypeBuilder<AnamnesisTemplate> builder)
        {
            builder.ToTable("AnamnesisTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Specialty)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(t => t.Description)
                .HasMaxLength(500);

            builder.Property(t => t.SectionsJson)
                .IsRequired()
                .HasColumnType("text");
            
            // Ignore the navigation property since we're using JSON serialization
            builder.Ignore(t => t.Sections);

            builder.Property(t => t.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(t => t.IsDefault)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(t => t.CreatedBy)
                .IsRequired();

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.Property(t => t.UpdatedAt);

            // Indexes
            builder.HasIndex(t => new { t.TenantId, t.Specialty })
                .HasDatabaseName("IX_AnamnesisTemplates_TenantId_Specialty");

            builder.HasIndex(t => new { t.TenantId, t.Specialty, t.IsDefault })
                .HasDatabaseName("IX_AnamnesisTemplates_TenantId_Specialty_IsDefault");

            builder.HasIndex(t => t.TenantId)
                .HasDatabaseName("IX_AnamnesisTemplates_TenantId");

            builder.HasIndex(t => t.IsActive)
                .HasDatabaseName("IX_AnamnesisTemplates_IsActive");
        }
    }
}
