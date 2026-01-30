using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ClinicTagConfiguration : IEntityTypeConfiguration<ClinicTag>
    {
        public void Configure(EntityTypeBuilder<ClinicTag> builder)
        {
            builder.ToTable("ClinicTags");

            builder.HasKey(ct => ct.Id);

            builder.Property(ct => ct.Id)
                .ValueGeneratedNever();

            builder.Property(ct => ct.ClinicId)
                .IsRequired();

            builder.Property(ct => ct.TagId)
                .IsRequired();

            builder.Property(ct => ct.AssignedBy)
                .HasMaxLength(100);

            builder.Property(ct => ct.AssignedAt)
                .IsRequired();

            builder.Property(ct => ct.IsAutoAssigned)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(ct => ct.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(ct => ct.Clinic)
                .WithMany()
                .HasForeignKey(ct => ct.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ct => ct.Tag)
                .WithMany()
                .HasForeignKey(ct => ct.TagId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ct => ct.ClinicId)
                .HasDatabaseName("IX_ClinicTags_ClinicId");

            builder.HasIndex(ct => ct.TagId)
                .HasDatabaseName("IX_ClinicTags_TagId");

            builder.HasIndex(ct => ct.TenantId)
                .HasDatabaseName("IX_ClinicTags_TenantId");

            // Unique constraint to prevent duplicate tag assignments
            builder.HasIndex(ct => new { ct.ClinicId, ct.TagId })
                .IsUnique()
                .HasDatabaseName("IX_ClinicTags_ClinicId_TagId");
        }
    }
}
