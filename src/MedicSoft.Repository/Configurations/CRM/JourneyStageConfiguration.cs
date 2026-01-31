using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class JourneyStageConfiguration : IEntityTypeConfiguration<JourneyStage>
    {
        public void Configure(EntityTypeBuilder<JourneyStage> builder)
        {
            builder.ToTable("JourneyStages", schema: "crm");

            builder.HasKey(js => js.Id);

            builder.Property(js => js.Id)
                .ValueGeneratedNever();

            builder.Property(js => js.PatientJourneyId)
                .IsRequired();

            builder.Property(js => js.Stage)
                .IsRequired();

            builder.Property(js => js.EnteredAt)
                .IsRequired();

            builder.Property(js => js.ExitedAt);

            builder.Property(js => js.DurationDays)
                .IsRequired();

            builder.Property(js => js.ExitTrigger)
                .HasMaxLength(500);

            builder.Property(js => js.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(js => js.CreatedAt)
                .IsRequired();

            builder.Property(js => js.UpdatedAt)
                .IsRequired();

            builder.HasOne(js => js.PatientJourney)
                .WithMany(pj => pj.Stages)
                .HasForeignKey(js => js.PatientJourneyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(js => js.Touchpoints)
                .WithOne(pt => pt.JourneyStage)
                .HasForeignKey(pt => pt.JourneyStageId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(js => js.Stage);
            builder.HasIndex(js => js.PatientJourneyId);
            builder.HasIndex(js => js.TenantId);
        }
    }
}
