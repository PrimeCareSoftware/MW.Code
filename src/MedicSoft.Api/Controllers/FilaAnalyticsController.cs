using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FilaAnalyticsController : BaseController
    {
        private readonly IFilaAnalyticsService _analyticsService;

        public FilaAnalyticsController(
            IFilaAnalyticsService analyticsService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _analyticsService = analyticsService;
        }

        /// <summary>
        /// Obter métricas do dia para uma fila específica ou todas as filas
        /// </summary>
        [HttpGet("metricas/dia")]
        public async Task<ActionResult<FilaMetricsDto>> GetMetricasDoDia(
            [FromQuery] DateTime? data,
            [FromQuery] Guid? filaId)
        {
            try
            {
                var dataConsulta = data ?? DateTime.Now;
                var metrics = await _analyticsService.GetMetricasDoDiaAsync(
                    dataConsulta,
                    filaId,
                    GetTenantId());
                
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter métricas de um período para análise
        /// </summary>
        [HttpGet("metricas/periodo")]
        public async Task<ActionResult<FilaMetricsDto>> GetMetricasDoPeriodo(
            [FromQuery] DateTime dataInicio,
            [FromQuery] DateTime dataFim,
            [FromQuery] Guid? filaId)
        {
            try
            {
                var metrics = await _analyticsService.GetMetricasDoPeriodoAsync(
                    dataInicio,
                    dataFim,
                    filaId,
                    GetTenantId());
                
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter tempo médio de espera por especialidade
        /// </summary>
        [HttpGet("tempo-medio-espera")]
        public async Task<ActionResult<double>> GetTempoMedioEspera([FromQuery] Guid? especialidadeId)
        {
            try
            {
                var tempo = await _analyticsService.GetTempoMedioEsperaAsync(
                    especialidadeId,
                    GetTenantId());
                
                return Ok(new { tempoMedioMinutos = tempo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter tempo médio de atendimento por especialidade
        /// </summary>
        [HttpGet("tempo-medio-atendimento")]
        public async Task<ActionResult<double>> GetTempoMedioAtendimento([FromQuery] Guid? especialidadeId)
        {
            try
            {
                var tempo = await _analyticsService.GetTempoMedioAtendimentoAsync(
                    especialidadeId,
                    GetTenantId());
                
                return Ok(new { tempoMedioMinutos = tempo });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Obter horário de pico de atendimentos
        /// </summary>
        [HttpGet("horario-pico")]
        public async Task<ActionResult<HorarioPicoDto>> GetHorarioPico(
            [FromQuery] DateTime? data,
            [FromQuery] Guid? filaId)
        {
            try
            {
                var dataConsulta = data ?? DateTime.Now;
                var horarioPico = await _analyticsService.GetHorarioPicoAsync(
                    dataConsulta,
                    filaId,
                    GetTenantId());
                
                return Ok(horarioPico);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Calcular taxa de não comparecimento
        /// </summary>
        [HttpGet("taxa-nao-comparecimento")]
        public async Task<ActionResult<double>> GetTaxaNaoComparecimento(
            [FromQuery] DateTime? data,
            [FromQuery] Guid? filaId)
        {
            try
            {
                var dataConsulta = data ?? DateTime.Now;
                var taxa = await _analyticsService.CalcularTaxaNaoComparecimentoAsync(
                    dataConsulta,
                    filaId,
                    GetTenantId());
                
                return Ok(new { taxaPercentual = taxa });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
