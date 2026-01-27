using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class FilaEsperaConfiguration : IEntityTypeConfiguration<FilaEspera>
    {
        public void Configure(EntityTypeBuilder<FilaEspera> builder)
        {
            builder.ToTable("FilasEspera");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.Nome)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(f => f.Tipo)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(f => f.Ativa)
                .IsRequired();

            builder.Property(f => f.TempoMedioAtendimento)
                .IsRequired();

            builder.Property(f => f.UsaPrioridade)
                .IsRequired();

            builder.Property(f => f.UsaAgendamento)
                .IsRequired();

            builder.Property(f => f.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(f => f.Clinica)
                .WithMany()
                .HasForeignKey(f => f.ClinicaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(f => f.Senhas)
                .WithOne(s => s.Fila)
                .HasForeignKey(s => s.FilaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(f => f.ClinicaId);
            builder.HasIndex(f => f.TenantId);
            builder.HasIndex(f => new { f.ClinicaId, f.Ativa });
        }
    }
}
