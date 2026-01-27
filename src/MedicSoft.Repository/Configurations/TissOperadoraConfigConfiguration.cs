using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissOperadoraConfigConfiguration : IEntityTypeConfiguration<TissOperadoraConfig>
    {
        public void Configure(EntityTypeBuilder<TissOperadoraConfig> builder)
        {
            builder.ToTable("TissOperadoraConfigs");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.WebServiceUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Usuario)
                .HasMaxLength(200);

            builder.Property(c => c.SenhaEncriptada)
                .HasMaxLength(500);

            builder.Property(c => c.CertificadoDigitalPath)
                .HasMaxLength(500);

            builder.Property(c => c.MapeamentoTabelasJson)
                .HasColumnType("text");

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(c => c.Operator)
                .WithMany()
                .HasForeignKey(c => c.OperatorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(c => new { c.TenantId, c.OperatorId })
                .IsUnique()
                .HasDatabaseName("IX_TissOperadoraConfigs_TenantId_OperatorId");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_TissOperadoraConfigs_TenantId");

            builder.HasIndex(c => c.OperatorId)
                .HasDatabaseName("IX_TissOperadoraConfigs_OperatorId");
        }
    }
}
