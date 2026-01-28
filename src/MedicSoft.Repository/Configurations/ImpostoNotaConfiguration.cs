using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Repository.Configurations
{
    public class ImpostoNotaConfiguration : IEntityTypeConfiguration<ImpostoNota>
    {
        public void Configure(EntityTypeBuilder<ImpostoNota> builder)
        {
            builder.ToTable("ImpostosNotas");

            builder.HasKey(i => i.Id);

            builder.Property(i => i.Id)
                .ValueGeneratedNever();

            builder.Property(i => i.NotaFiscalId)
                .IsRequired();

            // Valores base
            builder.Property(i => i.ValorBruto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.ValorDesconto)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Tributos Federais - PIS
            builder.Property(i => i.AliquotaPIS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorPIS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // COFINS
            builder.Property(i => i.AliquotaCOFINS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorCOFINS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // IR
            builder.Property(i => i.AliquotaIR)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorIR)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // CSLL
            builder.Property(i => i.AliquotaCSLL)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorCSLL)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            // Tributo Municipal - ISS
            builder.Property(i => i.AliquotaISS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorISS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.ISSRetido)
                .IsRequired();

            builder.Property(i => i.CodigoServicoMunicipal)
                .HasMaxLength(20)
                .IsRequired(false);

            // INSS
            builder.Property(i => i.AliquotaINSS)
                .HasColumnType("decimal(5,2)")
                .IsRequired();

            builder.Property(i => i.ValorINSS)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(i => i.INSSRetido)
                .IsRequired();

            // Metadados
            builder.Property(i => i.DataCalculo)
                .IsRequired();

            builder.Property(i => i.RegimeTributario)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(i => i.TenantId)
                .HasMaxLength(100)
                .IsRequired();

            // Relationships
            builder.HasOne(i => i.NotaFiscal)
                .WithOne()
                .HasForeignKey<ImpostoNota>(i => i.NotaFiscalId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();

            // Indexes
            builder.HasIndex(i => i.NotaFiscalId)
                .IsUnique()
                .HasDatabaseName("IX_ImpostosNotas_NotaFiscalId");

            builder.HasIndex(i => i.DataCalculo)
                .HasDatabaseName("IX_ImpostosNotas_DataCalculo");

            builder.HasIndex(i => i.RegimeTributario)
                .HasDatabaseName("IX_ImpostosNotas_RegimeTributario");

            builder.HasIndex(i => i.TenantId)
                .HasDatabaseName("IX_ImpostosNotas_TenantId");
        }
    }
}
