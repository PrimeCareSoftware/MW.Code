using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class PatientTouchpointConfiguration : IEntityTypeConfiguration<PatientTouchpoint>
    {
        public void Configure(EntityTypeBuilder<PatientTouchpoint> builder)
        {
            builder.ToTable("PatientTouchpoints", schema: "crm");

            builder.HasKey(pt => pt.Id);

            builder.Property(pt => pt.Id)
                .ValueGeneratedNever();

            builder.Property(pt => pt.JourneyStageId)
                .IsRequired();

            builder.Property(pt => pt.Timestamp)
                .IsRequired();

            builder.Property(pt => pt.Type)
                .IsRequired();

            builder.Property(pt => pt.Direction)
                .IsRequired();

            builder.Property(pt => pt.Channel)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(pt => pt.SentimentAnalysisId);

            builder.Property(pt => pt.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pt => pt.CreatedAt)
                .IsRequired();

            builder.Property(pt => pt.UpdatedAt)
                .IsRequired();

            builder.HasOne(pt => pt.JourneyStage)
                .WithMany(js => js.Touchpoints)
                .HasForeignKey(pt => pt.JourneyStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pt => pt.Timestamp);
            builder.HasIndex(pt => pt.Type);
            builder.HasIndex(pt => pt.JourneyStageId);
            builder.HasIndex(pt => pt.TenantId);
        }
    }
}
