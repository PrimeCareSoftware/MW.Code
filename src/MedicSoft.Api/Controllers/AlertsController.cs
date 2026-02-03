using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento de alertas do sistema
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class AlertsController : BaseController
    {
        private readonly IAlertService _alertService;
        
        public AlertsController(
            IAlertService alertService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _alertService = alertService;
        }
        
        /// <summary>
        /// Criar um novo alerta
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(AlertDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AlertDto>> CreateAlert([FromBody] CreateAlertDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var tenantId = GetTenantId();
            var alert = await _alertService.CreateAlertAsync(dto, tenantId);
            
            return CreatedAtAction(nameof(GetAlertById), new { id = alert.Id }, alert);
        }
        
        /// <summary>
        /// Obter alerta por ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AlertDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AlertDto>> GetAlertById(Guid id)
        {
            var tenantId = GetTenantId();
            var alert = await _alertService.GetAlertByIdAsync(id, tenantId);
            
            if (alert == null)
                return NotFound(new { message = "Alert not found" });
            
            return Ok(alert);
        }
        
        /// <summary>
        /// Obter alertas com filtros
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AlertDto>>> GetAlerts([FromQuery] AlertFilterDto filter)
        {
            var tenantId = GetTenantId();
            var alerts = await _alertService.GetAlertsAsync(filter, tenantId);
            
            return Ok(alerts);
        }
        
        /// <summary>
        /// Obter alertas ativos para o usuário atual
        /// </summary>
        [HttpGet("my-alerts")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AlertDto>>> GetMyAlerts()
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var alerts = await _alertService.GetActiveAlertsForUserAsync(userId, tenantId);
            
            return Ok(alerts);
        }
        
        /// <summary>
        /// Obter contagem de alertas ativos
        /// </summary>
        [HttpGet("my-alerts/count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetMyAlertCount()
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            var count = await _alertService.GetActiveAlertCountForUserAsync(userId, tenantId);
            
            return Ok(new { count });
        }
        
        /// <summary>
        /// Obter alertas críticos
        /// </summary>
        [HttpGet("critical")]
        [ProducesResponseType(typeof(List<AlertDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<AlertDto>>> GetCriticalAlerts()
        {
            var tenantId = GetTenantId();
            var alerts = await _alertService.GetCriticalAlertsAsync(tenantId);
            
            return Ok(alerts);
        }
        
        /// <summary>
        /// Obter contagem de alertas críticos
        /// </summary>
        [HttpGet("critical/count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<ActionResult<object>> GetCriticalAlertCount()
        {
            var tenantId = GetTenantId();
            var count = await _alertService.GetCriticalAlertCountAsync(tenantId);
            
            return Ok(new { count });
        }
        
        /// <summary>
        /// Obter estatísticas de alertas
        /// </summary>
        [HttpGet("statistics")]
        [ProducesResponseType(typeof(AlertStatisticsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AlertStatisticsDto>> GetStatistics()
        {
            var tenantId = GetTenantId();
            var stats = await _alertService.GetAlertStatisticsAsync(tenantId);
            
            return Ok(stats);
        }
        
        /// <summary>
        /// Reconhecer um alerta
        /// </summary>
        [HttpPost("{id}/acknowledge")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> AcknowledgeAlert(Guid id)
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            
            try
            {
                await _alertService.AcknowledgeAlertAsync(id, userId, tenantId);
                return Ok(new { message = "Alert acknowledged successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Alert not found" });
            }
        }
        
        /// <summary>
        /// Resolver um alerta
        /// </summary>
        [HttpPost("{id}/resolve")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> ResolveAlert(Guid id, [FromBody] ResolveAlertDto dto)
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            
            try
            {
                await _alertService.ResolveAlertAsync(id, userId, dto.Notes, tenantId);
                return Ok(new { message = "Alert resolved successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Alert not found" });
            }
        }
        
        /// <summary>
        /// Dispensar um alerta
        /// </summary>
        [HttpPost("{id}/dismiss")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DismissAlert(Guid id)
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            
            try
            {
                await _alertService.DismissAlertAsync(id, userId, tenantId);
                return Ok(new { message = "Alert dismissed successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Alert not found" });
            }
        }
    }
}
