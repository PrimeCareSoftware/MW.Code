using System;

namespace MedicSoft.Application.DTOs
{
    public class FilaEsperaDto
    {
        public Guid Id { get; set; }
        public Guid ClinicaId { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Ativa { get; set; }
        public int TempoMedioAtendimento { get; set; }
        public bool UsaPrioridade { get; set; }
        public bool UsaAgendamento { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class SenhaFilaDto
    {
        public Guid Id { get; set; }
        public Guid FilaId { get; set; }
        public Guid? PacienteId { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public string? CpfPaciente { get; set; }
        public string? TelefonePaciente { get; set; }
        public string NumeroSenha { get; set; } = string.Empty;
        public DateTime DataHoraEntrada { get; set; }
        public DateTime? DataHoraChamada { get; set; }
        public DateTime? DataHoraAtendimento { get; set; }
        public DateTime? DataHoraSaida { get; set; }
        public string Prioridade { get; set; } = string.Empty;
        public string? MotivoPrioridade { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TentativasChamada { get; set; }
        public Guid? MedicoId { get; set; }
        public string? NomeMedico { get; set; }
        public Guid? EspecialidadeId { get; set; }
        public string? NumeroConsultorio { get; set; }
        public Guid? AgendamentoId { get; set; }
        public int TempoEsperaMinutos { get; set; }
        public int TempoAtendimentoMinutos { get; set; }
        public int PosicaoNaFila { get; set; }
        public int TempoEstimadoEspera { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class GerarSenhaRequest
    {
        public Guid FilaId { get; set; }
        public Guid? PacienteId { get; set; }
        public string NomePaciente { get; set; } = string.Empty;
        public string? Cpf { get; set; }
        public string? Telefone { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool IsGestante { get; set; }
        public bool IsDeficiente { get; set; }
        public Guid? EspecialidadeId { get; set; }
        public Guid? AgendamentoId { get; set; }
    }

    public class ChamarSenhaRequest
    {
        public Guid FilaId { get; set; }
        public Guid? MedicoId { get; set; }
        public string? NumeroConsultorio { get; set; }
    }

    public class FilaSummaryDto
    {
        public Guid FilaId { get; set; }
        public string NomeFila { get; set; } = string.Empty;
        public int TotalAguardando { get; set; }
        public int TotalChamando { get; set; }
        public int TotalEmAtendimento { get; set; }
        public int TempoMedioEsperaMinutos { get; set; }
        public List<SenhaFilaDto> Senhas { get; set; } = new List<SenhaFilaDto>();
    }

    public class FilaMetricsDto
    {
        public DateTime Data { get; set; }
        public int TotalAtendimentos { get; set; }
        public double TempoMedioEspera { get; set; }
        public double TempoMedioAtendimento { get; set; }
        public double TaxaNaoComparecimento { get; set; }
        public TimeSpan? HorarioPico { get; set; }
        public Dictionary<string, int> AtendimentosPorPrioridade { get; set; } = new Dictionary<string, int>();
    }
}
