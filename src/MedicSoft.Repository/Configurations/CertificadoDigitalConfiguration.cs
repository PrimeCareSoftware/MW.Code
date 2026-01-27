using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Entity Framework configuration for CertificadoDigital entity.
    /// </summary>
    public class CertificadoDigitalConfiguration : IEntityTypeConfiguration<CertificadoDigital>
    {
        public void Configure(EntityTypeBuilder<CertificadoDigital> builder)
        {
            builder.ToTable("CertificadosDigitais");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedOnAdd();

            builder.Property(c => c.NumeroCertificado)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.SubjectName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IssuerName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.Thumbprint)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(c => c.CertificadoA1Criptografado)
                .HasColumnType("bytea");

            builder.Property(c => c.ChavePrivadaA1Criptografada)
                .HasColumnType("bytea");

            builder.Property(c => c.DataEmissao)
                .IsRequired();

            builder.Property(c => c.DataExpiracao)
                .IsRequired();

            builder.Property(c => c.Valido)
                .IsRequired();

            builder.Property(c => c.DataCadastro)
                .IsRequired();

            builder.Property(c => c.TotalAssinaturas)
                .IsRequired()
                .HasDefaultValue(0);

            builder.Property(c => c.TenantId)
                .IsRequired()
                .HasMaxLength(50);

            // Relationships
            builder.HasOne(c => c.Medico)
                .WithMany()
                .HasForeignKey(c => c.MedicoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(c => c.MedicoId);
            builder.HasIndex(c => c.Thumbprint).IsUnique();
            builder.HasIndex(c => c.TenantId);
        }
    }
}
