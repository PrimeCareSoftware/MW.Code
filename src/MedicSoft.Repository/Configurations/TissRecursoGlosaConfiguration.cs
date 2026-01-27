using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissRecursoGlosaConfiguration : IEntityTypeConfiguration<TissRecursoGlosa>
    {
        public void Configure(EntityTypeBuilder<TissRecursoGlosa> builder)
        {
            builder.ToTable("TissRecursosGlosa");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .ValueGeneratedNever();

            builder.Property(r => r.Justificativa)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(r => r.JustificativaOperadora)
                .HasMaxLength(2000);

            builder.Property(r => r.ValorDeferido)
                .HasColumnType("decimal(18,2)");

            builder.Property(r => r.AnexosJson)
                .HasColumnType("text");

            builder.Property(r => r.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(r => r.Glosa)
                .WithMany(g => g.Recursos)
                .HasForeignKey(r => r.GlosaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(r => new { r.TenantId, r.DataEnvio })
                .HasDatabaseName("IX_TissRecursosGlosa_TenantId_DataEnvio");

            builder.HasIndex(r => r.TenantId)
                .HasDatabaseName("IX_TissRecursosGlosa_TenantId");

            builder.HasIndex(r => r.GlosaId)
                .HasDatabaseName("IX_TissRecursosGlosa_GlosaId");

            builder.HasIndex(r => new { r.TenantId, r.Resultado })
                .HasDatabaseName("IX_TissRecursosGlosa_TenantId_Resultado");
        }
    }
}
