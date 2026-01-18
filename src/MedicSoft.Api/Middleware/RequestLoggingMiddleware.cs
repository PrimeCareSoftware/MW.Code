using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware para logging detalhado de requisições HTTP
    /// Monitora tempo de execução, detecta requisições lentas e captura informações relevantes
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;
        private readonly int _slowRequestThresholdMs;

        public RequestLoggingMiddleware(
            RequestDelegate next,
            ILogger<RequestLoggingMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _slowRequestThresholdMs = configuration.GetValue<int>("Monitoring:SlowRequestThresholdMs", 3000);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var requestId = Guid.NewGuid().ToString();
            var stopwatch = Stopwatch.StartNew();
            
            // Enrich logging context with request information
            using (_logger.BeginScope(new Dictionary<string, object>
            {
                ["RequestId"] = requestId,
                ["RequestPath"] = context.Request.Path,
                ["RequestMethod"] = context.Request.Method,
                ["UserAgent"] = context.Request.Headers["User-Agent"].ToString(),
                ["RemoteIpAddress"] = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                ["TenantId"] = context.Items["TenantId"]?.ToString() ?? "None",
                ["UserId"] = context.User?.Identity?.Name ?? "Anonymous"
            }))
            {
                try
                {
                    _logger.LogInformation(
                        "Request initiated: {Method} {Path} from {IpAddress}",
                        context.Request.Method,
                        context.Request.Path,
                        context.Connection.RemoteIpAddress);

                    // Capture original response body
                    var originalBodyStream = context.Response.Body;
                    using var responseBody = new MemoryStream();
                    context.Response.Body = responseBody;

                    await _next(context);

                    stopwatch.Stop();

                    // Log response details
                    var elapsedMs = stopwatch.ElapsedMilliseconds;
                    var statusCode = context.Response.StatusCode;
                    var responseSize = responseBody.Length;

                    // Detect slow requests
                    if (elapsedMs > _slowRequestThresholdMs)
                    {
                        _logger.LogWarning(
                            "SLOW REQUEST DETECTED: {Method} {Path} took {ElapsedMs}ms (threshold: {ThresholdMs}ms) - Status: {StatusCode}, Size: {Size} bytes",
                            context.Request.Method,
                            context.Request.Path,
                            elapsedMs,
                            _slowRequestThresholdMs,
                            statusCode,
                            responseSize);
                    }
                    else
                    {
                        _logger.LogInformation(
                            "Request completed: {Method} {Path} - Status: {StatusCode}, Duration: {ElapsedMs}ms, Size: {Size} bytes",
                            context.Request.Method,
                            context.Request.Path,
                            statusCode,
                            elapsedMs,
                            responseSize);
                    }

                    // Log error responses with details
                    if (statusCode >= 400)
                    {
                        responseBody.Seek(0, SeekOrigin.Begin);
                        var responseContent = await new StreamReader(responseBody).ReadToEndAsync();
                        
                        _logger.LogWarning(
                            "Error response: {Method} {Path} - Status: {StatusCode}, Duration: {ElapsedMs}ms, Response: {Response}",
                            context.Request.Method,
                            context.Request.Path,
                            statusCode,
                            elapsedMs,
                            responseContent);
                    }

                    // Copy response back to original stream
                    responseBody.Seek(0, SeekOrigin.Begin);
                    await responseBody.CopyToAsync(originalBodyStream);
                }
                catch (Exception ex)
                {
                    stopwatch.Stop();
                    _logger.LogError(
                        ex,
                        "Request failed: {Method} {Path} - Duration: {ElapsedMs}ms, Error: {ErrorMessage}",
                        context.Request.Method,
                        context.Request.Path,
                        stopwatch.ElapsedMilliseconds,
                        ex.Message);
                    throw;
                }
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
}
