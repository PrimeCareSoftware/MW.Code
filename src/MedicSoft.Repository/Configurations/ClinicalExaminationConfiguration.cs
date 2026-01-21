using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class ClinicalExaminationConfiguration : IEntityTypeConfiguration<ClinicalExamination>
    {
        public void Configure(EntityTypeBuilder<ClinicalExamination> builder)
        {
            builder.ToTable("ClinicalExaminations");

            builder.HasKey(ce => ce.Id);

            builder.Property(ce => ce.Id)
                .ValueGeneratedNever();

            // Sinais vitais
            builder.Property(ce => ce.BloodPressureSystolic)
                .HasPrecision(5, 1);

            builder.Property(ce => ce.BloodPressureDiastolic)
                .HasPrecision(5, 1);

            builder.Property(ce => ce.HeartRate);

            builder.Property(ce => ce.RespiratoryRate);

            builder.Property(ce => ce.Temperature)
                .HasPrecision(4, 1);

            builder.Property(ce => ce.OxygenSaturation)
                .HasPrecision(5, 2);

            // Medidas antropométricas
            builder.Property(ce => ce.Weight)
                .HasPrecision(6, 2);

            builder.Property(ce => ce.Height)
                .HasPrecision(4, 2);

            // Exame físico sistemático (obrigatório)
            builder.Property(ce => ce.SystematicExamination)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(ce => ce.GeneralState)
                .HasMaxLength(1000);

            builder.Property(ce => ce.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(ce => ce.MedicalRecord)
                .WithMany(mr => mr.Examinations)
                .HasForeignKey(ce => ce.MedicalRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(ce => new { ce.TenantId, ce.MedicalRecordId })
                .HasDatabaseName("IX_ClinicalExaminations_TenantId_MedicalRecord");

            builder.HasIndex(ce => ce.TenantId)
                .HasDatabaseName("IX_ClinicalExaminations_TenantId");
        }
    }
}
