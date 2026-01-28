using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class BalancoPatrimonialConfiguration : IEntityTypeConfiguration<BalancoPatrimonial>
    {
        public void Configure(EntityTypeBuilder<BalancoPatrimonial> builder)
        {
            builder.ToTable("BalancosPatrimoniais");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.ClinicaId)
                .IsRequired();

            builder.Property(b => b.DataReferencia)
                .IsRequired();

            builder.Property(b => b.DataGeracao)
                .IsRequired();

            // ATIVO CIRCULANTE
            builder.Property(b => b.AtivoCirculante)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.DisponibilidadesCaixa)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.ContasReceber)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Estoques)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.OutrosAtivosCirculantes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // ATIVO NÃO CIRCULANTE
            builder.Property(b => b.AtivoNaoCirculante)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.AtivoRealizavelLongoPrazo)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Investimentos)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Imobilizado)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.DepreciacaoAcumulada)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.Intangivel)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.AmortizacaoAcumulada)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // TOTAL DO ATIVO
            builder.Property(b => b.TotalAtivo)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // PASSIVO CIRCULANTE
            builder.Property(b => b.PassivoCirculante)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.FornecedoresPagar)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.ObrigacoesTrabalhistas)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.ObrigacoesTributarias)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.EmprestimosFinanciamentos)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.OutrosPassivosCirculantes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // PASSIVO NÃO CIRCULANTE
            builder.Property(b => b.PassivoNaoCirculante)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.EmprestimosLongoPrazo)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.OutrosPassivosNaoCirculantes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // PATRIMÔNIO LÍQUIDO
            builder.Property(b => b.PatrimonioLiquido)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.CapitalSocial)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.ReservasCapital)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.ReservasLucros)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.LucrosAcumulados)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.PrejuizosAcumulados)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // TOTAL DO PASSIVO
            builder.Property(b => b.TotalPassivo)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(b => b.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(b => b.Clinica)
                .WithMany()
                .HasForeignKey(b => b.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(b => new { b.ClinicaId, b.DataReferencia })
                .HasDatabaseName("IX_BalancosPatrimoniais_ClinicaId_DataReferencia")
                .IsUnique();

            builder.HasIndex(b => b.TenantId)
                .HasDatabaseName("IX_BalancosPatrimoniais_TenantId");

            builder.HasIndex(b => b.DataGeracao)
                .HasDatabaseName("IX_BalancosPatrimoniais_DataGeracao");
        }
    }
}
