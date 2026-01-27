using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Repository.Configurations
{
    public class SenhaFilaConfiguration : IEntityTypeConfiguration<SenhaFila>
    {
        public void Configure(EntityTypeBuilder<SenhaFila> builder)
        {
            builder.ToTable("SenhasFila");

            builder.HasKey(s => s.Id);

            builder.Property(s => s.NomePaciente)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(s => s.CpfPaciente)
                .HasMaxLength(14);

            builder.Property(s => s.TelefonePaciente)
                .HasMaxLength(20);

            builder.Property(s => s.NumeroSenha)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.DataHoraEntrada)
                .IsRequired();

            builder.Property(s => s.Prioridade)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.MotivoPrioridade)
                .HasMaxLength(200);

            builder.Property(s => s.Status)
                .IsRequired()
                .HasConversion<int>();

            builder.Property(s => s.TentativasChamada)
                .IsRequired();

            builder.Property(s => s.NumeroConsultorio)
                .HasMaxLength(50);

            builder.Property(s => s.TempoEsperaMinutos)
                .IsRequired();

            builder.Property(s => s.TempoAtendimentoMinutos)
                .IsRequired();

            builder.Property(s => s.TenantId)
                .IsRequired()
                .HasMaxLength(100);

            // Relationships
            builder.HasOne(s => s.Fila)
                .WithMany(f => f.Senhas)
                .HasForeignKey(s => s.FilaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(s => s.Paciente)
                .WithMany()
                .HasForeignKey(s => s.PacienteId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(s => s.Medico)
                .WithMany()
                .HasForeignKey(s => s.MedicoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            builder.HasOne(s => s.Agendamento)
                .WithMany()
                .HasForeignKey(s => s.AgendamentoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Indexes
            builder.HasIndex(s => s.FilaId);
            builder.HasIndex(s => s.PacienteId);
            builder.HasIndex(s => s.Status);
            builder.HasIndex(s => s.TenantId);
            builder.HasIndex(s => new { s.FilaId, s.Status });
            builder.HasIndex(s => new { s.FilaId, s.DataHoraEntrada });
            builder.HasIndex(s => new { s.NumeroSenha, s.FilaId });
        }
    }
}
