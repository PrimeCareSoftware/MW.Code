using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class SurveyConfiguration : IEntityTypeConfiguration<Survey>
    {
        public void Configure(EntityTypeBuilder<Survey> builder)
        {
            builder.ToTable("Surveys", schema: "crm");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.Id)
                .ValueGeneratedNever();

            builder.Property(s => s.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(s => s.Type)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .IsRequired();

            builder.Property(s => s.TriggerStage);

            builder.Property(s => s.TriggerEvent)
                .HasMaxLength(200);

            builder.Property(s => s.DelayHours);

            builder.Property(s => s.AverageScore)
                .IsRequired();

            builder.Property(s => s.TotalResponses)
                .IsRequired();

            builder.Property(s => s.ResponseRate)
                .IsRequired();

            builder.Property(s => s.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.CreatedAt)
                .IsRequired();

            builder.Property(s => s.UpdatedAt)
                .IsRequired();

            builder.HasMany<SurveyQuestion>()
                .WithOne()
                .HasForeignKey("SurveyId")
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany<SurveyResponse>()
                .WithOne()
                .HasForeignKey("SurveyId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(s => s.Type);
            builder.HasIndex(s => s.IsActive);
            builder.HasIndex(s => s.TriggerStage);
            builder.HasIndex(s => s.TenantId);
        }
    }
}
