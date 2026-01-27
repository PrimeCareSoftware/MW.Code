using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum TipoFila
    {
        Geral = 1,
        PorEspecialidade = 2,
        PorMedico = 3,
        Triagem = 4
    }

    /// <summary>
    /// Representa uma fila de espera avançada com totem e painel de TV
    /// </summary>
    public class FilaEspera : BaseEntity
    {
        public Guid ClinicaId { get; private set; }
        
        public string Nome { get; private set; }
        public TipoFila Tipo { get; private set; }
        public bool Ativa { get; private set; }
        
        // Configurações
        public int TempoMedioAtendimento { get; private set; }
        public bool UsaPrioridade { get; private set; }
        public bool UsaAgendamento { get; private set; }
        
        // Navigation properties
        public Clinic Clinica { get; private set; } = null!;
        public ICollection<SenhaFila> Senhas { get; private set; } = new List<SenhaFila>();

        private FilaEspera()
        {
            // EF Core constructor
            Nome = null!;
        }

        public FilaEspera(
            Guid clinicaId,
            string nome,
            TipoFila tipo,
            string tenantId,
            int tempoMedioAtendimento = 15,
            bool usaPrioridade = true,
            bool usaAgendamento = true) : base(tenantId)
        {
            if (clinicaId == Guid.Empty)
                throw new ArgumentException("O ID da clínica não pode estar vazio", nameof(clinicaId));
            
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da fila não pode estar vazio", nameof(nome));
            
            if (tempoMedioAtendimento <= 0)
                throw new ArgumentException("O tempo médio de atendimento deve ser maior que zero", nameof(tempoMedioAtendimento));

            ClinicaId = clinicaId;
            Nome = nome.Trim();
            Tipo = tipo;
            Ativa = true;
            TempoMedioAtendimento = tempoMedioAtendimento;
            UsaPrioridade = usaPrioridade;
            UsaAgendamento = usaAgendamento;
        }

        public void Ativar()
        {
            Ativa = true;
            UpdateTimestamp();
        }

        public void Desativar()
        {
            Ativa = false;
            UpdateTimestamp();
        }

        public void AtualizarConfiguracoes(
            int? tempoMedioAtendimento = null,
            bool? usaPrioridade = null,
            bool? usaAgendamento = null)
        {
            if (tempoMedioAtendimento.HasValue)
            {
                if (tempoMedioAtendimento.Value <= 0)
                    throw new ArgumentException("O tempo médio de atendimento deve ser maior que zero");
                
                TempoMedioAtendimento = tempoMedioAtendimento.Value;
            }

            if (usaPrioridade.HasValue)
                UsaPrioridade = usaPrioridade.Value;

            if (usaAgendamento.HasValue)
                UsaAgendamento = usaAgendamento.Value;

            UpdateTimestamp();
        }

        public void AtualizarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("O nome da fila não pode estar vazio", nameof(nome));

            Nome = nome.Trim();
            UpdateTimestamp();
        }
    }
}
