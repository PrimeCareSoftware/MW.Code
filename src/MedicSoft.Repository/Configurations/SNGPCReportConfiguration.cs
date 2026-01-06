using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SNGPCReportConfiguration : IEntityTypeConfiguration<SNGPCReport>
    {
        public void Configure(EntityTypeBuilder<SNGPCReport> builder)
        {
            builder.ToTable("SNGPCReports");

            builder.HasKey(sr => sr.Id);

            builder.Property(sr => sr.Id)
                .ValueGeneratedNever();

            builder.Property(sr => sr.Month)
                .IsRequired();

            builder.Property(sr => sr.Year)
                .IsRequired();

            builder.Property(sr => sr.ReportPeriodStart)
                .IsRequired();

            builder.Property(sr => sr.ReportPeriodEnd)
                .IsRequired();

            builder.Property(sr => sr.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(sr => sr.GeneratedAt)
                .IsRequired();

            builder.Property(sr => sr.TransmittedAt);

            builder.Property(sr => sr.TransmissionProtocol)
                .HasMaxLength(100);

            builder.Property(sr => sr.XmlContent)
                .HasColumnType("text");

            builder.Property(sr => sr.XmlHash)
                .HasMaxLength(64);

            builder.Property(sr => sr.TotalPrescriptions)
                .IsRequired();

            builder.Property(sr => sr.TotalItems)
                .IsRequired();

            builder.Property(sr => sr.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(sr => sr.LastAttemptAt);

            builder.Property(sr => sr.AttemptCount)
                .IsRequired();

            builder.Property(sr => sr.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(sr => sr.CreatedAt)
                .IsRequired();

            builder.Property(sr => sr.UpdatedAt);

            // Store prescription IDs as JSON array
            builder.Property(sr => sr.PrescriptionIds)
                .HasConversion(
                    v => System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => System.Text.Json.JsonSerializer.Deserialize<List<Guid>>(v, (System.Text.Json.JsonSerializerOptions?)null) ?? new List<Guid>()
                )
                .HasColumnType("jsonb");

            // Indexes
            builder.HasIndex(sr => new { sr.TenantId, sr.Month, sr.Year })
                .IsUnique()
                .HasDatabaseName("IX_SNGPCReports_TenantId_Month_Year");

            builder.HasIndex(sr => new { sr.TenantId, sr.Status })
                .HasDatabaseName("IX_SNGPCReports_TenantId_Status");

            builder.HasIndex(sr => new { sr.TenantId, sr.Year })
                .HasDatabaseName("IX_SNGPCReports_TenantId_Year");

            builder.HasIndex(sr => sr.TransmissionProtocol)
                .HasDatabaseName("IX_SNGPCReports_TransmissionProtocol");
        }
    }
}
