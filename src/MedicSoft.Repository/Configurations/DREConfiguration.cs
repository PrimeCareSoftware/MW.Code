using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class DREConfiguration : IEntityTypeConfiguration<DRE>
    {
        public void Configure(EntityTypeBuilder<DRE> builder)
        {
            builder.ToTable("DREs");

            builder.HasKey(d => d.Id);

            builder.Property(d => d.Id)
                .ValueGeneratedNever();

            builder.Property(d => d.ClinicaId)
                .IsRequired();

            builder.Property(d => d.PeriodoInicio)
                .IsRequired();

            builder.Property(d => d.PeriodoFim)
                .IsRequired();

            builder.Property(d => d.DataGeracao)
                .IsRequired();

            // Receitas
            builder.Property(d => d.ReceitaBruta)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.Deducoes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.ReceitaLiquida)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Custos
            builder.Property(d => d.CustoServicos)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Lucro Bruto
            builder.Property(d => d.LucroBruto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.MargemBruta)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            // Despesas
            builder.Property(d => d.DespesasOperacionais)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.DespesasAdministrativas)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.DespesasComerciais)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // EBITDA
            builder.Property(d => d.EBITDA)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.MargemEBITDA)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            // Depreciação
            builder.Property(d => d.DepreciacaoAmortizacao)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // EBIT
            builder.Property(d => d.EBIT)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Resultado Financeiro
            builder.Property(d => d.ReceitasFinanceiras)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.DespesasFinanceiras)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.ResultadoFinanceiro)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Lucro
            builder.Property(d => d.LucroAntesIR)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.ImpostoRenda)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.CSLL)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.LucroLiquido)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(d => d.MargemLiquida)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(d => d.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(d => d.Clinica)
                .WithMany()
                .HasForeignKey(d => d.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(d => new { d.ClinicaId, d.PeriodoInicio, d.PeriodoFim })
                .HasDatabaseName("IX_DREs_ClinicaId_Periodo");

            builder.HasIndex(d => d.TenantId)
                .HasDatabaseName("IX_DREs_TenantId");

            builder.HasIndex(d => d.DataGeracao)
                .HasDatabaseName("IX_DREs_DataGeracao");
        }
    }
}
