using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissGuideConfiguration : IEntityTypeConfiguration<TissGuide>
    {
        public void Configure(EntityTypeBuilder<TissGuide> builder)
        {
            builder.ToTable("TissGuides");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id)
                .ValueGeneratedNever();

            builder.Property(g => g.GuideNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.AuthorizationNumber)
                .HasMaxLength(50);

            builder.Property(g => g.TotalAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.ApprovedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.GlosedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.GlossReason)
                .HasMaxLength(1000);

            builder.Property(g => g.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(g => g.TissBatch)
                .WithMany(b => b.Guides)
                .HasForeignKey(g => g.TissBatchId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.Appointment)
                .WithMany()
                .HasForeignKey(g => g.AppointmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(g => g.PatientHealthInsurance)
                .WithMany()
                .HasForeignKey(g => g.PatientHealthInsuranceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.Procedures)
                .WithOne(p => p.TissGuide)
                .HasForeignKey(p => p.TissGuideId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(g => new { g.TenantId, g.GuideNumber })
                .IsUnique()
                .HasDatabaseName("IX_TissGuides_TenantId_GuideNumber");

            builder.HasIndex(g => new { g.TenantId, g.Status })
                .HasDatabaseName("IX_TissGuides_TenantId_Status");

            builder.HasIndex(g => g.TenantId)
                .HasDatabaseName("IX_TissGuides_TenantId");

            builder.HasIndex(g => g.TissBatchId)
                .HasDatabaseName("IX_TissGuides_TissBatchId");
        }
    }
}
