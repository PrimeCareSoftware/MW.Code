using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace MedicSoft.CrossCutting.Security
{
    /// <summary>
    /// Middleware to add security headers to HTTP responses
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip security headers for Swagger UI to prevent blank page issues
            // Swagger UI requires 'unsafe-inline' for scripts and styles, and blob: for workers
            var path = context.Request.Path.Value?.ToLowerInvariant() ?? string.Empty;
            if (path.StartsWith("/swagger"))
            {
                await _next(context);
                return;
            }

            // X-Content-Type-Options: Prevent MIME type sniffing
            context.Response.Headers.Add("X-Content-Type-Options", "nosniff");

            // X-Frame-Options: Prevent clickjacking
            context.Response.Headers.Add("X-Frame-Options", "DENY");

            // X-XSS-Protection: Enable XSS filter
            context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");

            // Referrer-Policy: Control referrer information
            context.Response.Headers.Add("Referrer-Policy", "strict-origin-when-cross-origin");

            // Content-Security-Policy: Prevent XSS attacks
            var csp = "default-src 'self'; " +
                      "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
                      "style-src 'self' 'unsafe-inline'; " +
                      "img-src 'self' data: https:; " +
                      "font-src 'self' data:; " +
                      "connect-src 'self'; " +
                      "frame-ancestors 'none';";
            context.Response.Headers.Add("Content-Security-Policy", csp);

            // Permissions-Policy: Control browser features
            var permissionsPolicy = "accelerometer=(), " +
                                   "camera=(), " +
                                   "geolocation=(), " +
                                   "gyroscope=(), " +
                                   "magnetometer=(), " +
                                   "microphone=(), " +
                                   "payment=(), " +
                                   "usb=()";
            context.Response.Headers.Add("Permissions-Policy", permissionsPolicy);

            // Remove server header to avoid revealing server information
            context.Response.Headers.Remove("Server");
            context.Response.Headers.Remove("X-Powered-By");

            await _next(context);
        }
    }

    /// <summary>
    /// Extension method to add security headers middleware
    /// </summary>
    public static class SecurityHeadersMiddlewareExtensions
    {
        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SecurityHeadersMiddleware>();
        }
    }
}
