using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Analytics.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for Business Intelligence and Analytics dashboards
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AnalyticsController : BaseController
    {
        private readonly IDashboardClinicoService _dashboardClinicoService;
        private readonly IDashboardFinanceiroService _dashboardFinanceiroService;
        private readonly IConsolidacaoDadosService _consolidacaoDadosService;
        private readonly ILogger<AnalyticsController> _logger;

        public AnalyticsController(
            ITenantContext tenantContext,
            IDashboardClinicoService dashboardClinicoService,
            IDashboardFinanceiroService dashboardFinanceiroService,
            IConsolidacaoDadosService consolidacaoDadosService,
            ILogger<AnalyticsController> logger) : base(tenantContext)
        {
            _dashboardClinicoService = dashboardClinicoService;
            _dashboardFinanceiroService = dashboardFinanceiroService;
            _consolidacaoDadosService = consolidacaoDadosService;
            _logger = logger;
        }

        /// <summary>
        /// Get clinical dashboard metrics
        /// </summary>
        /// <param name="inicio">Start date</param>
        /// <param name="fim">End date</param>
        /// <param name="medicoId">Optional: Filter by doctor ID</param>
        /// <returns>Clinical dashboard data</returns>
        [HttpGet("dashboard/clinico")]
        public async Task<IActionResult> GetDashboardClinico(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim,
            [FromQuery] Guid? medicoId = null)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogInformation("Buscando dashboard clínico - Tenant: {TenantId}, Período: {Inicio} - {Fim}", 
                    tenantId, inicio, fim);

                var dashboard = await _dashboardClinicoService.GetDashboardAsync(inicio, fim, tenantId, medicoId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dashboard clínico");
                return StatusCode(500, new { message = "Erro ao processar a solicitação" });
            }
        }

        /// <summary>
        /// Get financial dashboard metrics
        /// </summary>
        /// <param name="inicio">Start date</param>
        /// <param name="fim">End date</param>
        /// <returns>Financial dashboard data</returns>
        [HttpGet("dashboard/financeiro")]
        public async Task<IActionResult> GetDashboardFinanceiro(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogInformation("Buscando dashboard financeiro - Tenant: {TenantId}, Período: {Inicio} - {Fim}", 
                    tenantId, inicio, fim);

                var dashboard = await _dashboardFinanceiroService.GetDashboardAsync(inicio, fim, tenantId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar dashboard financeiro");
                return StatusCode(500, new { message = "Erro ao processar a solicitação" });
            }
        }

        /// <summary>
        /// Get revenue projection for current month
        /// </summary>
        /// <returns>Projected revenue for the month</returns>
        [HttpGet("projecao/receita-mes")]
        public async Task<IActionResult> GetProjecaoReceitaMes()
        {
            try
            {
                var tenantId = GetTenantId();
                var agora = DateTime.UtcNow;
                var projecao = await _dashboardFinanceiroService.ProjetarReceitaMesAsync(agora, tenantId);
                
                return Ok(new 
                { 
                    mes = agora.ToString("MMMM yyyy"),
                    projecao = projecao,
                    dataCalculo = agora
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao projetar receita do mês");
                return StatusCode(500, new { message = "Erro ao processar a solicitação" });
            }
        }

        /// <summary>
        /// Manually trigger data consolidation for a specific date
        /// </summary>
        /// <param name="data">Date to consolidate</param>
        /// <returns>Success message</returns>
        [HttpPost("consolidar/dia")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> ConsolidarDia([FromQuery] DateTime data)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogInformation("Consolidando dados manualmente - Tenant: {TenantId}, Data: {Data}", 
                    tenantId, data);

                await _consolidacaoDadosService.ConsolidarDadosDiarioAsync(data, tenantId);
                
                return Ok(new 
                { 
                    message = "Dados consolidados com sucesso",
                    data = data.Date
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consolidar dados");
                return StatusCode(500, new { message = "Erro ao processar a consolidação" });
            }
        }

        /// <summary>
        /// Manually trigger data consolidation for a date range
        /// </summary>
        /// <param name="inicio">Start date</param>
        /// <param name="fim">End date</param>
        /// <returns>Success message</returns>
        [HttpPost("consolidar/periodo")]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<IActionResult> ConsolidarPeriodo(
            [FromQuery] DateTime inicio,
            [FromQuery] DateTime fim)
        {
            try
            {
                var tenantId = GetTenantId();
                _logger.LogInformation("Consolidando período - Tenant: {TenantId}, Período: {Inicio} - {Fim}", 
                    tenantId, inicio, fim);

                await _consolidacaoDadosService.ConsolidarPeriodoAsync(inicio, fim, tenantId);
                
                return Ok(new 
                { 
                    message = "Período consolidado com sucesso",
                    inicio = inicio.Date,
                    fim = fim.Date
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao consolidar período");
                return StatusCode(500, new { message = "Erro ao processar a consolidação" });
            }
        }
    }
}
