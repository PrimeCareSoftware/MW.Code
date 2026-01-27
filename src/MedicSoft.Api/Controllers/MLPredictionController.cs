using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.ML.Services;
using MedicSoft.ML.Models;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MLPredictionController : ControllerBase
    {
        private readonly IPrevisaoDemandaService _demandaService;
        private readonly IPrevisaoNoShowService _noShowService;
        private readonly ILogger<MLPredictionController> _logger;

        public MLPredictionController(
            IPrevisaoDemandaService demandaService,
            IPrevisaoNoShowService noShowService,
            ILogger<MLPredictionController> logger)
        {
            _demandaService = demandaService;
            _noShowService = noShowService;
            _logger = logger;
        }

        /// <summary>
        /// Get demand forecast for the next 7 days
        /// </summary>
        [HttpGet("demanda/proxima-semana")]
        public ActionResult<PrevisaoConsultas> GetPrevisaoProximaSemana()
        {
            try
            {
                var previsao = _demandaService.PreverProximaSemana();
                return Ok(previsao);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Modelo de demanda não está treinado");
                return BadRequest(new { message = "Modelo de previsão não está disponível. Treine o modelo primeiro." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar previsão de demanda");
                return StatusCode(500, new { message = "Erro ao gerar previsão de demanda" });
            }
        }

        /// <summary>
        /// Get demand forecast for a specific date
        /// </summary>
        [HttpGet("demanda/data")]
        public ActionResult<int> GetPrevisaoParaData([FromQuery] DateTime data)
        {
            try
            {
                var previsao = _demandaService.PreverParaData(data);
                return Ok(new { data, consultasPrevistas = previsao });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Modelo de demanda não está treinado");
                return BadRequest(new { message = "Modelo de previsão não está disponível" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar previsão de demanda para data específica");
                return StatusCode(500, new { message = "Erro ao gerar previsão" });
            }
        }

        /// <summary>
        /// Calculate no-show risk for an appointment
        /// </summary>
        [HttpPost("noshow/calcular-risco")]
        public ActionResult<object> CalcularRiscoNoShow([FromBody] DadosNoShow dados)
        {
            try
            {
                // Validate input
                if (dados == null)
                {
                    return BadRequest(new { message = "Dados de entrada não podem ser nulos" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var risco = _noShowService.CalcularRiscoNoShow(dados);
                var acoes = _noShowService.SugerirAcoes(risco);

                return Ok(new
                {
                    riscoNoShow = risco,
                    riscoPercentual = Math.Round(risco * 100, 2),
                    nivel = GetNivelRisco(risco),
                    acoesRecomendadas = acoes
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Modelo de no-show não está treinado");
                return BadRequest(new { message = "Modelo de previsão não está disponível" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular risco de no-show");
                return StatusCode(500, new { message = "Erro ao calcular risco" });
            }
        }

        /// <summary>
        /// Load ML models (Admin only)
        /// </summary>
        [HttpPost("admin/carregar-modelos")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> CarregarModelos()
        {
            try
            {
                var demandaCarregado = await _demandaService.CarregarModeloAsync();
                var noShowCarregado = await _noShowService.CarregarModeloAsync();

                return Ok(new
                {
                    message = "Modelos carregados",
                    modeloDemanda = demandaCarregado ? "Carregado" : "Não encontrado",
                    modeloNoShow = noShowCarregado ? "Carregado" : "Não encontrado"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar modelos ML");
                return StatusCode(500, new { message = "Erro ao carregar modelos" });
            }
        }

        /// <summary>
        /// Train demand forecasting model (Admin only)
        /// </summary>
        [HttpPost("admin/treinar/demanda")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> TreinarModeloDemanda([FromBody] List<DadosTreinamentoDemanda> dadosTreinamento)
        {
            try
            {
                if (dadosTreinamento == null || dadosTreinamento.Count < 30)
                {
                    return BadRequest(new { message = "São necessários pelo menos 30 registros para treinar o modelo" });
                }

                await _demandaService.TreinarModeloAsync(dadosTreinamento);
                return Ok(new { message = "Modelo de demanda treinado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo de demanda");
                return StatusCode(500, new { message = "Erro ao treinar modelo", erro = ex.Message });
            }
        }

        /// <summary>
        /// Train no-show prediction model (Admin only)
        /// </summary>
        [HttpPost("admin/treinar/noshow")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult> TreinarModeloNoShow([FromBody] List<DadosNoShow> dadosTreinamento)
        {
            try
            {
                if (dadosTreinamento == null || dadosTreinamento.Count < 30)
                {
                    return BadRequest(new { message = "São necessários pelo menos 30 registros para treinar o modelo" });
                }

                await _noShowService.TreinarModeloAsync(dadosTreinamento);
                return Ok(new { message = "Modelo de no-show treinado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao treinar modelo de no-show");
                return StatusCode(500, new { message = "Erro ao treinar modelo", erro = ex.Message });
            }
        }

        private string GetNivelRisco(double risco)
        {
            if (risco > 0.7) return "ALTO";
            if (risco > 0.5) return "MÉDIO";
            if (risco > 0.3) return "BAIXO";
            return "MUITO BAIXO";
        }
    }
}
