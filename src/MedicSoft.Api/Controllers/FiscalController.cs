using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities.Fiscal;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller para gestão fiscal e dashboards de impostos
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class FiscalController : BaseController
    {
        private readonly IApuracaoImpostosService _apuracaoService;
        private readonly IApuracaoImpostosRepository _apuracaoRepository;
        private readonly IConfiguracaoFiscalRepository _configuracaoFiscalRepository;
        private readonly IDREService _dreService;
        private readonly ILogger<FiscalController> _logger;

        public FiscalController(
            ITenantContext tenantContext,
            IApuracaoImpostosService apuracaoService,
            IApuracaoImpostosRepository apuracaoRepository,
            IConfiguracaoFiscalRepository configuracaoFiscalRepository,
            IDREService dreService,
            ILogger<FiscalController> logger) : base(tenantContext)
        {
            _apuracaoService = apuracaoService;
            _apuracaoRepository = apuracaoRepository;
            _configuracaoFiscalRepository = configuracaoFiscalRepository;
            _dreService = dreService;
            _logger = logger;
        }

        /// <summary>
        /// Obtém apuração mensal de impostos
        /// </summary>
        /// <param name="mes">Mês (1-12)</param>
        /// <param name="ano">Ano (ex: 2026)</param>
        /// <returns>Dados da apuração mensal</returns>
        [HttpGet("apuracao/{mes}/{ano}")]
        [ProducesResponseType(typeof(ApuracaoImpostos), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetApuracaoMensal(int mes, int ano)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId();

                if (!clinicId.HasValue)
                {
                    return BadRequest(new { message = "Clínica não identificada. Por favor, faça login novamente." });
                }

                // Buscar apuração existente
                var apuracao = await _apuracaoRepository.GetByClinicaAndMesAnoAsync(
                    clinicId.Value, 
                    mes, 
                    ano, 
                    tenantId);

                // Se não existe, tentar gerar
                if (apuracao == null)
                {
                    _logger.LogInformation(
                        "Apuração não encontrada para {ClinicaId}/{Mes}/{Ano}. Tentando gerar...",
                        clinicId.Value, mes, ano);

                    try
                    {
                        apuracao = await _apuracaoService.GerarApuracaoMensalAsync(
                            clinicId.Value, 
                            mes, 
                            ano, 
                            tenantId);
                    }
                    catch (InvalidOperationException ex)
                    {
                        _logger.LogWarning(ex, "Não foi possível gerar apuração automática");
                        return NotFound(new { 
                            message = "Apuração não encontrada e não foi possível gerá-la automaticamente.",
                            details = ex.Message 
                        });
                    }
                }

                return Ok(apuracao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter apuração mensal");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Obtém configuração fiscal da clínica
        /// </summary>
        /// <returns>Configuração fiscal vigente</returns>
        [HttpGet("configuracao")]
        [ProducesResponseType(typeof(ConfiguracaoFiscal), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetConfiguracao()
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId();

                if (!clinicId.HasValue)
                {
                    return BadRequest(new { message = "Clínica não identificada." });
                }

                var dataReferencia = DateTime.UtcNow.Date;
                var configuracao = await _configuracaoFiscalRepository.GetConfiguracaoVigenteAsync(
                    clinicId.Value, 
                    dataReferencia, 
                    tenantId);

                if (configuracao == null)
                {
                    return NotFound(new { message = "Configuração fiscal não encontrada para esta clínica." });
                }

                return Ok(configuracao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter configuração fiscal");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Obtém evolução mensal de impostos dos últimos N meses
        /// </summary>
        /// <param name="meses">Quantidade de meses (padrão: 12)</param>
        /// <returns>Lista com apurações mensais</returns>
        [HttpGet("evolucao-mensal")]
        [ProducesResponseType(typeof(List<ApuracaoImpostos>), 200)]
        public async Task<IActionResult> GetEvolucaoMensal([FromQuery] int meses = 12)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId();

                if (!clinicId.HasValue)
                {
                    return BadRequest(new { message = "Clínica não identificada." });
                }

                if (meses < 1 || meses > 24)
                {
                    return BadRequest(new { message = "Quantidade de meses deve estar entre 1 e 24." });
                }

                var apuracoes = new List<ApuracaoImpostos>();
                var dataAtual = DateTime.UtcNow.Date;

                // Buscar apurações dos últimos N meses
                for (int i = 0; i < meses; i++)
                {
                    var data = dataAtual.AddMonths(-i);
                    var mes = data.Month;
                    var ano = data.Year;

                    var apuracao = await _apuracaoRepository.GetByClinicaAndMesAnoAsync(
                        clinicId.Value, 
                        mes, 
                        ano, 
                        tenantId);

                    if (apuracao != null)
                    {
                        apuracoes.Add(apuracao);
                    }
                }

                // Ordenar do mais antigo para o mais recente
                apuracoes = apuracoes.OrderBy(a => a.Ano).ThenBy(a => a.Mes).ToList();

                return Ok(apuracoes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter evolução mensal");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Obtém DRE do período especificado
        /// </summary>
        /// <param name="mes">Mês (1-12)</param>
        /// <param name="ano">Ano (ex: 2026)</param>
        /// <returns>Demonstração do Resultado do Exercício</returns>
        [HttpGet("dre/{mes}/{ano}")]
        [ProducesResponseType(typeof(DRE), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDRE(int mes, int ano)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId();

                if (!clinicId.HasValue)
                {
                    return BadRequest(new { message = "Clínica não identificada." });
                }

                var dataInicio = new DateTime(ano, mes, 1);
                var dataFim = dataInicio.AddMonths(1).AddDays(-1);

                var dre = await _dreService.GerarDREAsync(
                    clinicId.Value, 
                    dataInicio, 
                    dataFim, 
                    tenantId);

                if (dre == null)
                {
                    return NotFound(new { message = "DRE não encontrada para o período especificado." });
                }

                return Ok(dre);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter DRE");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Gera nova apuração mensal de impostos
        /// </summary>
        /// <param name="mes">Mês (1-12)</param>
        /// <param name="ano">Ano (ex: 2026)</param>
        /// <returns>Apuração gerada</returns>
        [HttpPost("apuracao/{mes}/{ano}")]
        [ProducesResponseType(typeof(ApuracaoImpostos), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GerarApuracao(int mes, int ano)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId();

                if (!clinicId.HasValue)
                {
                    return BadRequest(new { message = "Clínica não identificada." });
                }

                var apuracao = await _apuracaoService.GerarApuracaoMensalAsync(
                    clinicId.Value, 
                    mes, 
                    ano, 
                    tenantId);

                return CreatedAtAction(
                    nameof(GetApuracaoMensal), 
                    new { mes, ano }, 
                    apuracao);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar apuração");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Atualiza status da apuração
        /// </summary>
        /// <param name="apuracaoId">ID da apuração</param>
        /// <param name="status">Novo status</param>
        /// <returns>Apuração atualizada</returns>
        [HttpPut("apuracao/{apuracaoId}/status")]
        [ProducesResponseType(typeof(ApuracaoImpostos), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> AtualizarStatus(Guid apuracaoId, [FromBody] StatusApuracao status)
        {
            try
            {
                var tenantId = GetTenantId();

                var apuracao = await _apuracaoService.AtualizarStatusAsync(
                    apuracaoId, 
                    status, 
                    tenantId);

                return Ok(apuracao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar status da apuração");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }

        /// <summary>
        /// Registra pagamento de apuração
        /// </summary>
        [HttpPost("apuracao/{apuracaoId}/pagamento")]
        [ProducesResponseType(typeof(ApuracaoImpostos), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RegistrarPagamento(
            Guid apuracaoId, 
            [FromBody] PagamentoRequest request)
        {
            try
            {
                var tenantId = GetTenantId();

                var apuracao = await _apuracaoService.RegistrarPagamentoAsync(
                    apuracaoId, 
                    request.DataPagamento, 
                    request.Comprovante, 
                    tenantId);

                return Ok(apuracao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao registrar pagamento");
                return StatusCode(500, new { message = "Erro ao processar solicitação." });
            }
        }
    }

    /// <summary>
    /// Request para registrar pagamento
    /// </summary>
    public class PagamentoRequest
    {
        public DateTime DataPagamento { get; set; }
        public string Comprovante { get; set; } = string.Empty;
    }
}
