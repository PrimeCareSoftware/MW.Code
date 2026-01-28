using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class ApuracaoImpostosConfiguration : IEntityTypeConfiguration<ApuracaoImpostos>
    {
        public void Configure(EntityTypeBuilder<ApuracaoImpostos> builder)
        {
            builder.ToTable("ApuracoesImpostos");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .ValueGeneratedNever();

            builder.Property(a => a.ClinicaId)
                .IsRequired();

            builder.Property(a => a.Mes)
                .IsRequired();

            builder.Property(a => a.Ano)
                .IsRequired();

            builder.Property(a => a.DataApuracao)
                .IsRequired();

            // Faturamento
            builder.Property(a => a.FaturamentoBruto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.Deducoes)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Impostos apurados
            builder.Property(a => a.TotalPIS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.TotalCOFINS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.TotalIR)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.TotalCSLL)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.TotalISS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(a => a.TotalINSS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Simples Nacional
            builder.Property(a => a.ReceitaBruta12Meses)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(a => a.AliquotaEfetiva)
                .HasColumnType("decimal(5,2)")
                .IsRequired(false);

            builder.Property(a => a.ValorDAS)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            // Status
            builder.Property(a => a.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(a => a.DataPagamento)
                .IsRequired(false);

            builder.Property(a => a.ComprovantesPagamento)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(a => a.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(a => a.Clinica)
                .WithMany()
                .HasForeignKey(a => a.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasMany(a => a.NotasIncluidas)
                .WithOne()
                .HasForeignKey("ApuracaoImpostosId")
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(a => new { a.ClinicaId, a.Mes, a.Ano })
                .IsUnique()
                .HasDatabaseName("IX_ApuracoesImpostos_ClinicaId_Mes_Ano");

            builder.HasIndex(a => a.Status)
                .HasDatabaseName("IX_ApuracoesImpostos_Status");

            builder.HasIndex(a => a.DataApuracao)
                .HasDatabaseName("IX_ApuracoesImpostos_DataApuracao");

            builder.HasIndex(a => a.TenantId)
                .HasDatabaseName("IX_ApuracoesImpostos_TenantId");
        }
    }
}
