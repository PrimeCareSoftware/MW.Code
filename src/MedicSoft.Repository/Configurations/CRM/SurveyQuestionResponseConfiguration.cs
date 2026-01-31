using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class SurveyQuestionResponseConfiguration : IEntityTypeConfiguration<SurveyQuestionResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyQuestionResponse> builder)
        {
            builder.ToTable("SurveyQuestionResponses", schema: "crm");

            builder.HasKey(sqr => sqr.Id);

            builder.Property(sqr => sqr.Id)
                .ValueGeneratedNever();

            builder.Property(sqr => sqr.SurveyResponseId)
                .IsRequired();

            builder.Property(sqr => sqr.SurveyQuestionId)
                .IsRequired();

            builder.Property(sqr => sqr.TextAnswer)
                .HasMaxLength(2000);

            builder.Property(sqr => sqr.NumericAnswer);

            builder.Property(sqr => sqr.AnsweredAt)
                .IsRequired();

            builder.Property(sqr => sqr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sqr => sqr.CreatedAt)
                .IsRequired();

            builder.Property(sqr => sqr.UpdatedAt)
                .IsRequired();

            builder.HasOne(sqr => sqr.SurveyResponse)
                .WithMany(sr => sr.QuestionResponses)
                .HasForeignKey(sqr => sqr.SurveyResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(sqr => sqr.SurveyQuestion)
                .WithMany()
                .HasForeignKey(sqr => sqr.SurveyQuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(sqr => sqr.SurveyResponseId);
            builder.HasIndex(sqr => sqr.SurveyQuestionId);
            builder.HasIndex(sqr => sqr.TenantId);
        }
    }
}
