using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class LancamentoContabilConfiguration : IEntityTypeConfiguration<LancamentoContabil>
    {
        public void Configure(EntityTypeBuilder<LancamentoContabil> builder)
        {
            builder.ToTable("LancamentosContabeis");

            builder.HasKey(l => l.Id);

            builder.Property(l => l.Id)
                .ValueGeneratedNever();

            builder.Property(l => l.ClinicaId)
                .IsRequired();

            builder.Property(l => l.PlanoContasId)
                .IsRequired();

            builder.Property(l => l.DataLancamento)
                .IsRequired();

            builder.Property(l => l.Tipo)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(l => l.Valor)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(l => l.Historico)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(l => l.Origem)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(l => l.DocumentoOrigemId)
                .IsRequired(false);

            builder.Property(l => l.NumeroDocumento)
                .HasMaxLength(50)
                .IsRequired(false);

            builder.Property(l => l.LoteId)
                .IsRequired(false);

            builder.Property(l => l.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(l => l.Clinica)
                .WithMany()
                .HasForeignKey(l => l.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            builder.HasOne(l => l.Conta)
                .WithMany(p => p.Lancamentos)
                .HasForeignKey(l => l.PlanoContasId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(l => l.PlanoContasId)
                .HasDatabaseName("IX_LancamentosContabeis_PlanoContasId");

            builder.HasIndex(l => new { l.ClinicaId, l.DataLancamento })
                .HasDatabaseName("IX_LancamentosContabeis_ClinicaId_DataLancamento");

            builder.HasIndex(l => l.LoteId)
                .HasDatabaseName("IX_LancamentosContabeis_LoteId");

            builder.HasIndex(l => l.DocumentoOrigemId)
                .HasDatabaseName("IX_LancamentosContabeis_DocumentoOrigemId");

            builder.HasIndex(l => l.Tipo)
                .HasDatabaseName("IX_LancamentosContabeis_Tipo");

            builder.HasIndex(l => l.Origem)
                .HasDatabaseName("IX_LancamentosContabeis_Origem");

            builder.HasIndex(l => l.TenantId)
                .HasDatabaseName("IX_LancamentosContabeis_TenantId");
        }
    }
}
