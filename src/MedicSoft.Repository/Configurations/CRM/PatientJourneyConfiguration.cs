using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class PatientJourneyConfiguration : IEntityTypeConfiguration<PatientJourney>
    {
        public void Configure(EntityTypeBuilder<PatientJourney> builder)
        {
            builder.ToTable("PatientJourneys", schema: "crm");

            builder.HasKey(pj => pj.Id);

            builder.Property(pj => pj.Id)
                .ValueGeneratedNever();

            builder.Property(pj => pj.PacienteId)
                .IsRequired();

            builder.Property(pj => pj.CurrentStage)
                .IsRequired();

            builder.Property(pj => pj.TotalTouchpoints)
                .IsRequired();

            builder.Property(pj => pj.LifetimeValue)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(pj => pj.NpsScore)
                .IsRequired();

            builder.Property(pj => pj.SatisfactionScore)
                .IsRequired();

            builder.Property(pj => pj.ChurnRisk)
                .IsRequired();

            builder.Property(pj => pj.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(pj => pj.CreatedAt)
                .IsRequired();

            builder.Property(pj => pj.UpdatedAt)
                .IsRequired();

            // Relationships
            builder.HasOne(pj => pj.Paciente)
                .WithMany()
                .HasForeignKey(pj => pj.PacienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany<JourneyStage>()
                .WithOne()
                .HasForeignKey("PatientJourneyId")
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pj => pj.PacienteId);
            builder.HasIndex(pj => pj.CurrentStage);
            builder.HasIndex(pj => pj.ChurnRisk);
            builder.HasIndex(pj => pj.TenantId);
        }
    }
}
