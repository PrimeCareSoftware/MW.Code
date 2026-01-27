using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class TissGlosaConfiguration : IEntityTypeConfiguration<TissGlosa>
    {
        public void Configure(EntityTypeBuilder<TissGlosa> builder)
        {
            builder.ToTable("TissGlosas");

            builder.HasKey(g => g.Id);

            builder.Property(g => g.Id)
                .ValueGeneratedNever();

            builder.Property(g => g.NumeroGuia)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.CodigoGlosa)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(g => g.DescricaoGlosa)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(g => g.ValorGlosado)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.ValorOriginal)
                .HasColumnType("decimal(18,2)");

            builder.Property(g => g.CodigoProcedimento)
                .HasMaxLength(50);

            builder.Property(g => g.NomeProcedimento)
                .HasMaxLength(500);

            builder.Property(g => g.JustificativaRecurso)
                .HasMaxLength(2000);

            builder.Property(g => g.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(g => g.Guide)
                .WithMany()
                .HasForeignKey(g => g.GuideId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.Recursos)
                .WithOne(r => r.Glosa)
                .HasForeignKey(r => r.GlosaId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(g => new { g.TenantId, g.NumeroGuia })
                .HasDatabaseName("IX_TissGlosas_TenantId_NumeroGuia");

            builder.HasIndex(g => new { g.TenantId, g.Status })
                .HasDatabaseName("IX_TissGlosas_TenantId_Status");

            builder.HasIndex(g => new { g.TenantId, g.DataGlosa })
                .HasDatabaseName("IX_TissGlosas_TenantId_DataGlosa");

            builder.HasIndex(g => g.TenantId)
                .HasDatabaseName("IX_TissGlosas_TenantId");

            builder.HasIndex(g => g.GuideId)
                .HasDatabaseName("IX_TissGlosas_GuideId");

            builder.HasIndex(g => new { g.TenantId, g.Tipo })
                .HasDatabaseName("IX_TissGlosas_TenantId_Tipo");
        }
    }
}
