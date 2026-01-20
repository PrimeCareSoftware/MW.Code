using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ConsultationFormProfileConfiguration : IEntityTypeConfiguration<ConsultationFormProfile>
    {
        public void Configure(EntityTypeBuilder<ConsultationFormProfile> builder)
        {
            builder.ToTable("ConsultationFormProfiles");

            builder.HasKey(cfp => cfp.Id);

            builder.Property(cfp => cfp.Id)
                .ValueGeneratedNever();

            builder.Property(cfp => cfp.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cfp => cfp.Description)
                .HasMaxLength(1000);

            builder.Property(cfp => cfp.Specialty)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(cfp => cfp.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.IsSystemDefault)
                .IsRequired()
                .HasDefaultValue(false);

            // Field visibility properties
            builder.Property(cfp => cfp.ShowChiefComplaint)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.ShowHistoryOfPresentIllness)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.ShowPastMedicalHistory)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.ShowFamilyHistory)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.ShowLifestyleHabits)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfp => cfp.ShowCurrentMedications)
                .IsRequired()
                .HasDefaultValue(true);

            // Custom fields stored as JSON
            builder.Property(cfp => cfp.CustomFieldsJson)
                .IsRequired()
                .HasColumnType("jsonb")
                .HasDefaultValue("[]");

            builder.Property(cfp => cfp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cfp => cfp.CreatedAt)
                .IsRequired();

            builder.Property(cfp => cfp.UpdatedAt);

            // Indexes
            builder.HasIndex(cfp => new { cfp.TenantId, cfp.Specialty, cfp.IsActive })
                .HasDatabaseName("IX_ConsultationFormProfiles_TenantId_Specialty_IsActive");

            builder.HasIndex(cfp => new { cfp.TenantId, cfp.IsSystemDefault })
                .HasDatabaseName("IX_ConsultationFormProfiles_TenantId_IsSystemDefault");

            builder.HasIndex(cfp => cfp.TenantId)
                .HasDatabaseName("IX_ConsultationFormProfiles_TenantId");
        }
    }
}
