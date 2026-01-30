using System;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware para auditoria automática de todas as requisições HTTP
    /// Compliance: LGPD Art. 37 - Registro de acesso a dados pessoais
    /// </summary>
    public class AutomaticAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<AutomaticAuditMiddleware> _logger;

        public AutomaticAuditMiddleware(
            RequestDelegate next,
            ILogger<AutomaticAuditMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(
            HttpContext context,
            IAuditService auditService,
            ITenantContext tenantContext)
        {
            if (!ShouldAudit(context))
            {
                await _next(context);
                return;
            }

            var startTime = DateTime.UtcNow;
            var originalBodyStream = context.Response.Body;

            try
            {
                using var responseBody = new MemoryStream();
                context.Response.Body = responseBody;

                await _next(context);

                var statusCode = context.Response.StatusCode;
                var duration = (DateTime.UtcNow - startTime).TotalMilliseconds;

                // Log the audit after request completes
                await LogAuditAsync(context, auditService, tenantContext, statusCode);

                await responseBody.CopyToAsync(originalBodyStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AutomaticAuditMiddleware");
                await LogAuditAsync(context, auditService, tenantContext, 500, ex.Message);
                throw;
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }

        private bool ShouldAudit(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var method = context.Request.Method.ToUpperInvariant();

            // Skip health checks, static files, and non-sensitive GET requests
            if (path.StartsWith("/health") ||
                path.StartsWith("/swagger") ||
                path.StartsWith("/api/swagger") ||
                path.StartsWith("/_framework") ||
                path.StartsWith("/css") ||
                path.StartsWith("/js") ||
                path.StartsWith("/img") ||
                path.StartsWith("/favicon"))
            {
                return false;
            }

            // Always audit sensitive operations (POST, PUT, DELETE, PATCH)
            if (method == "POST" || method == "PUT" || method == "DELETE" || method == "PATCH")
            {
                return true;
            }

            // Audit GET requests to sensitive endpoints
            if (method == "GET" && IsSensitiveEndpoint(path))
            {
                return true;
            }

            return false;
        }

        private bool IsSensitiveEndpoint(string path)
        {
            var sensitivePatterns = new[]
            {
                "/api/patients",
                "/api/medicalrecords",
                "/api/prescriptions",
                "/api/attendances",
                "/api/exams",
                "/api/users",
                "/api/auth",
                "/api/lgpd",
                "/api/audit",
                "/api/financial",
                "/api/appointments"
            };

            foreach (var pattern in sensitivePatterns)
            {
                if (path.Contains(pattern))
                {
                    return true;
                }
            }

            return false;
        }

        private async Task LogAuditAsync(
            HttpContext context,
            IAuditService auditService,
            ITenantContext tenantContext,
            int statusCode,
            string? failureReason = null)
        {
            try
            {
                var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                var userName = context.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous";
                var userEmail = context.User?.FindFirst(ClaimTypes.Email)?.Value ?? "unknown@unknown.com";
                var tenantId = tenantContext.TenantId ?? "system";
                var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var userAgent = context.Request.Headers["User-Agent"].ToString();
                var requestPath = context.Request.Path.Value ?? "/";
                var httpMethod = context.Request.Method;

                var action = MapHttpMethodToAuditAction(httpMethod);
                var result = statusCode >= 200 && statusCode < 300 
                    ? OperationResult.SUCCESS 
                    : OperationResult.FAILED;
                var severity = DetermineSeverity(httpMethod, statusCode);

                var dto = new CreateAuditLogDto(
                    UserId: userId,
                    UserName: userName,
                    UserEmail: userEmail,
                    Action: action,
                    ActionDescription: $"{httpMethod} {requestPath}",
                    EntityType: ExtractEntityType(requestPath),
                    EntityId: ExtractEntityId(requestPath) ?? "N/A",
                    EntityDisplayName: null,
                    IpAddress: ipAddress,
                    UserAgent: userAgent,
                    RequestPath: requestPath,
                    HttpMethod: httpMethod,
                    Result: result,
                    DataCategory: DetermineDataCategory(requestPath),
                    Purpose: LgpdPurpose.HEALTHCARE,
                    Severity: severity,
                    TenantId: tenantId,
                    StatusCode: statusCode,
                    FailureReason: failureReason
                );

                await auditService.LogAsync(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log audit entry");
            }
        }

        private AuditAction MapHttpMethodToAuditAction(string httpMethod)
        {
            return httpMethod.ToUpperInvariant() switch
            {
                "POST" => AuditAction.CREATE,
                "GET" => AuditAction.READ,
                "PUT" => AuditAction.UPDATE,
                "PATCH" => AuditAction.UPDATE,
                "DELETE" => AuditAction.DELETE,
                _ => AuditAction.READ
            };
        }

        private AuditSeverity DetermineSeverity(string httpMethod, int statusCode)
        {
            if (statusCode >= 500)
                return AuditSeverity.CRITICAL;
            if (statusCode >= 400)
                return AuditSeverity.WARNING;
            if (httpMethod == "DELETE")
                return AuditSeverity.WARNING;
            if (httpMethod == "POST" || httpMethod == "PUT" || httpMethod == "PATCH")
                return AuditSeverity.INFO;
            
            return AuditSeverity.DEBUG;
        }

        private DataCategory DetermineDataCategory(string path)
        {
            var lowerPath = path.ToLowerInvariant();

            if (lowerPath.Contains("/medicalrecord") || 
                lowerPath.Contains("/prescription") || 
                lowerPath.Contains("/exam") ||
                lowerPath.Contains("/attendance"))
                return DataCategory.HEALTH;

            if (lowerPath.Contains("/patient") || 
                lowerPath.Contains("/user") ||
                lowerPath.Contains("/auth"))
                return DataCategory.PERSONAL;

            if (lowerPath.Contains("/financial") || 
                lowerPath.Contains("/payment"))
                return DataCategory.FINANCIAL;

            return DataCategory.SYSTEM;
        }

        private string ExtractEntityType(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length >= 2 && segments[0].Equals("api", StringComparison.OrdinalIgnoreCase))
            {
                return segments[1];
            }
            return "Unknown";
        }

        private string? ExtractEntityId(string path)
        {
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            
            // Try to find a GUID or numeric ID in the path
            for (int i = 2; i < segments.Length; i++)
            {
                if (Guid.TryParse(segments[i], out _) || int.TryParse(segments[i], out _))
                {
                    return segments[i];
                }
            }
            
            return null;
        }
    }
}
