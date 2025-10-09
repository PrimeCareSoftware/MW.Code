using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class MedicalRecordTemplateConfiguration : IEntityTypeConfiguration<MedicalRecordTemplate>
    {
        public void Configure(EntityTypeBuilder<MedicalRecordTemplate> builder)
        {
            builder.ToTable("MedicalRecordTemplates");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .ValueGeneratedNever();

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasMaxLength(1000);

            builder.Property(t => t.TemplateContent)
                .IsRequired()
                .HasMaxLength(10000);

            builder.Property(t => t.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.Property(t => t.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Indexes
            builder.HasIndex(t => new { t.TenantId, t.Category })
                .HasDatabaseName("IX_MedicalRecordTemplates_TenantId_Category");

            builder.HasIndex(t => new { t.TenantId, t.Name })
                .HasDatabaseName("IX_MedicalRecordTemplates_TenantId_Name");
        }
    }
}
