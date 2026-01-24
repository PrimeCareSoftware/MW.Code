using System.Text.RegularExpressions;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// CFM 1.638/2002 - Middleware to automatically log all accesses to medical records
    /// Tracks View, Edit, Close, Reopen, Print, Export operations for audit compliance
    /// </summary>
    public class MedicalRecordAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<MedicalRecordAuditMiddleware> _logger;

        public MedicalRecordAuditMiddleware(RequestDelegate next, ILogger<MedicalRecordAuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IMedicalRecordAuditService auditService, ITenantContext tenantContext)
        {
            var path = context.Request.Path.Value;
            var method = context.Request.Method;

            // Check if this is a medical records request
            if (!string.IsNullOrEmpty(path) && path.Contains("/api/medical-records/", StringComparison.OrdinalIgnoreCase))
            {
                // Extract medical record ID if present in the path
                var recordId = ExtractRecordId(path);
                
                if (recordId != null && recordId != Guid.Empty)
                {
                    try
                    {
                        var userId = GetUserIdFromContext(context);
                        var accessType = DetermineAccessType(method, path);
                        var ipAddress = context.Connection.RemoteIpAddress?.ToString();
                        var userAgent = context.Request.Headers["User-Agent"].ToString();
                        var tenantId = tenantContext.TenantId;

                        // Log access in background (non-blocking, doesn't capture sync context)
                        _ = LogAccessInBackgroundAsync(auditService, recordId.Value, userId, accessType, tenantId, ipAddress, userAgent);
                    }
                    catch (Exception ex)
                    {
                        // Don't fail the request if audit logging fails
                        _logger.LogError(ex, "Error setting up audit logging for medical record access");
                    }
                }
            }

            await _next(context);
        }

        private Guid? ExtractRecordId(string path)
        {
            // Pattern: /api/medical-records/{id}/...
            var match = Regex.Match(path, @"/api/medical-records/([0-9a-fA-F-]{36})(/|$)", RegexOptions.IgnoreCase);
            
            if (match.Success && Guid.TryParse(match.Groups[1].Value, out var recordId))
            {
                return recordId;
            }

            return null;
        }

        private Guid GetUserIdFromContext(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst("sub")?.Value 
                            ?? context.User?.FindFirst("userId")?.Value 
                            ?? context.User?.FindFirst("id")?.Value;
            
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }

            return Guid.Empty;
        }

        private string DetermineAccessType(string method, string path)
        {
            // Determine access type based on HTTP method and path
            if (path.Contains("/close", StringComparison.OrdinalIgnoreCase))
                return "Close";
            
            if (path.Contains("/reopen", StringComparison.OrdinalIgnoreCase))
                return "Reopen";
            
            if (path.Contains("/versions", StringComparison.OrdinalIgnoreCase))
                return "ViewVersions";
            
            if (path.Contains("/access-logs", StringComparison.OrdinalIgnoreCase))
                return "ViewAccessLogs";
            
            if (path.Contains("/print", StringComparison.OrdinalIgnoreCase))
                return "Print";
            
            if (path.Contains("/export", StringComparison.OrdinalIgnoreCase))
                return "Export";

            return method switch
            {
                "GET" => "View",
                "PUT" or "PATCH" => "Edit",
                "POST" => "Edit",
                "DELETE" => "Delete",
                _ => "Unknown"
            };
        }

        private async Task LogAccessInBackgroundAsync(
            IMedicalRecordAuditService auditService,
            Guid recordId,
            Guid userId,
            string accessType,
            string tenantId,
            string? ipAddress,
            string? userAgent)
        {
            try
            {
                await auditService.LogAccessAsync(
                    recordId: recordId,
                    userId: userId,
                    accessType: accessType,
                    tenantId: tenantId,
                    ipAddress: ipAddress,
                    userAgent: userAgent
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to log medical record access for record {RecordId}", recordId);
            }
        }
    }
}
