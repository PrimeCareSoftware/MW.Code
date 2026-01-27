using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework configuration for AssinaturaDigital entity.
    /// </summary>
    public class AssinaturaDigitalConfiguration : IEntityTypeConfiguration<AssinaturaDigital>
    {
        public void Configure(EntityTypeBuilder<AssinaturaDigital> builder)
        {
            builder.ToTable("AssinaturasDigitais");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedOnAdd();

            builder.Property(a => a.TipoDocumento)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(a => a.AssinaturaDigitalBytes)
                .IsRequired()
                .HasColumnType("bytea");

            builder.Property(a => a.HashDocumento)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.DataHoraAssinatura)
                .IsRequired();

            builder.Property(a => a.TemTimestamp)
                .IsRequired();

            builder.Property(a => a.TimestampBytes)
                .HasColumnType("bytea");

            builder.Property(a => a.Valida)
                .IsRequired();

            builder.Property(a => a.LocalAssinatura)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.IpAssinatura)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.TenantId)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(a => a.Medico)
                .WithMany()
                .HasForeignKey(a => a.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.Certificado)
                .WithMany()
                .HasForeignKey(a => a.CertificadoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(a => a.DocumentoId);
            builder.HasIndex(a => a.MedicoId);
            builder.HasIndex(a => a.CertificadoId);
            builder.HasIndex(a => new { a.DocumentoId, a.TipoDocumento });
            builder.HasIndex(a => a.TenantId);
        }
    }
}
