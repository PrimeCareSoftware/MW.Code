using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class PlanoContasConfiguration : IEntityTypeConfiguration<PlanoContas>
    {
        public void Configure(EntityTypeBuilder<PlanoContas> builder)
        {
            builder.ToTable("PlanoContas");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.ClinicaId)
                .IsRequired();

            builder.Property(p => p.Codigo)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.Nome)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(p => p.Tipo)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.Natureza)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(p => p.ContaPaiId)
                .IsRequired(false);

            builder.Property(p => p.Analitica)
                .IsRequired();

            builder.Property(p => p.Ativa)
                .IsRequired();

            builder.Property(p => p.Nivel)
                .IsRequired();

            builder.Property(p => p.Observacoes)
                .HasMaxLength(1000)
                .IsRequired(false);

            builder.Property(p => p.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(p => p.Clinica)
                .WithMany()
                .HasForeignKey(p => p.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Self-referencing relationship for hierarchy
            builder.HasOne(p => p.ContaPai)
                .WithMany(p => p.SubContas)
                .HasForeignKey(p => p.ContaPaiId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasMany(p => p.Lancamentos)
                .WithOne(l => l.Conta)
                .HasForeignKey(l => l.PlanoContasId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(p => new { p.ClinicaId, p.Codigo })
                .IsUnique()
                .HasDatabaseName("IX_PlanoContas_ClinicaId_Codigo");

            builder.HasIndex(p => p.ContaPaiId)
                .HasDatabaseName("IX_PlanoContas_ContaPaiId");

            builder.HasIndex(p => p.Tipo)
                .HasDatabaseName("IX_PlanoContas_Tipo");

            builder.HasIndex(p => new { p.ClinicaId, p.Ativa })
                .HasDatabaseName("IX_PlanoContas_ClinicaId_Ativa");

            builder.HasIndex(p => new { p.ClinicaId, p.Analitica })
                .HasDatabaseName("IX_PlanoContas_ClinicaId_Analitica");

            builder.HasIndex(p => p.TenantId)
                .HasDatabaseName("IX_PlanoContas_TenantId");
        }
    }
}
