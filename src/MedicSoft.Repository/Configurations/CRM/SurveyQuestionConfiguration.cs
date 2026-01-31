using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class SurveyQuestionConfiguration : IEntityTypeConfiguration<SurveyQuestion>
    {
        public void Configure(EntityTypeBuilder<SurveyQuestion> builder)
        {
            builder.ToTable("SurveyQuestions", schema: "crm");

            builder.HasKey(sq => sq.Id);

            builder.Property(sq => sq.Id)
                .ValueGeneratedNever();

            builder.Property(sq => sq.SurveyId)
                .IsRequired();

            builder.Property(sq => sq.QuestionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(sq => sq.Type)
                .IsRequired();

            builder.Property(sq => sq.Order)
                .IsRequired();

            builder.Property(sq => sq.IsRequired)
                .IsRequired();

            builder.Property(sq => sq.OptionsJson)
                .HasMaxLength(2000);

            builder.Property(sq => sq.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sq => sq.CreatedAt)
                .IsRequired();

            builder.Property(sq => sq.UpdatedAt)
                .IsRequired();

            builder.HasOne(sq => sq.Survey)
                .WithMany(s => s.Questions)
                .HasForeignKey(sq => sq.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(sq => sq.SurveyId);
            builder.HasIndex(sq => sq.TenantId);
        }
    }
}
