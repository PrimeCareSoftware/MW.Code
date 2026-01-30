using System;
using System.Linq;
using System.Security.Claims;
using System.Text;
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
        private readonly ISuspiciousActivityDetector _suspiciousActivityDetector;

        public AuditController(
            ITenantContext tenantContext,
            IAuditService auditService,
            ISuspiciousActivityDetector suspiciousActivityDetector) : base(tenantContext)
        {
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _suspiciousActivityDetector = suspiciousActivityDetector ?? throw new ArgumentNullException(nameof(suspiciousActivityDetector));
        }

        /// <summary>
        /// Obtém atividades de um usuário específico
        /// </summary>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
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
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
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
            var isAdmin = User.IsInRole("SystemAdmin") || User.IsInRole("ClinicOwner");

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
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
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
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
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

        /// <summary>
        /// Exporta logs de auditoria para CSV
        /// </summary>
        [HttpGet("export/csv")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> ExportToCsv([FromQuery] AuditFilterRequest filterRequest)
        {
            var filter = MapToAuditFilter(filterRequest);
            filter.TenantId = GetTenantId();

            var csv = await _auditService.ExportToCsvAsync(filter);
            var bytes = Encoding.UTF8.GetBytes(csv);
            
            return File(bytes, "text/csv", $"audit_logs_{DateTime.UtcNow:yyyyMMddHHmmss}.csv");
        }

        /// <summary>
        /// Exporta logs de auditoria para JSON
        /// </summary>
        [HttpGet("export/json")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> ExportToJson([FromQuery] AuditFilterRequest filterRequest)
        {
            var filter = MapToAuditFilter(filterRequest);
            filter.TenantId = GetTenantId();

            var json = await _auditService.ExportToJsonAsync(filter);
            var bytes = Encoding.UTF8.GetBytes(json);
            
            return File(bytes, "application/json", $"audit_logs_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Gera relatório de compliance LGPD para um usuário específico
        /// </summary>
        [HttpGet("export/lgpd/{userId}")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> ExportLgpdReport(string userId)
        {
            var tenantId = GetTenantId();
            var json = await _auditService.ExportLgpdComplianceReportAsync(userId, tenantId);
            var bytes = Encoding.UTF8.GetBytes(json);
            
            return File(bytes, "application/json", $"lgpd_report_{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}.json");
        }

        /// <summary>
        /// Detecta atividades suspeitas
        /// </summary>
        [HttpGet("suspicious-activity")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetSuspiciousActivity()
        {
            var tenantId = GetTenantId();
            var alerts = await _suspiciousActivityDetector.DetectSuspiciousActivityAsync(tenantId);
            
            return Ok(new
            {
                TotalAlerts = alerts.Count,
                CriticalAlerts = alerts.Count(a => a.Severity == Application.Services.SecurityAlertSeverity.Critical),
                HighAlerts = alerts.Count(a => a.Severity == Application.Services.SecurityAlertSeverity.High),
                MediumAlerts = alerts.Count(a => a.Severity == Application.Services.SecurityAlertSeverity.Medium),
                LowAlerts = alerts.Count(a => a.Severity == Application.Services.SecurityAlertSeverity.Low),
                Alerts = alerts
            });
        }

        /// <summary>
        /// Obtém alertas de segurança ativos
        /// </summary>
        [HttpGet("security-alerts")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetSecurityAlerts()
        {
            var tenantId = GetTenantId();
            var alerts = await _suspiciousActivityDetector.GetActiveAlertsAsync(tenantId);
            
            return Ok(alerts.OrderByDescending(a => a.Severity).ThenByDescending(a => a.DetectedAt));
        }

        /// <summary>
        /// Obtém estatísticas de auditoria
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var tenantId = GetTenantId();
            var stats = await _auditService.GetStatisticsAsync(tenantId, startDate, endDate);
            
            return Ok(stats);
        }

        /// <summary>
        /// Obtém informações sobre política de retenção
        /// </summary>
        [HttpGet("retention-policy")]
        [Authorize(Roles = "SystemAdmin")]
        public IActionResult GetRetentionPolicy()
        {
            return Ok(new
            {
                RetentionDays = 2555, // 7 years
                RetentionYears = 7,
                Description = "Audit logs are retained for 7 years (2555 days) as required by LGPD",
                AutomaticCleanup = true,
                CleanupSchedule = "Daily at 2:00 AM UTC"
            });
        }

        /// <summary>
        /// Aplica política de retenção manualmente
        /// </summary>
        [HttpPost("apply-retention")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApplyRetention([FromQuery] int? retentionDays)
        {
            var tenantId = GetTenantId();
            var days = retentionDays ?? 2555; // Default 7 years
            
            var deletedCount = await _auditService.ApplyRetentionPolicyAsync(tenantId, days);
            
            return Ok(new
            {
                Message = "Retention policy applied successfully",
                DeletedLogs = deletedCount,
                RetentionDays = days,
                CutoffDate = DateTime.UtcNow.AddDays(-days)
            });
        }

        private AuditFilter MapToAuditFilter(AuditFilterRequest request)
        {
            return new AuditFilter
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                UserId = request.UserId,
                EntityType = request.EntityType,
                EntityId = request.EntityId,
                Action = request.Action.HasValue ? Enum.Parse<AuditAction>(request.Action.Value) : null,
                Result = request.Result.HasValue ? Enum.Parse<OperationResult>(request.Result.Value) : null,
                Severity = request.Severity.HasValue ? Enum.Parse<AuditSeverity>(request.Severity.Value) : null,
                PageNumber = request.PageNumber ?? 1,
                PageSize = request.PageSize ?? 50,
                TenantId = GetTenantId()
            };
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

    public record AuditFilterRequest(
        DateTime? StartDate = null,
        DateTime? EndDate = null,
        string? UserId = null,
        string? EntityType = null,
        string? EntityId = null,
        string? Action = null,
        string? Result = null,
        string? Severity = null,
        int? PageNumber = null,
        int? PageSize = null
    );
}
