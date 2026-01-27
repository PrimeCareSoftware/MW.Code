using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Repository.Configurations.CRM
{
    public class ChurnPredictionConfiguration : IEntityTypeConfiguration<ChurnPrediction>
    {
        public void Configure(EntityTypeBuilder<ChurnPrediction> builder)
        {
            builder.ToTable("ChurnPredictions", schema: "crm");

            builder.HasKey(cp => cp.Id);

            builder.Property(cp => cp.Id)
                .ValueGeneratedNever();

            builder.Property(cp => cp.PatientId)
                .IsRequired();

            builder.Property(cp => cp.ChurnProbability)
                .IsRequired();

            builder.Property(cp => cp.RiskLevel)
                .IsRequired();

            builder.Property(cp => cp.PredictedAt)
                .IsRequired();

            // Features
            builder.Property(cp => cp.DaysSinceLastVisit)
                .IsRequired();

            builder.Property(cp => cp.TotalVisits)
                .IsRequired();

            builder.Property(cp => cp.LifetimeValue)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(cp => cp.AverageSatisfactionScore)
                .IsRequired();

            builder.Property(cp => cp.ComplaintsCount)
                .IsRequired();

            builder.Property(cp => cp.NoShowCount)
                .IsRequired();

            builder.Property(cp => cp.CancelledAppointmentsCount)
                .IsRequired();

            // Store collections as JSON
            builder.Property<List<string>>("_riskFactors")
                .HasColumnName("RiskFactors")
                .HasColumnType("jsonb");

            builder.Property<List<string>>("_recommendedActions")
                .HasColumnName("RecommendedActions")
                .HasColumnType("jsonb");

            builder.Property(cp => cp.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(cp => cp.CreatedAt)
                .IsRequired();

            builder.Property(cp => cp.UpdatedAt)
                .IsRequired();

            builder.HasOne(cp => cp.Patient)
                .WithMany()
                .HasForeignKey(cp => cp.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(cp => cp.PatientId);
            builder.HasIndex(cp => cp.RiskLevel);
            builder.HasIndex(cp => cp.PredictedAt);
            builder.HasIndex(cp => cp.TenantId);
        }
    }
}
