using Microsoft.Extensions.Primitives;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware to extract tenant information from subdomain or path
    /// Supports formats: subdomain.domain.com or domain.com/subdomain
    /// </summary>
    public class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(RequestDelegate next, ILogger<TenantResolutionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, IClinicRepository clinicRepository)
        {
            string? tenantId = null;
            string? subdomain = null;

            // Try to extract subdomain from host
            var host = context.Request.Host.Host;
            subdomain = ExtractSubdomainFromHost(host);

            if (!string.IsNullOrEmpty(subdomain))
            {
                _logger.LogInformation("Extracted subdomain from host: {Subdomain}", subdomain);
                
                // Look up clinic by subdomain
                var clinic = await clinicRepository.GetBySubdomainAsync(subdomain);
                if (clinic != null)
                {
                    tenantId = clinic.TenantId;
                    _logger.LogInformation("Resolved subdomain {Subdomain} to tenantId: {TenantId}", subdomain, tenantId);
                }
                else
                {
                    _logger.LogWarning("No active clinic found for subdomain: {Subdomain}", subdomain);
                }
            }

            // If subdomain resolution failed, try path-based routing (e.g., /subdomain/login)
            if (string.IsNullOrEmpty(tenantId))
            {
                subdomain = ExtractSubdomainFromPath(context.Request.Path);
                if (!string.IsNullOrEmpty(subdomain))
                {
                    _logger.LogInformation("Extracted subdomain from path: {Subdomain}", subdomain);
                    
                    var clinic = await clinicRepository.GetBySubdomainAsync(subdomain);
                    if (clinic != null)
                    {
                        tenantId = clinic.TenantId;
                        _logger.LogInformation("Resolved path subdomain {Subdomain} to tenantId: {TenantId}", subdomain, tenantId);
                    }
                    else
                    {
                        _logger.LogWarning("No active clinic found for path subdomain: {Subdomain}", subdomain);
                    }
                }
            }

            // Store resolved tenant information in HttpContext
            if (!string.IsNullOrEmpty(tenantId))
            {
                context.Items["TenantId"] = tenantId;
                context.Items["Subdomain"] = subdomain;
                
                // Also add to request headers for easy access by controllers
                if (!context.Request.Headers.ContainsKey("X-Tenant-Id"))
                {
                    context.Request.Headers["X-Tenant-Id"] = tenantId;
                }
                if (!string.IsNullOrEmpty(subdomain) && !context.Request.Headers.ContainsKey("X-Subdomain"))
                {
                    context.Request.Headers["X-Subdomain"] = subdomain;
                }
            }

            await _next(context);
        }

        private string? ExtractSubdomainFromHost(string host)
        {
            if (string.IsNullOrEmpty(host))
                return null;

            // Skip plain localhost and IP addresses
            if (host == "localhost" || host.StartsWith("127.") || host.StartsWith("192.168."))
                return null;

            var parts = host.Split('.');
            
            // Support subdomain.localhost format (2 parts)
            // Also support subdomain.domain.com format (3+ parts)
            if (parts.Length >= 2)
            {
                // Return first part as subdomain, but exclude 'www'
                var subdomain = parts[0].ToLowerInvariant();
                return subdomain == "www" ? null : subdomain;
            }

            return null;
        }

        private string? ExtractSubdomainFromPath(string path)
        {
            if (string.IsNullOrEmpty(path) || path == "/")
                return null;

            // Extract first path segment (e.g., /subdomain/login -> subdomain)
            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length > 0)
            {
                var firstSegment = segments[0].ToLowerInvariant();
                
                // Exclude common API paths
                var excludedPaths = new[] { "api", "swagger", "health", "assets" };
                if (!excludedPaths.Contains(firstSegment))
                {
                    return firstSegment;
                }
            }

            return null;
        }
    }

    public static class TenantResolutionMiddlewareExtensions
    {
        public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<TenantResolutionMiddleware>();
        }
    }
}
