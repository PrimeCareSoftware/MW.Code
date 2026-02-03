using System;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Controllers.CRM
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin")]
    public class SalesforceLeadsController : BaseController
    {
        private readonly ISalesforceLeadService _salesforceLeadService;
        private readonly ILogger<SalesforceLeadsController> _logger;

        public SalesforceLeadsController(
            ISalesforceLeadService salesforceLeadService,
            ITenantContext tenantContext,
            ILogger<SalesforceLeadsController> logger) : base(tenantContext)
        {
            _salesforceLeadService = salesforceLeadService;
            _logger = logger;
        }

        /// <summary>
        /// Get all unsynced leads
        /// </summary>
        [HttpGet("unsynced")]
        public async Task<IActionResult> GetUnsyncedLeads()
        {
            try
            {
                var leads = await _salesforceLeadService.GetUnsyncedLeadsAsync();
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unsynced leads");
                return StatusCode(500, new { message = "Erro ao buscar leads não sincronizados" });
            }
        }

        /// <summary>
        /// Get leads by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetLeadsByStatus(LeadStatus status)
        {
            try
            {
                var leads = await _salesforceLeadService.GetLeadsByStatusAsync(status);
                return Ok(leads);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting leads by status {Status}", status);
                return StatusCode(500, new { message = "Erro ao buscar leads por status" });
            }
        }

        /// <summary>
        /// Get lead statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var stats = await _salesforceLeadService.GetLeadStatisticsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting lead statistics");
                return StatusCode(500, new { message = "Erro ao buscar estatísticas de leads" });
            }
        }

        /// <summary>
        /// Create lead from abandoned funnel session
        /// </summary>
        [HttpPost("create-from-funnel/{sessionId}")]
        public async Task<IActionResult> CreateLeadFromFunnel(string sessionId)
        {
            try
            {
                var lead = await _salesforceLeadService.CreateLeadFromFunnelAsync(sessionId);
                return Ok(lead);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lead from funnel session {SessionId}", sessionId);
                return StatusCode(500, new { message = "Erro ao criar lead" });
            }
        }

        /// <summary>
        /// Sync a specific lead to Salesforce
        /// </summary>
        [HttpPost("sync/{leadId}")]
        public async Task<IActionResult> SyncLead(Guid leadId)
        {
            try
            {
                var success = await _salesforceLeadService.SyncLeadToSalesforceAsync(leadId);
                if (success)
                {
                    return Ok(new { message = "Lead sincronizado com sucesso" });
                }
                return BadRequest(new { message = "Falha ao sincronizar lead" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing lead {LeadId}", leadId);
                return StatusCode(500, new { message = "Erro ao sincronizar lead" });
            }
        }

        /// <summary>
        /// Sync all unsynced leads to Salesforce
        /// </summary>
        [HttpPost("sync-all")]
        public async Task<IActionResult> SyncAllLeads()
        {
            try
            {
                var result = await _salesforceLeadService.SyncAllLeadsAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error syncing all leads");
                return StatusCode(500, new { message = "Erro ao sincronizar todos os leads" });
            }
        }

        /// <summary>
        /// Update lead status
        /// </summary>
        [HttpPut("{leadId}/status")]
        public async Task<IActionResult> UpdateLeadStatus(Guid leadId, [FromBody] UpdateLeadStatusRequest request)
        {
            try
            {
                var success = await _salesforceLeadService.UpdateLeadStatusAsync(leadId, request.Status);
                if (success)
                {
                    return Ok(new { message = "Status do lead atualizado com sucesso" });
                }
                return NotFound(new { message = "Lead não encontrado" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead {LeadId} status", leadId);
                return StatusCode(500, new { message = "Erro ao atualizar status do lead" });
            }
        }

        /// <summary>
        /// Test Salesforce connection
        /// </summary>
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            try
            {
                var isConnected = await _salesforceLeadService.TestConnectionAsync();
                return Ok(new { connected = isConnected });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing Salesforce connection");
                return Ok(new { connected = false, error = ex.Message });
            }
        }
    }

    public class UpdateLeadStatusRequest
    {
        public LeadStatus Status { get; set; }
    }
}
