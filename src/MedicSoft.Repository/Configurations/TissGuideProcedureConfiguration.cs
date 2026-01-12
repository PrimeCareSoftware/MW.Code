using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissGuideProcedureConfiguration : IEntityTypeConfiguration<TissGuideProcedure>
    {
        public void Configure(EntityTypeBuilder<TissGuideProcedure> builder)
        {
            builder.ToTable("TissGuideProcedures");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.ProcedureCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(p => p.ProcedureDescription)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.UnitPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.TotalPrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.ApprovedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.GlosedAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(p => p.GlossReason)
                .HasMaxLength(1000);

            builder.Property(p => p.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(p => p.TissGuide)
                .WithMany(g => g.Procedures)
                .HasForeignKey(p => p.TissGuideId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_TissGuideProcedures_TenantId");

            builder.HasIndex(p => p.TissGuideId)
                .HasDatabaseName("IX_TissGuideProcedures_TissGuideId");

            builder.HasIndex(p => p.ProcedureCode)
                .HasDatabaseName("IX_TissGuideProcedures_ProcedureCode");
        }
    }
}
