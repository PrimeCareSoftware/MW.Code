using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Middleware
{
    /// <summary>
    /// Middleware para monitoramento de performance e detecção de operações longas
    /// Coleta métricas detalhadas de tempo de execução e alerta sobre possíveis timeouts
    /// </summary>
    public class PerformanceMonitoringMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<PerformanceMonitoringMiddleware> _logger;
        private readonly int _warningThresholdMs;
        private readonly int _criticalThresholdMs;
        private readonly int _timeoutThresholdMs;

        public PerformanceMonitoringMiddleware(
            RequestDelegate next,
            ILogger<PerformanceMonitoringMiddleware> logger,
            IConfiguration configuration)
        {
            _next = next;
            _logger = logger;
            _warningThresholdMs = configuration.GetValue<int>("Monitoring:PerformanceWarningThresholdMs", 2000);
            _criticalThresholdMs = configuration.GetValue<int>("Monitoring:PerformanceCriticalThresholdMs", 5000);
            _timeoutThresholdMs = configuration.GetValue<int>("Monitoring:TimeoutThresholdMs", 30000);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var path = context.Request.Path.Value ?? "/";
            var method = context.Request.Method;

            try
            {
                await _next(context);
                stopwatch.Stop();

                var elapsed = stopwatch.ElapsedMilliseconds;
                AnalyzePerformance(method, path, elapsed);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                var elapsed = stopwatch.ElapsedMilliseconds;

                _logger.LogError(
                    ex,
                    "PERFORMANCE CRITICAL - Request failed: {Method} {Path} after {ElapsedMs}ms - Exception: {ExceptionType}",
                    method,
                    path,
                    elapsed,
                    ex.GetType().Name);

                throw;
            }
        }

        private void AnalyzePerformance(string method, string path, long elapsedMs)
        {
            // Métricas estruturadas para análise posterior
            var metrics = new Dictionary<string, object>
            {
                ["Endpoint"] = $"{method} {path}",
                ["DurationMs"] = elapsedMs,
                ["Timestamp"] = DateTime.UtcNow,
                ["PerformanceCategory"] = GetPerformanceCategory(elapsedMs)
            };

            using (_logger.BeginScope(metrics))
            {
                if (elapsedMs >= _timeoutThresholdMs)
                {
                    _logger.LogCritical(
                        "TIMEOUT ALERT: {Method} {Path} took {ElapsedMs}ms (timeout threshold: {TimeoutThresholdMs}ms) - Possible timeout scenario",
                        method,
                        path,
                        elapsedMs,
                        _timeoutThresholdMs);
                }
                else if (elapsedMs >= _criticalThresholdMs)
                {
                    _logger.LogError(
                        "PERFORMANCE CRITICAL: {Method} {Path} took {ElapsedMs}ms (critical threshold: {CriticalThresholdMs}ms)",
                        method,
                        path,
                        elapsedMs,
                        _criticalThresholdMs);
                }
                else if (elapsedMs >= _warningThresholdMs)
                {
                    _logger.LogWarning(
                        "PERFORMANCE WARNING: {Method} {Path} took {ElapsedMs}ms (warning threshold: {WarningThresholdMs}ms)",
                        method,
                        path,
                        elapsedMs,
                        _warningThresholdMs);
                }
                else
                {
                    _logger.LogDebug(
                        "Performance metrics: {Method} {Path} - Duration: {ElapsedMs}ms",
                        method,
                        path,
                        elapsedMs);
                }
            }
        }

        private string GetPerformanceCategory(long elapsedMs)
        {
            if (elapsedMs >= _timeoutThresholdMs)
                return "TIMEOUT";
            if (elapsedMs >= _criticalThresholdMs)
                return "CRITICAL";
            if (elapsedMs >= _warningThresholdMs)
                return "WARNING";
            return "NORMAL";
        }
    }

    public static class PerformanceMonitoringMiddlewareExtensions
    {
        public static IApplicationBuilder UsePerformanceMonitoring(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PerformanceMonitoringMiddleware>();
        }
    }
}
