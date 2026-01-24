using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SngpcTransmissionConfiguration : IEntityTypeConfiguration<SngpcTransmission>
    {
        public void Configure(EntityTypeBuilder<SngpcTransmission> builder)
        {
            builder.ToTable("SngpcTransmissions");

            builder.HasKey(st => st.Id);

            builder.Property(st => st.Id)
                .ValueGeneratedNever();

            builder.Property(st => st.SNGPCReportId)
                .IsRequired();

            builder.Property(st => st.AttemptNumber)
                .IsRequired();

            builder.Property(st => st.AttemptedAt)
                .IsRequired();

            builder.Property(st => st.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(st => st.ProtocolNumber)
                .HasMaxLength(100);

            builder.Property(st => st.AnvisaResponse)
                .HasColumnType("text");

            builder.Property(st => st.ErrorMessage)
                .HasMaxLength(1000);

            builder.Property(st => st.ErrorCode)
                .HasMaxLength(50);

            builder.Property(st => st.TransmissionMethod)
                .HasMaxLength(50);

            builder.Property(st => st.EndpointUrl)
                .HasMaxLength(500);

            builder.Property(st => st.HttpStatusCode);

            builder.Property(st => st.ResponseTimeMs);

            builder.Property(st => st.XmlHash)
                .HasMaxLength(64);

            builder.Property(st => st.XmlSizeBytes);

            builder.Property(st => st.InitiatedByUserId);

            builder.Property(st => st.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(st => st.CreatedAt)
                .IsRequired();

            builder.Property(st => st.UpdatedAt);

            // Relationships
            builder.HasOne(st => st.Report)
                .WithMany()
                .HasForeignKey(st => st.SNGPCReportId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(st => st.InitiatedBy)
                .WithMany()
                .HasForeignKey(st => st.InitiatedByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(st => new { st.TenantId, st.SNGPCReportId, st.AttemptNumber })
                .HasDatabaseName("IX_SngpcTransmissions_TenantId_ReportId_Attempt");

            builder.HasIndex(st => new { st.TenantId, st.Status })
                .HasDatabaseName("IX_SngpcTransmissions_TenantId_Status");

            builder.HasIndex(st => new { st.TenantId, st.AttemptedAt })
                .HasDatabaseName("IX_SngpcTransmissions_TenantId_AttemptedAt");

            builder.HasIndex(st => st.ProtocolNumber)
                .HasDatabaseName("IX_SngpcTransmissions_ProtocolNumber");

            builder.HasIndex(st => new { st.SNGPCReportId, st.Status })
                .HasDatabaseName("IX_SngpcTransmissions_ReportId_Status");
        }
    }
}
