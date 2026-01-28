using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    /// <summary>
    /// Configuração EF Core para ConfiguracaoIntegracao
    /// </summary>
    public class ConfiguracaoIntegracaoConfiguration : IEntityTypeConfiguration<ConfiguracaoIntegracao>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoIntegracao> builder)
        {
            builder.ToTable("ConfiguracoesIntegracao");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.ClinicaId)
                .IsRequired();

            builder.Property(e => e.Provedor)
                .IsRequired();

            builder.Property(e => e.Ativa)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(e => e.ApiUrl)
                .HasMaxLength(500);

            builder.Property(e => e.ApiKey)
                .HasMaxLength(500);

            builder.Property(e => e.ClientId)
                .HasMaxLength(500);

            builder.Property(e => e.ClientSecret)
                .HasMaxLength(500);

            builder.Property(e => e.AccessToken)
                .HasMaxLength(2000);

            builder.Property(e => e.RefreshToken)
                .HasMaxLength(2000);

            builder.Property(e => e.CodigoEmpresa)
                .HasMaxLength(100);

            builder.Property(e => e.CodigoFilial)
                .HasMaxLength(100);

            builder.Property(e => e.ConfiguracoesAdicionais)
                .HasMaxLength(4000);

            builder.Property(e => e.UltimoErro)
                .HasMaxLength(2000);

            builder.Property(e => e.TentativasErro)
                .HasDefaultValue(0);

            // Índices
            builder.HasIndex(e => e.ClinicaId);
            builder.HasIndex(e => new { e.ClinicaId, e.Ativa });

            // Relacionamentos
            builder.HasOne(e => e.Clinica)
                .WithMany()
                .HasForeignKey(e => e.ClinicaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
