using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissBatchConfiguration : IEntityTypeConfiguration<TissBatch>
    {
        public void Configure(EntityTypeBuilder<TissBatch> builder)
        {
            builder.ToTable("TissBatches");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.BatchNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(b => b.XmlFileName)
                .HasMaxLength(500);

            builder.Property(b => b.XmlFilePath)
                .HasMaxLength(1000);

            builder.Property(b => b.ProtocolNumber)
                .HasMaxLength(50);

            builder.Property(b => b.ResponseXmlFileName)
                .HasMaxLength(500);

            builder.Property(b => b.ApprovedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.GlosedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(b => b.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(b => b.Clinic)
                .WithMany()
                .HasForeignKey(b => b.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.Operator)
                .WithMany()
                .HasForeignKey(b => b.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Guides)
                .WithOne(g => g.TissBatch)
                .HasForeignKey(g => g.TissBatchId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => new { b.TenantId, b.BatchNumber })
                .IsUnique()
                .HasDatabaseName("IX_TissBatches_TenantId_BatchNumber");

            builder.HasIndex(b => new { b.TenantId, b.Status })
                .HasDatabaseName("IX_TissBatches_TenantId_Status");

            builder.HasIndex(b => b.TenantId)
                .HasDatabaseName("IX_TissBatches_TenantId");
        }
    }
}
