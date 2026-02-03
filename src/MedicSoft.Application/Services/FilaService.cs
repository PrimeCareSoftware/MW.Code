using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IFilaService
    {
        Task<FilaEsperaDto> CreateFilaAsync(Guid clinicaId, string nome, string tipo, string tenantId);
        Task<FilaEsperaDto?> GetFilaByIdAsync(Guid filaId, string tenantId);
        Task<FilaSummaryDto> GetFilaSummaryAsync(Guid filaId, string tenantId);
        Task<SenhaFilaDto> GerarSenhaAsync(GerarSenhaRequest request, string tenantId);
        Task<SenhaFilaDto> ChamarProximaSenhaAsync(ChamarSenhaRequest request, string tenantId);
        Task<SenhaFilaDto> IniciarAtendimentoAsync(Guid senhaId, string tenantId);
        Task<SenhaFilaDto> FinalizarAtendimentoAsync(Guid senhaId, string tenantId);
        Task CancelarSenhaAsync(Guid senhaId, string tenantId);
        Task<int> CalcularTempoEsperaAsync(Guid senhaId, string tenantId);
        Task<int> ObterPosicaoNaFilaAsync(Guid senhaId, string tenantId);
        Task<SenhaFilaDto?> ConsultarSenhaAsync(string numeroSenha, Guid filaId, string tenantId);
        Task<List<SenhaFilaDto>> GetSenhasAguardandoAsync(Guid filaId, string tenantId);
        Task<List<SenhaFilaDto>> GetUltimasChamadasAsync(Guid filaId, int quantidade, string tenantId);
    }

    public class FilaService : IFilaService
    {
        private readonly IFilaEsperaRepository _filaRepository;
        private readonly ISenhaFilaRepository _senhaRepository;
        private readonly IPatientRepository _patientRepository;

        public FilaService(
            IFilaEsperaRepository filaRepository,
            ISenhaFilaRepository senhaRepository,
            IPatientRepository patientRepository)
        {
            _filaRepository = filaRepository;
            _senhaRepository = senhaRepository;
            _patientRepository = patientRepository;
        }

        public async Task<FilaEsperaDto> CreateFilaAsync(Guid clinicaId, string nome, string tipo, string tenantId)
        {
            if (!Enum.TryParse<TipoFila>(tipo, out var tipoFila))
                throw new ArgumentException("Tipo de fila inválido");

            var fila = new FilaEspera(clinicaId, nome, tipoFila, tenantId);
            
            await _filaRepository.AddAsync(fila);
            await _filaRepository.SaveChangesAsync();

            return MapFilaToDto(fila);
        }

        public async Task<FilaEsperaDto?> GetFilaByIdAsync(Guid filaId, string tenantId)
        {
            var fila = await _filaRepository.GetByIdAsync(filaId, tenantId);
            return fila != null ? MapFilaToDto(fila) : null;
        }

        public async Task<FilaSummaryDto> GetFilaSummaryAsync(Guid filaId, string tenantId)
        {
            var fila = await _filaRepository.GetByIdAsync(filaId, tenantId);
            if (fila == null)
                throw new InvalidOperationException("Fila não encontrada");

            var senhas = await _senhaRepository.GetActiveSenhasByFilaAsync(filaId, tenantId);
            
            var senhasDto = new List<SenhaFilaDto>();
            foreach (var senha in senhas)
            {
                var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(senha.Id, tenantId);
                var tempoEstimado = await CalcularTempoEsperaInternoAsync(senha.Id, tenantId);
                
                senhasDto.Add(MapSenhaToDto(senha, posicao, tempoEstimado));
            }

            var senhasAguardando = senhas.Where(s => s.Status == StatusSenha.Aguardando).ToList();
            var tempoMedioEspera = senhasAguardando.Any()
                ? (int)senhasAguardando.Average(s => s.ObterTempoEspera().TotalMinutes)
                : 0;

            return new FilaSummaryDto
            {
                FilaId = fila.Id,
                NomeFila = fila.Nome,
                TotalAguardando = senhas.Count(s => s.Status == StatusSenha.Aguardando),
                TotalChamando = senhas.Count(s => s.Status == StatusSenha.Chamando),
                TotalEmAtendimento = senhas.Count(s => s.Status == StatusSenha.EmAtendimento),
                TempoMedioEsperaMinutos = tempoMedioEspera,
                Senhas = senhasDto
            };
        }

        public async Task<SenhaFilaDto> GerarSenhaAsync(GerarSenhaRequest request, string tenantId)
        {
            var fila = await _filaRepository.GetByIdAsync(request.FilaId, tenantId);
            if (fila == null)
                throw new InvalidOperationException("Fila não encontrada");

            if (!fila.Ativa)
                throw new InvalidOperationException("Fila não está ativa");

            // Determina prioridade
            var prioridade = DeterminarPrioridade(
                request.DataNascimento,
                request.IsGestante,
                request.IsDeficiente);

            // Gera número da senha
            var numeroSenha = await GerarNumeroSenhaAsync(request.FilaId, prioridade, tenantId);

            var senha = new SenhaFila(
                request.FilaId,
                request.NomePaciente,
                numeroSenha,
                prioridade,
                tenantId,
                request.PacienteId,
                request.Cpf,
                request.Telefone,
                request.AgendamentoId,
                request.EspecialidadeId);

            await _senhaRepository.AddAsync(senha);
            await _senhaRepository.SaveChangesAsync();

            // Calcula estimativas
            var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(senha.Id, tenantId);
            var tempoEstimado = await CalcularTempoEsperaInternoAsync(senha.Id, tenantId);

            return MapSenhaToDto(senha, posicao, tempoEstimado);
        }

        public async Task<SenhaFilaDto> ChamarProximaSenhaAsync(ChamarSenhaRequest request, string tenantId)
        {
            var proximaSenha = await _senhaRepository.GetProximaSenhaAsync(request.FilaId, tenantId);
            
            if (proximaSenha == null)
                throw new InvalidOperationException("Não há senhas na fila");

            proximaSenha.Chamar(request.MedicoId, request.NumeroConsultorio);
            
            await _senhaRepository.UpdateAsync(proximaSenha);
            await _senhaRepository.SaveChangesAsync();

            var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(proximaSenha.Id, tenantId);
            var tempoEstimado = await CalcularTempoEsperaInternoAsync(proximaSenha.Id, tenantId);

            return MapSenhaToDto(proximaSenha, posicao, tempoEstimado);
        }

        public async Task<SenhaFilaDto> IniciarAtendimentoAsync(Guid senhaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByIdAsync(senhaId, tenantId);
            if (senha == null)
                throw new InvalidOperationException("Senha não encontrada");

            senha.IniciarAtendimento();
            
            await _senhaRepository.UpdateAsync(senha);
            await _senhaRepository.SaveChangesAsync();

            var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(senha.Id, tenantId);
            var tempoEstimado = await CalcularTempoEsperaInternoAsync(senha.Id, tenantId);

            return MapSenhaToDto(senha, posicao, tempoEstimado);
        }

        public async Task<SenhaFilaDto> FinalizarAtendimentoAsync(Guid senhaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByIdAsync(senhaId, tenantId);
            if (senha == null)
                throw new InvalidOperationException("Senha não encontrada");

            senha.FinalizarAtendimento();
            
            await _senhaRepository.UpdateAsync(senha);
            await _senhaRepository.SaveChangesAsync();

            var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(senha.Id, tenantId);
            var tempoEstimado = await CalcularTempoEsperaInternoAsync(senha.Id, tenantId);

            return MapSenhaToDto(senha, posicao, tempoEstimado);
        }

        public async Task CancelarSenhaAsync(Guid senhaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByIdAsync(senhaId, tenantId);
            if (senha == null)
                throw new InvalidOperationException("Senha não encontrada");

            senha.Cancelar();
            
            await _senhaRepository.UpdateAsync(senha);
            await _senhaRepository.SaveChangesAsync();
        }

        public async Task<int> CalcularTempoEsperaAsync(Guid senhaId, string tenantId)
        {
            return await CalcularTempoEsperaInternoAsync(senhaId, tenantId);
        }

        public async Task<int> ObterPosicaoNaFilaAsync(Guid senhaId, string tenantId)
        {
            return await _senhaRepository.GetPosicaoNaFilaAsync(senhaId, tenantId);
        }

        public async Task<SenhaFilaDto?> ConsultarSenhaAsync(string numeroSenha, Guid filaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByNumeroSenhaAsync(numeroSenha, filaId, tenantId);
            if (senha == null)
                return null;

            var posicao = await _senhaRepository.GetPosicaoNaFilaAsync(senha.Id, tenantId);
            var tempoEstimado = await CalcularTempoEsperaInternoAsync(senha.Id, tenantId);

            return MapSenhaToDto(senha, posicao, tempoEstimado);
        }

        private PrioridadeAtendimento DeterminarPrioridade(
            DateTime dataNascimento,
            bool isGestante,
            bool isDeficiente)
        {
            var idade = DateTime.UtcNow.Year - dataNascimento.Year;
            if (DateTime.UtcNow < dataNascimento.AddYears(idade))
                idade--;

            if (isDeficiente) return PrioridadeAtendimento.Deficiente;
            if (isGestante) return PrioridadeAtendimento.Gestante;
            if (idade >= 60) return PrioridadeAtendimento.Idoso;
            if (idade < 2) return PrioridadeAtendimento.Crianca;

            return PrioridadeAtendimento.Normal;
        }

        private async Task<string> GerarNumeroSenhaAsync(Guid filaId, PrioridadeAtendimento prioridade, string tenantId)
        {
            // Prefixo baseado na prioridade
            var prefixo = prioridade switch
            {
                PrioridadeAtendimento.Urgencia => "U",
                PrioridadeAtendimento.Deficiente => "D",
                PrioridadeAtendimento.Gestante => "G",
                PrioridadeAtendimento.Idoso => "I",
                PrioridadeAtendimento.Crianca => "C",
                _ => "N"
            };

            // Conta senhas do dia com o mesmo prefixo
            var hoje = DateTime.UtcNow.Date;
            var senhasHoje = await _senhaRepository.GetByFilaIdAsync(filaId, tenantId);
            var contador = senhasHoje
                .Where(s => s.DataHoraEntrada.Date == hoje && s.NumeroSenha.StartsWith(prefixo))
                .Count() + 1;

            return $"{prefixo}{contador:D3}";
        }

        private async Task<int> CalcularTempoEsperaInternoAsync(Guid senhaId, string tenantId)
        {
            var senha = await _senhaRepository.GetByIdAsync(senhaId, tenantId);
            if (senha == null || senha.Status != StatusSenha.Aguardando)
                return 0;

            var fila = await _filaRepository.GetByIdAsync(senha.FilaId, tenantId);
            if (fila == null)
                return 0;

            // Conta senhas à frente
            var senhasAFrente = await _senhaRepository.CountSenhasAFrenteAsync(senhaId, tenantId);

            // Tempo médio de atendimento da fila
            var tempoMedio = fila.TempoMedioAtendimento;

            // Fator de prioridade (senhas normais esperam mais)
            var fatorPrioridade = senha.Prioridade == PrioridadeAtendimento.Normal ? 1.3 : 1.0;

            return (int)(senhasAFrente * tempoMedio * fatorPrioridade);
        }

        private static FilaEsperaDto MapFilaToDto(FilaEspera fila)
        {
            return new FilaEsperaDto
            {
                Id = fila.Id,
                ClinicaId = fila.ClinicaId,
                Nome = fila.Nome,
                Tipo = fila.Tipo.ToString(),
                Ativa = fila.Ativa,
                TempoMedioAtendimento = fila.TempoMedioAtendimento,
                UsaPrioridade = fila.UsaPrioridade,
                UsaAgendamento = fila.UsaAgendamento,
                CreatedAt = fila.CreatedAt,
                UpdatedAt = fila.UpdatedAt ?? fila.CreatedAt
            };
        }

        public async Task<List<SenhaFilaDto>> GetSenhasAguardandoAsync(Guid filaId, string tenantId)
        {
            var senhas = await _senhaRepository.GetAguardandoByFilaAsync(filaId, tenantId);
            
            // For waiting queue display, we don't need detailed position/time calculations
            // as these are for simple listing on the TV panel
            return senhas.Select(senha => MapSenhaToDto(senha, 0, 0)).ToList();
        }

        public async Task<List<SenhaFilaDto>> GetUltimasChamadasAsync(Guid filaId, int quantidade, string tenantId)
        {
            var senhas = await _senhaRepository.GetUltimasChamadasAsync(filaId, quantidade, tenantId);
            
            // For call history display, position and estimated time are not relevant
            return senhas.Select(senha => MapSenhaToDto(senha, 0, 0)).ToList();
        }

        private static SenhaFilaDto MapSenhaToDto(SenhaFila senha, int posicao, int tempoEstimado)
        {
            return new SenhaFilaDto
            {
                Id = senha.Id,
                FilaId = senha.FilaId,
                PacienteId = senha.PacienteId,
                NomePaciente = senha.NomePaciente,
                CpfPaciente = senha.CpfPaciente,
                TelefonePaciente = senha.TelefonePaciente,
                NumeroSenha = senha.NumeroSenha,
                DataHoraEntrada = senha.DataHoraEntrada,
                DataHoraChamada = senha.DataHoraChamada,
                DataHoraAtendimento = senha.DataHoraAtendimento,
                DataHoraSaida = senha.DataHoraSaida,
                Prioridade = senha.Prioridade.ToString(),
                MotivoPrioridade = senha.MotivoPrioridade,
                Status = senha.Status.ToString(),
                TentativasChamada = senha.TentativasChamada,
                MedicoId = senha.MedicoId,
                NomeMedico = senha.Medico?.FullName,
                EspecialidadeId = senha.EspecialidadeId,
                NumeroConsultorio = senha.NumeroConsultorio,
                AgendamentoId = senha.AgendamentoId,
                TempoEsperaMinutos = senha.TempoEsperaMinutos,
                TempoAtendimentoMinutos = senha.TempoAtendimentoMinutos,
                PosicaoNaFila = posicao,
                TempoEstimadoEspera = tempoEstimado,
                CreatedAt = senha.CreatedAt,
                UpdatedAt = senha.UpdatedAt ?? senha.CreatedAt
            };
        }
    }
}
