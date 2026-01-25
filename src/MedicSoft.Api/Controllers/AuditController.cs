using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AuditController : BaseController
    {
        private readonly IAuditService _auditService;

        public AuditController(
            ITenantContext tenantContext,
            IAuditService auditService) : base(tenantContext)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
        }

        /// <summary>
        /// Obtém atividades de um usuário específico
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Admin,SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetUserActivity(
            string userId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var tenantId = GetTenantId();
            var logs = await _auditService.GetUserActivityAsync(userId, startDate, endDate, tenantId);
            return Ok(logs);
        }

        /// <summary>
        /// Obtém histórico de uma entidade específica
        /// </summary>
        [HttpGet("entity/{entityType}/{entityId}")]
        [Authorize(Roles = "Admin,SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetEntityHistory(
            string entityType,
            string entityId)
        {
            var tenantId = GetTenantId();
            var logs = await _auditService.GetEntityHistoryAsync(entityType, entityId, tenantId);
            return Ok(logs);
        }

        /// <summary>
        /// Obtém eventos de segurança
        /// </summary>
        [HttpGet("security-events")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> GetSecurityEvents(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var tenantId = GetTenantId();
            var logs = await _auditService.GetSecurityEventsAsync(startDate, endDate, tenantId);
            return Ok(logs);
        }

        /// <summary>
        /// Gera relatório LGPD para um usuário
        /// Usuário pode ver seu próprio relatório
        /// </summary>
        [HttpGet("lgpd-report/{userId}")]
        public async Task<IActionResult> GetLgpdReport(string userId)
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("SystemAdmin") || User.IsInRole("ClinicOwner");

            // Usuário pode ver seu próprio relatório ou admin pode ver qualquer relatório
            if (currentUserId != userId && !isAdmin)
            {
                return Forbid();
            }

            var tenantId = GetTenantId();
            var report = await _auditService.GenerateLgpdReportAsync(userId, tenantId);
            return Ok(report);
        }

        /// <summary>
        /// Consulta logs de auditoria com filtros avançados
        /// </summary>
        [HttpPost("query")]
        [Authorize(Roles = "Admin,SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> QueryAuditLogs([FromBody] AuditFilter filter)
        {
            // Ensure tenantId is set from authenticated context
            filter.TenantId = GetTenantId();

            var (logs, totalCount) = await _auditService.QueryAsync(filter);
            
            return Ok(new
            {
                Data = logs,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
            });
        }

        /// <summary>
        /// Log manual de auditoria (para casos especiais)
        /// </summary>
        [HttpPost("log")]
        [Authorize(Roles = "Admin,SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> LogManual([FromBody] CreateAuditLogDto dto)
        {
            // Override tenant from authenticated context
            var tenantId = GetTenantId();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            var logDto = dto with
            {
                TenantId = tenantId,
                IpAddress = ipAddress,
                UserAgent = userAgent
            };

            await _auditService.LogAsync(logDto);
            return Ok(new { Message = "Audit log created successfully" });
        }

        /// <summary>
        /// Log de acesso a dados sensíveis
        /// </summary>
        [HttpPost("log-data-access")]
        public async Task<IActionResult> LogDataAccess([FromBody] LogDataAccessRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(ClaimTypes.Name)?.Value;
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var tenantId = GetTenantId();
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
            {
                return Unauthorized();
            }

            await _auditService.LogDataAccessAsync(
                userId,
                userName,
                userEmail,
                request.EntityType,
                request.EntityId,
                request.EntityDisplayName,
                ipAddress,
                userAgent,
                HttpContext.Request.Path,
                HttpContext.Request.Method,
                tenantId,
                request.DataCategory,
                request.Purpose
            );

            return Ok(new { Message = "Data access logged successfully" });
        }
    }

    // Request DTOs
    public record LogDataAccessRequest(
        string EntityType,
        string EntityId,
        string EntityDisplayName,
        DataCategory DataCategory = DataCategory.PERSONAL,
        LgpdPurpose Purpose = LgpdPurpose.HEALTHCARE
    );
}
