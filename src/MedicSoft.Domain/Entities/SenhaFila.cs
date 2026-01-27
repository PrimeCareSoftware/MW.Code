using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum PrioridadeAtendimento
    {
        Normal = 0,
        Idoso = 1,          // +60 anos
        Gestante = 2,
        Deficiente = 3,
        Crianca = 4,        // < 2 anos
        Urgencia = 5
    }

    public enum StatusSenha
    {
        Aguardando = 1,
        Chamando = 2,
        EmAtendimento = 3,
        Atendido = 4,
        NaoCompareceu = 5,
        Cancelado = 6
    }

    /// <summary>
    /// Representa uma senha gerada na fila de espera
    /// </summary>
    public class SenhaFila : BaseEntity
    {
        public Guid FilaId { get; private set; }
        
        // Dados do paciente
        public Guid? PacienteId { get; private set; }
        public string NomePaciente { get; private set; }
        public string? CpfPaciente { get; private set; }
        public string? TelefonePaciente { get; private set; }
        
        // Dados da senha
        public string NumeroSenha { get; private set; }
        public DateTime DataHoraEntrada { get; private set; }
        public DateTime? DataHoraChamada { get; private set; }
        public DateTime? DataHoraAtendimento { get; private set; }
        public DateTime? DataHoraSaida { get; private set; }
        
        // Prioridade
        public PrioridadeAtendimento Prioridade { get; private set; }
        public string? MotivoPrioridade { get; private set; }
        
        // Status
        public StatusSenha Status { get; private set; }
        public int TentativasChamada { get; private set; }
        
        // Atendimento
        public Guid? MedicoId { get; private set; }
        public Guid? EspecialidadeId { get; private set; }
        public Guid? ConsultorioId { get; private set; }
        public string? NumeroConsultorio { get; private set; }
        
        // Agendamento vinculado
        public Guid? AgendamentoId { get; private set; }
        
        // Métricas
        public int TempoEsperaMinutos { get; private set; }
        public int TempoAtendimentoMinutos { get; private set; }

        // Navigation properties
        public FilaEspera Fila { get; private set; } = null!;
        public Patient? Paciente { get; private set; }
        public User? Medico { get; private set; }
        public Appointment? Agendamento { get; private set; }

        private SenhaFila()
        {
            // EF Core constructor
            NomePaciente = null!;
            NumeroSenha = null!;
        }

        public SenhaFila(
            Guid filaId,
            string nomePaciente,
            string numeroSenha,
            PrioridadeAtendimento prioridade,
            string tenantId,
            Guid? pacienteId = null,
            string? cpfPaciente = null,
            string? telefonePaciente = null,
            Guid? agendamentoId = null,
            Guid? especialidadeId = null) : base(tenantId)
        {
            if (filaId == Guid.Empty)
                throw new ArgumentException("O ID da fila não pode estar vazio", nameof(filaId));
            
            if (string.IsNullOrWhiteSpace(nomePaciente))
                throw new ArgumentException("O nome do paciente não pode estar vazio", nameof(nomePaciente));
            
            if (string.IsNullOrWhiteSpace(numeroSenha))
                throw new ArgumentException("O número da senha não pode estar vazio", nameof(numeroSenha));

            FilaId = filaId;
            PacienteId = pacienteId;
            NomePaciente = nomePaciente.Trim();
            CpfPaciente = cpfPaciente?.Trim();
            TelefonePaciente = telefonePaciente?.Trim();
            NumeroSenha = numeroSenha.Trim();
            DataHoraEntrada = DateTime.UtcNow;
            Prioridade = prioridade;
            MotivoPrioridade = ObterMotivoPrioridade(prioridade);
            Status = StatusSenha.Aguardando;
            TentativasChamada = 0;
            AgendamentoId = agendamentoId;
            EspecialidadeId = especialidadeId;
        }

        private static string ObterMotivoPrioridade(PrioridadeAtendimento prioridade)
        {
            return prioridade switch
            {
                PrioridadeAtendimento.Idoso => "Idoso (+60 anos)",
                PrioridadeAtendimento.Gestante => "Gestante",
                PrioridadeAtendimento.Deficiente => "Pessoa com deficiência",
                PrioridadeAtendimento.Crianca => "Criança (< 2 anos)",
                PrioridadeAtendimento.Urgencia => "Urgência",
                _ => "Atendimento normal"
            };
        }

        public void Chamar(Guid? medicoId = null, string? numeroConsultorio = null)
        {
            if (Status != StatusSenha.Aguardando)
                throw new InvalidOperationException("Apenas senhas aguardando podem ser chamadas");

            Status = StatusSenha.Chamando;
            DataHoraChamada = DateTime.UtcNow;
            TentativasChamada++;
            MedicoId = medicoId;
            NumeroConsultorio = numeroConsultorio;
            UpdateTimestamp();
        }

        public void IniciarAtendimento()
        {
            if (Status != StatusSenha.Chamando)
                throw new InvalidOperationException("Apenas senhas em chamada podem iniciar atendimento");

            Status = StatusSenha.EmAtendimento;
            DataHoraAtendimento = DateTime.UtcNow;
            
            if (DataHoraChamada.HasValue)
            {
                TempoEsperaMinutos = (int)(DataHoraAtendimento.Value - DataHoraEntrada).TotalMinutes;
            }
            
            UpdateTimestamp();
        }

        public void FinalizarAtendimento()
        {
            if (Status != StatusSenha.EmAtendimento)
                throw new InvalidOperationException("Apenas senhas em atendimento podem ser finalizadas");

            Status = StatusSenha.Atendido;
            DataHoraSaida = DateTime.UtcNow;
            
            if (DataHoraAtendimento.HasValue)
            {
                TempoAtendimentoMinutos = (int)(DataHoraSaida.Value - DataHoraAtendimento.Value).TotalMinutes;
            }
            
            UpdateTimestamp();
        }

        public void MarcarNaoCompareceu()
        {
            if (Status != StatusSenha.Chamando && Status != StatusSenha.Aguardando)
                throw new InvalidOperationException("Apenas senhas aguardando ou sendo chamadas podem ser marcadas como não compareceu");

            Status = StatusSenha.NaoCompareceu;
            UpdateTimestamp();
        }

        public void Cancelar()
        {
            if (Status == StatusSenha.Atendido)
                throw new InvalidOperationException("Senhas já atendidas não podem ser canceladas");

            Status = StatusSenha.Cancelado;
            UpdateTimestamp();
        }

        public void AtualizarPrioridade(PrioridadeAtendimento novaPrioridade)
        {
            if (Status != StatusSenha.Aguardando)
                throw new InvalidOperationException("Apenas senhas aguardando podem ter a prioridade alterada");

            Prioridade = novaPrioridade;
            MotivoPrioridade = ObterMotivoPrioridade(novaPrioridade);
            UpdateTimestamp();
        }

        public void VincularConsultorio(string numeroConsultorio)
        {
            if (string.IsNullOrWhiteSpace(numeroConsultorio))
                throw new ArgumentException("O número do consultório não pode estar vazio", nameof(numeroConsultorio));

            NumeroConsultorio = numeroConsultorio.Trim();
            UpdateTimestamp();
        }

        public TimeSpan ObterTempoEspera()
        {
            var tempoFim = DataHoraChamada ?? DateTime.UtcNow;
            return tempoFim - DataHoraEntrada;
        }

        public bool EstaAtiva()
        {
            return Status == StatusSenha.Aguardando || 
                   Status == StatusSenha.Chamando || 
                   Status == StatusSenha.EmAtendimento;
        }
    }
}
