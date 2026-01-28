using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class ConfiguracaoFiscalConfiguration : IEntityTypeConfiguration<ConfiguracaoFiscal>
    {
        public void Configure(EntityTypeBuilder<ConfiguracaoFiscal> builder)
        {
            builder.ToTable("ConfiguracoesFiscais");

            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .ValueGeneratedNever();

            builder.Property(c => c.ClinicaId)
                .IsRequired();

            // Regime tributário
            builder.Property(c => c.Regime)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(c => c.VigenciaInicio)
                .IsRequired();

            builder.Property(c => c.VigenciaFim)
                .IsRequired(false);

            // Simples Nacional
            builder.Property(c => c.OptanteSimplesNacional)
                .IsRequired();

            builder.Property(c => c.AnexoSimples)
                .HasConversion<int?>()
                .IsRequired(false);

            builder.Property(c => c.FatorR)
                .HasColumnType("decimal(5,2)")
                .IsRequired(false);

            // Alíquotas
            builder.Property(c => c.AliquotaISS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.AliquotaPIS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.AliquotaCOFINS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.AliquotaIR)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(c => c.AliquotaCSLL)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            // INSS
            builder.Property(c => c.RetemINSS)
                .IsRequired();

            builder.Property(c => c.AliquotaINSS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            // Configurações específicas
            builder.Property(c => c.CodigoServico)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.CNAE)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(c => c.InscricaoMunicipal)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(c => c.ISS_Retido)
                .IsRequired();

            builder.Property(c => c.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(c => c.Clinica)
                .WithMany()
                .HasForeignKey(c => c.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(c => new { c.ClinicaId, c.VigenciaInicio })
                .HasDatabaseName("IX_ConfiguracoesFiscais_ClinicaId_VigenciaInicio");

            builder.HasIndex(c => c.TenantId)
                .HasDatabaseName("IX_ConfiguracoesFiscais_TenantId");

            builder.HasIndex(c => c.Regime)
                .HasDatabaseName("IX_ConfiguracoesFiscais_Regime");
        }
    }
}
