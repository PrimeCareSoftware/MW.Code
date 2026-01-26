using System;
using System.IO;
using System.Text;
using System.Text.Json;
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
    /// LGPD Compliance Middleware - Logs all sensitive data operations
    /// Implements LGPD Art. 37 - Record keeping of data processing operations
    /// </summary>
    public class LgpdAuditMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LgpdAuditMiddleware> _logger;

        // Sensitive endpoints that require audit logging
        private static readonly string[] SensitiveEndpoints = new[]
        {
            "/api/patients",
            "/api/medical-records",
            "/api/appointments",
            "/api/prescriptions",
            "/api/digital-prescriptions",
            "/api/exam-requests",
            "/api/informed-consents",
            "/api/consent",
            "/api/data-portability",
            "/api/data-deletion",
            "/api/health-insurance"
        };

        public LgpdAuditMiddleware(RequestDelegate next, ILogger<LgpdAuditMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IAuditService auditService, ITenantContext tenantContext)
        {
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? "";
            var method = context.Request.Method;

            // Check if this is a sensitive endpoint that requires audit logging
            if (IsSensitiveEndpoint(path))
            {
                // Capture request details before processing
                var requestDetails = await CaptureRequestDetailsAsync(context);

                // Store original response body stream
                var originalBodyStream = context.Response.Body;

                try
                {
                    // Use a memory stream to capture response
                    using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    // Process the request
                    await _next(context);

                    // Log the audit after successful request
                    if (context.Response.StatusCode < 400)
                    {
                        await LogAuditAsync(
                            auditService,
                            context,
                            tenantContext.TenantId,
                            requestDetails,
                            OperationResult.SUCCESS
                        );
                    }
                    else if (context.Response.StatusCode == 401 || context.Response.StatusCode == 403)
                    {
                        await LogAuditAsync(
                            auditService,
                            context,
                            tenantContext.TenantId,
                            requestDetails,
                            OperationResult.UNAUTHORIZED
                        );
                    }
                    else
                    {
                        await LogAuditAsync(
                            auditService,
                            context,
                            tenantContext.TenantId,
                            requestDetails,
                            OperationResult.FAILED
                        );
                    }

                    // Copy the response back to the original stream
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in LGPD audit middleware for path {Path}", path);
                    
                    // Log failed operation
                    try
                    {
                        await LogAuditAsync(
                            auditService,
                            context,
                            tenantContext.TenantId,
                            requestDetails,
                            OperationResult.FAILED
                        );
                    }
                    catch (Exception auditEx)
                    {
                        _logger.LogError(auditEx, "Failed to log audit for exception in path {Path}", path);
                    }

                    // Restore original response body
                    context.Response.Body = originalBodyStream;
                    throw;
                }
                finally
                {
                    context.Response.Body = originalBodyStream;
                }
            }
            else
            {
                // Non-sensitive endpoint, just continue
                await _next(context);
            }
        }

        private bool IsSensitiveEndpoint(string path)
        {
            foreach (var endpoint in SensitiveEndpoints)
            {
                if (path.Contains(endpoint, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        private async Task<RequestDetails> CaptureRequestDetailsAsync(HttpContext context)
        {
            var details = new RequestDetails
            {
                Path = context.Request.Path.Value ?? "",
                Method = context.Request.Method,
                QueryString = context.Request.QueryString.Value ?? "",
                IpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                UserId = GetUserIdFromContext(context),
                UserName = GetUserNameFromContext(context),
                UserEmail = GetUserEmailFromContext(context)
            };

            return details;
        }

        private async Task LogAuditAsync(
            IAuditService auditService,
            HttpContext context,
            string tenantId,
            RequestDetails requestDetails,
            OperationResult result)
        {
            try
            {
                var action = DetermineAuditAction(requestDetails.Method, requestDetails.Path);
                var entityType = ExtractEntityType(requestDetails.Path);
                var entityId = ExtractEntityId(requestDetails.Path);
                var dataCategory = DetermineDataCategory(entityType);
                var purpose = DetermineLgpdPurpose(action);
                var severity = DetermineSeverity(action, result);

                // Only log if we have valid user context
                if (string.IsNullOrEmpty(requestDetails.UserId))
                {
                    _logger.LogWarning("Skipping audit log - no user context for path {Path}", requestDetails.Path);
                    return;
                }

                var dto = new CreateAuditLogDto(
                    UserId: requestDetails.UserId,
                    UserName: requestDetails.UserName,
                    UserEmail: requestDetails.UserEmail,
                    Action: action,
                    ActionDescription: $"{action} on {entityType}",
                    EntityType: entityType,
                    EntityId: entityId ?? "unknown",
                    EntityDisplayName: null,
                    IpAddress: requestDetails.IpAddress,
                    UserAgent: requestDetails.UserAgent,
                    RequestPath: requestDetails.Path,
                    HttpMethod: requestDetails.Method,
                    Result: result,
                    DataCategory: dataCategory,
                    Purpose: purpose,
                    Severity: severity,
                    TenantId: tenantId
                );

                await auditService.LogAsync(dto);

                _logger.LogInformation(
                    "LGPD Audit logged: {Action} on {EntityType} by user {UserId} - Result: {Result}",
                    action, entityType, requestDetails.UserId, result
                );
            }
            catch (Exception ex)
            {
                // Don't fail the request if audit logging fails
                _logger.LogError(ex, "Failed to log LGPD audit for path {Path}", requestDetails.Path);
            }
        }

        private string GetUserIdFromContext(HttpContext context)
        {
            var userIdClaim = context.User?.FindFirst("sub")?.Value
                            ?? context.User?.FindFirst("userId")?.Value
                            ?? context.User?.FindFirst("id")?.Value;

            return userIdClaim ?? "anonymous";
        }

        private string GetUserNameFromContext(HttpContext context)
        {
            return context.User?.FindFirst("name")?.Value
                   ?? context.User?.FindFirst("username")?.Value
                   ?? "Unknown User";
        }

        private string GetUserEmailFromContext(HttpContext context)
        {
            return context.User?.FindFirst("email")?.Value
                   ?? "unknown@example.com";
        }

        private AuditAction DetermineAuditAction(string method, string path)
        {
            // Check for specific actions in path
            if (path.Contains("/export", StringComparison.OrdinalIgnoreCase))
                return AuditAction.EXPORT;

            if (path.Contains("/consent", StringComparison.OrdinalIgnoreCase))
                return AuditAction.DATA_ACCESS_REQUEST;

            if (path.Contains("/data-deletion", StringComparison.OrdinalIgnoreCase))
                return AuditAction.DATA_DELETION_REQUEST;

            if (path.Contains("/data-portability", StringComparison.OrdinalIgnoreCase))
                return AuditAction.DATA_PORTABILITY_REQUEST;

            // Standard CRUD operations
            return method switch
            {
                "GET" => AuditAction.READ,
                "POST" => AuditAction.CREATE,
                "PUT" or "PATCH" => AuditAction.UPDATE,
                "DELETE" => AuditAction.DELETE,
                _ => AuditAction.READ
            };
        }

        private string ExtractEntityType(string path)
        {
            if (path.Contains("/patients")) return "Patient";
            if (path.Contains("/medical-records")) return "MedicalRecord";
            if (path.Contains("/appointments")) return "Appointment";
            if (path.Contains("/prescriptions")) return "Prescription";
            if (path.Contains("/exam-requests")) return "ExamRequest";
            if (path.Contains("/consent")) return "Consent";
            if (path.Contains("/data-portability")) return "DataPortability";
            if (path.Contains("/data-deletion")) return "DataDeletion";
            if (path.Contains("/health-insurance")) return "HealthInsurance";

            return "Unknown";
        }

        private string? ExtractEntityId(string path)
        {
            // Try to extract GUID from path
            var segments = path.Split('/');
            foreach (var segment in segments)
            {
                if (Guid.TryParse(segment, out var guid))
                {
                    return guid.ToString();
                }
            }
            return null;
        }

        private DataCategory DetermineDataCategory(string entityType)
        {
            return entityType switch
            {
                "Patient" => DataCategory.PERSONAL,
                "MedicalRecord" => DataCategory.SENSITIVE,
                "Prescription" => DataCategory.SENSITIVE,
                "ExamRequest" => DataCategory.SENSITIVE,
                "Appointment" => DataCategory.PERSONAL,
                "Consent" => DataCategory.PERSONAL,
                "DataPortability" => DataCategory.PUBLIC,
                "DataDeletion" => DataCategory.PUBLIC,
                "HealthInsurance" => DataCategory.PERSONAL,
                _ => DataCategory.PUBLIC
            };
        }

        private LgpdPurpose DetermineLgpdPurpose(AuditAction action)
        {
            return action switch
            {
                AuditAction.DATA_ACCESS_REQUEST => LgpdPurpose.CONSENT,
                AuditAction.DATA_PORTABILITY_REQUEST or AuditAction.EXPORT => LgpdPurpose.LEGAL_OBLIGATION,
                AuditAction.DATA_DELETION_REQUEST => LgpdPurpose.LEGAL_OBLIGATION,
                _ => LgpdPurpose.LEGITIMATE_INTEREST
            };
        }

        private AuditSeverity DetermineSeverity(AuditAction action, OperationResult result)
        {
            if (result == OperationResult.UNAUTHORIZED || result == OperationResult.FAILED)
            {
                return AuditSeverity.ERROR;
            }

            return action switch
            {
                AuditAction.DELETE or AuditAction.DATA_DELETION_REQUEST => AuditSeverity.WARNING,
                AuditAction.UPDATE or AuditAction.EXPORT or AuditAction.DATA_PORTABILITY_REQUEST => AuditSeverity.WARNING,
                _ => AuditSeverity.INFO
            };
        }

        private class RequestDetails
        {
            public string Path { get; set; } = "";
            public string Method { get; set; } = "";
            public string QueryString { get; set; } = "";
            public string IpAddress { get; set; } = "";
            public string UserAgent { get; set; } = "";
            public string UserId { get; set; } = "";
            public string UserName { get; set; } = "";
            public string UserEmail { get; set; } = "";
        }
    }
}
