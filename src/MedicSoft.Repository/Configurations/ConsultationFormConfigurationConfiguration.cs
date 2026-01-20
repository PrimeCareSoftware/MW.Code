using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ConsultationFormConfigurationConfiguration : IEntityTypeConfiguration<ConsultationFormConfiguration>
    {
        public void Configure(EntityTypeBuilder<ConsultationFormConfiguration> builder)
        {
            builder.ToTable("ConsultationFormConfigurations");

            builder.HasKey(cfc => cfc.Id);

            builder.Property(cfc => cfc.Id)
                .ValueGeneratedNever();

            builder.Property(cfc => cfc.ConfigurationName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(cfc => cfc.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            // Field visibility properties
            builder.Property(cfc => cfc.ShowChiefComplaint)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfc => cfc.ShowHistoryOfPresentIllness)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfc => cfc.ShowPastMedicalHistory)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfc => cfc.ShowFamilyHistory)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfc => cfc.ShowLifestyleHabits)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(cfc => cfc.ShowCurrentMedications)
                .IsRequired()
                .HasDefaultValue(true);

            // Custom fields stored as JSON
            builder.Property(cfc => cfc.CustomFieldsJson)
                .IsRequired()
                .HasColumnType("jsonb")
                .HasDefaultValue("[]");

            builder.Property(cfc => cfc.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cfc => cfc.CreatedAt)
                .IsRequired();

            builder.Property(cfc => cfc.UpdatedAt);

            // Relationships
            builder.HasOne(cfc => cfc.Clinic)
                .WithMany()
                .HasForeignKey(cfc => cfc.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(cfc => cfc.Profile)
                .WithMany(p => p.Configurations)
                .HasForeignKey(cfc => cfc.ProfileId)
                .OnDelete(DeleteBehavior.SetNull);

            // Indexes
            builder.HasIndex(cfc => new { cfc.TenantId, cfc.ClinicId, cfc.IsActive })
                .HasDatabaseName("IX_ConsultationFormConfigurations_TenantId_ClinicId_IsActive");

            builder.HasIndex(cfc => new { cfc.TenantId, cfc.ProfileId })
                .HasDatabaseName("IX_ConsultationFormConfigurations_TenantId_ProfileId");

            builder.HasIndex(cfc => cfc.TenantId)
                .HasDatabaseName("IX_ConsultationFormConfigurations_TenantId");
        }
    }
}
