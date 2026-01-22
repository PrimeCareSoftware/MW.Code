using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for TISS analytics and reporting
    /// </summary>
    [ApiController]
    [Route("api/tiss-analytics")]
    [Authorize]
    public class TissAnalyticsController : BaseController
    {
        private readonly ITissAnalyticsService _tissAnalyticsService;

        public TissAnalyticsController(
            ITissAnalyticsService tissAnalyticsService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _tissAnalyticsService = tissAnalyticsService;
        }

        /// <summary>
        /// Get glosas summary for a clinic in a date range
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("glosas-summary")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<GlosasSummaryDto>> GetGlosasSummary(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetGlosasSummaryAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar resumo de glosas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get glosas grouped by operator
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("glosas-by-operator")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<GlosasByOperatorDto>>> GetGlosasByOperator(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetGlosasByOperatorAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar glosas por operadora: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get glosas trend over time
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="months">Number of months to analyze (default: 6)</param>
        [HttpGet("glosas-trend")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<GlosasTrendDto>>> GetGlosasTrend(
            [FromQuery] Guid clinicId,
            [FromQuery] int months = 6)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (months < 1 || months > 24)
                    return BadRequest(new { message = "Número de meses deve estar entre 1 e 24." });

                var result = await _tissAnalyticsService.GetGlosasTrendAsync(
                    clinicId, 
                    months, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar tendência de glosas: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get top procedures with glosas
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("procedure-glosas")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<ProcedureGlosasDto>>> GetProcedureGlosas(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetProcedureGlosasAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar glosas por procedimento: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get authorization rate by operator
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("authorization-rate")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<AuthorizationRateDto>>> GetAuthorizationRate(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetAuthorizationRateAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar taxa de autorização: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get average approval time by operator
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("approval-time")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<ApprovalTimeDto>>> GetApprovalTime(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetApprovalTimeAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar tempo de aprovação: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get monthly performance comparison
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="months">Number of months to compare (default: 12)</param>
        [HttpGet("monthly-performance")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<MonthlyPerformanceDto>>> GetMonthlyPerformance(
            [FromQuery] Guid clinicId,
            [FromQuery] int months = 12)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (months < 1 || months > 24)
                    return BadRequest(new { message = "Número de meses deve estar entre 1 e 24." });

                var result = await _tissAnalyticsService.GetMonthlyPerformanceAsync(
                    clinicId, 
                    months, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar performance mensal: {ex.Message}" });
            }
        }

        /// <summary>
        /// Get glosa alerts (items above average)
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Start date (ISO format)</param>
        /// <param name="endDate">End date (ISO format)</param>
        [HttpGet("glosa-alerts")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<GlosaAlertDto>>> GetGlosaAlerts(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            try
            {
                if (clinicId == Guid.Empty)
                    return BadRequest(new { message = "Clinic ID é obrigatório." });

                if (startDate > endDate)
                    return BadRequest(new { message = "Data inicial deve ser anterior à data final." });

                var result = await _tissAnalyticsService.GetGlosaAlertsAsync(
                    clinicId, 
                    startDate, 
                    endDate, 
                    GetTenantId());

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Erro ao buscar alertas de glosa: {ex.Message}" });
            }
        }
    }
}
