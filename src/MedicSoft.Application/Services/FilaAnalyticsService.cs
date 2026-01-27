using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Interface for queue analytics service
    /// </summary>
    public interface IFilaAnalyticsService
    {
        Task<FilaMetricsDto> GetMetricasDoDiaAsync(DateTime data, Guid? filaId, string tenantId);
        Task<FilaMetricsDto> GetMetricasDoPeriodoAsync(DateTime dataInicio, DateTime dataFim, Guid? filaId, string tenantId);
        Task<double> GetTempoMedioEsperaAsync(Guid? especialidadeId, string tenantId);
        Task<double> GetTempoMedioAtendimentoAsync(Guid? especialidadeId, string tenantId);
        Task<HorarioPicoDto> GetHorarioPicoAsync(DateTime data, Guid? filaId, string tenantId);
        Task<double> CalcularTaxaNaoComparecimentoAsync(DateTime data, Guid? filaId, string tenantId);
        Task RegistrarAtendimentoAsync(SenhaFila senha);
    }

    /// <summary>
    /// Service for queue analytics and metrics
    /// </summary>
    public class FilaAnalyticsService : IFilaAnalyticsService
    {
        private readonly ISenhaFilaRepository _senhaRepository;
        private readonly IFilaEsperaRepository _filaRepository;

        public FilaAnalyticsService(
            ISenhaFilaRepository senhaRepository,
            IFilaEsperaRepository filaRepository)
        {
            _senhaRepository = senhaRepository;
            _filaRepository = filaRepository;
        }

        /// <summary>
        /// Obtém métricas do dia para uma fila específica ou todas as filas
        /// </summary>
        public async Task<FilaMetricsDto> GetMetricasDoDiaAsync(DateTime data, Guid? filaId, string tenantId)
        {
            return await GetMetricasDoPeriodoAsync(data.Date, data.Date.AddDays(1).AddSeconds(-1), filaId, tenantId);
        }

        /// <summary>
        /// Obtém métricas de um período para análise
        /// </summary>
        public async Task<FilaMetricsDto> GetMetricasDoPeriodoAsync(DateTime dataInicio, DateTime dataFim, Guid? filaId, string tenantId)
        {
            IEnumerable<SenhaFila> senhas;

            if (filaId.HasValue)
            {
                var todasSenhas = await _senhaRepository.GetByFilaIdAsync(filaId.Value, tenantId);
                senhas = todasSenhas.Where(s => 
                    s.DataHoraEntrada >= dataInicio && 
                    s.DataHoraEntrada <= dataFim);
            }
            else
            {
                // Todas as filas do tenant
                var todasSenhas = await _senhaRepository.GetAllAsync(tenantId);
                senhas = todasSenhas.Where(s => 
                    s.DataHoraEntrada >= dataInicio && 
                    s.DataHoraEntrada <= dataFim);
            }

            var senhasList = senhas.ToList();
            var senhasAtendidas = senhasList.Where(s => s.Status == StatusSenha.Atendido).ToList();
            var senhasNaoCompareceram = senhasList.Where(s => s.Status == StatusSenha.NaoCompareceu).ToList();

            var tempoMedioEspera = senhasAtendidas.Any() 
                ? senhasAtendidas.Average(s => s.TempoEsperaMinutos) 
                : 0;

            var tempoMedioAtendimento = senhasAtendidas.Any() 
                ? senhasAtendidas.Average(s => s.TempoAtendimentoMinutos) 
                : 0;

            var taxaNaoComparecimento = senhasList.Any()
                ? (double)senhasNaoCompareceram.Count / senhasList.Count * 100
                : 0;

            var horarioPico = await GetHorarioPicoAsync(dataInicio, filaId, tenantId);

            var atendimentosPorPrioridade = senhasList.Count > 0
                ? senhasList
                    .GroupBy(s => s.Prioridade)
                    .Select(g => new AtendimentoPrioridadeDto
                    {
                        Prioridade = g.Key.ToString(),
                        Total = g.Count(),
                        Percentual = (double)g.Count() / senhasList.Count * 100
                    })
                    .ToList()
                : new List<AtendimentoPrioridadeDto>();

            return new FilaMetricsDto
            {
                DataInicio = dataInicio,
                DataFim = dataFim,
                FilaId = filaId,
                TotalSenhas = senhasList.Count,
                TotalAtendimentos = senhasAtendidas.Count,
                TotalNaoCompareceu = senhasNaoCompareceram.Count,
                TotalCancelados = senhasList.Count(s => s.Status == StatusSenha.Cancelado),
                TotalAguardando = senhasList.Count(s => s.Status == StatusSenha.Aguardando),
                TempoMedioEsperaMinutos = Math.Round(tempoMedioEspera, 2),
                TempoMedioAtendimentoMinutos = Math.Round(tempoMedioAtendimento, 2),
                TaxaNaoComparecimento = Math.Round(taxaNaoComparecimento, 2),
                HorarioPico = horarioPico,
                AtendimentosPorPrioridade = atendimentosPorPrioridade
            };
        }

        /// <summary>
        /// Obtém tempo médio de espera por especialidade
        /// </summary>
        public async Task<double> GetTempoMedioEsperaAsync(Guid? especialidadeId, string tenantId)
        {
            var senhas = await _senhaRepository.GetAllAsync(tenantId);
            
            var senhasFiltradas = especialidadeId.HasValue
                ? senhas.Where(s => s.EspecialidadeId == especialidadeId.Value && s.Status == StatusSenha.Atendido)
                : senhas.Where(s => s.Status == StatusSenha.Atendido);

            var lista = senhasFiltradas.ToList();
            return lista.Any() ? lista.Average(s => s.TempoEsperaMinutos) : 0;
        }

        /// <summary>
        /// Obtém tempo médio de atendimento por especialidade
        /// </summary>
        public async Task<double> GetTempoMedioAtendimentoAsync(Guid? especialidadeId, string tenantId)
        {
            var senhas = await _senhaRepository.GetAllAsync(tenantId);
            
            var senhasFiltradas = especialidadeId.HasValue
                ? senhas.Where(s => s.EspecialidadeId == especialidadeId.Value && s.Status == StatusSenha.Atendido)
                : senhas.Where(s => s.Status == StatusSenha.Atendido);

            var lista = senhasFiltradas.ToList();
            return lista.Any() ? lista.Average(s => s.TempoAtendimentoMinutos) : 0;
        }

        /// <summary>
        /// Identifica horário de pico de atendimentos
        /// </summary>
        public async Task<HorarioPicoDto> GetHorarioPicoAsync(DateTime data, Guid? filaId, string tenantId)
        {
            IEnumerable<SenhaFila> senhas;

            if (filaId.HasValue)
            {
                var todasSenhas = await _senhaRepository.GetByFilaIdAsync(filaId.Value, tenantId);
                senhas = todasSenhas.Where(s => s.DataHoraEntrada.Date == data.Date);
            }
            else
            {
                var todasSenhas = await _senhaRepository.GetAllAsync(tenantId);
                senhas = todasSenhas.Where(s => s.DataHoraEntrada.Date == data.Date);
            }

            var senhasPorHora = senhas
                .GroupBy(s => s.DataHoraEntrada.Hour)
                .Select(g => new
                {
                    Hora = g.Key,
                    Quantidade = g.Count()
                })
                .OrderByDescending(x => x.Quantidade)
                .FirstOrDefault();

            if (senhasPorHora == null)
            {
                return new HorarioPicoDto
                {
                    Hora = 0,
                    Quantidade = 0,
                    Descricao = "Sem dados"
                };
            }

            return new HorarioPicoDto
            {
                Hora = senhasPorHora.Hora,
                Quantidade = senhasPorHora.Quantidade,
                Descricao = $"{senhasPorHora.Hora:D2}:00 - {senhasPorHora.Hora + 1:D2}:00"
            };
        }

        /// <summary>
        /// Calcula taxa de não comparecimento
        /// </summary>
        public async Task<double> CalcularTaxaNaoComparecimentoAsync(DateTime data, Guid? filaId, string tenantId)
        {
            IEnumerable<SenhaFila> senhas;

            if (filaId.HasValue)
            {
                var todasSenhas = await _senhaRepository.GetByFilaIdAsync(filaId.Value, tenantId);
                senhas = todasSenhas.Where(s => s.DataHoraEntrada.Date == data.Date);
            }
            else
            {
                var todasSenhas = await _senhaRepository.GetAllAsync(tenantId);
                senhas = todasSenhas.Where(s => s.DataHoraEntrada.Date == data.Date);
            }

            var senhasList = senhas.ToList();
            if (!senhasList.Any())
                return 0;

            var naoCompareceram = senhasList.Count(s => s.Status == StatusSenha.NaoCompareceu);
            return (double)naoCompareceram / senhasList.Count * 100;
        }

        /// <summary>
        /// Registra dados do atendimento finalizado para análise futura
        /// </summary>
        public async Task RegistrarAtendimentoAsync(SenhaFila senha)
        {
            // Este método pode ser usado para registrar dados adicionais
            // em uma tabela de analytics separada se necessário
            // Por enquanto, os dados já estão na própria senha
            await Task.CompletedTask;
        }
    }
}
