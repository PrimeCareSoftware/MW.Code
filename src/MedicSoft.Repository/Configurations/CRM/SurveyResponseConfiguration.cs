using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyResponse> builder)
        {
            builder.ToTable("SurveyResponses", schema: "crm");

            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Id)
                .ValueGeneratedNever();

            builder.Property(sr => sr.SurveyId)
                .IsRequired();

            builder.Property(sr => sr.PatientId)
                .IsRequired();

            builder.Property(sr => sr.StartedAt)
                .IsRequired();

            builder.Property(sr => sr.CompletedAt);

            builder.Property(sr => sr.IsCompleted)
                .IsRequired();

            builder.Property(sr => sr.NpsScore);

            builder.Property(sr => sr.CsatScore);

            builder.Property(sr => sr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sr => sr.CreatedAt)
                .IsRequired();

            builder.Property(sr => sr.UpdatedAt)
                .IsRequired();

            builder.HasOne(sr => sr.Survey)
                .WithMany()
                .HasForeignKey(sr => sr.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sr => sr.Patient)
                .WithMany()
                .HasForeignKey(sr => sr.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<SurveyQuestionResponse>()
                .WithOne(sqr => sqr.SurveyResponse)
                .HasForeignKey(sqr => sqr.SurveyResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(sr => sr.SurveyId);
            builder.HasIndex(sr => sr.PatientId);
            builder.HasIndex(sr => sr.NpsScore);
            builder.HasIndex(sr => sr.CsatScore);
            builder.HasIndex(sr => sr.TenantId);
        }
    }
}
