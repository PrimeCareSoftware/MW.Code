using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.SystemAdmin;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface IMonitoringService
    {
        Task TrackRumMetricAsync(RumMetricDto metric);
        Task TrackErrorAsync(ErrorTrackingDto error);
        Task<WebVitalsSummaryDto> GetWebVitalsSummaryAsync(int days = 7);
        Task<List<PagePerformanceDto>> GetTopSlowPagesAsync(int limit = 10);
    }

    /// <summary>
    /// Monitoring service for collecting RUM metrics and errors.
    /// 
    /// IMPORTANT: This implementation uses in-memory storage for simplicity.
    /// Limitations:
    /// - Data is lost on application restart
    /// - Each instance in multi-instance deployments has separate metrics
    /// - Limited to last 10,000 RUM metrics and 5,000 errors
    /// 
    /// For production, consider:
    /// - Persisting to database (e.g., TimescaleDB, InfluxDB)
    /// - Using external APM services (Application Insights, Datadog, New Relic)
    /// - Implementing distributed tracing (OpenTelemetry)
    /// </summary>
    public class MonitoringService : IMonitoringService
    {
        private readonly ILogger<MonitoringService> _logger;
        private static readonly List<RumMetricDto> _rumMetrics = new();
        private static readonly List<ErrorTrackingDto> _errors = new();
        private static readonly object _lock = new();

        public MonitoringService(ILogger<MonitoringService> logger)
        {
            _logger = logger;
        }

        public Task TrackRumMetricAsync(RumMetricDto metric)
        {
            try
            {
                lock (_lock)
                {
                    _rumMetrics.Add(metric);
                    
                    // Keep only last 10000 metrics to avoid memory issues
                    if (_rumMetrics.Count > 10000)
                    {
                        _rumMetrics.RemoveRange(0, _rumMetrics.Count - 10000);
                    }
                }

                // Log key metrics
                if (metric.Metric == "fcp" || metric.Metric == "lcp")
                {
                    _logger.LogInformation(
                        "RUM Metric: {Metric} = {Value}ms on {Url}",
                        metric.Metric.ToUpper(),
                        metric.Value,
                        metric.Url
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking RUM metric");
            }

            return Task.CompletedTask;
        }

        public Task TrackErrorAsync(ErrorTrackingDto error)
        {
            try
            {
                lock (_lock)
                {
                    _errors.Add(error);
                    
                    // Keep only last 5000 errors
                    if (_errors.Count > 5000)
                    {
                        _errors.RemoveRange(0, _errors.Count - 5000);
                    }
                }

                // Log errors based on severity
                var logLevel = error.Severity?.ToLower() switch
                {
                    "critical" => LogLevel.Critical,
                    "high" => LogLevel.Error,
                    "medium" => LogLevel.Warning,
                    _ => LogLevel.Information
                };

                _logger.Log(
                    logLevel,
                    "Frontend Error [{Severity}]: {Message} on {Url}",
                    error.Severity,
                    error.Message,
                    error.Url
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking frontend error");
            }

            return Task.CompletedTask;
        }

        public Task<WebVitalsSummaryDto> GetWebVitalsSummaryAsync(int days = 7)
        {
            try
            {
                var cutoffDate = DateTime.UtcNow.AddDays(-days);
                
                List<RumMetricDto> recentMetrics;
                lock (_lock)
                {
                    recentMetrics = _rumMetrics.ToList();
                }

                var fcpMetrics = recentMetrics.Where(m => m.Metric == "fcp").ToList();
                var lcpMetrics = recentMetrics.Where(m => m.Metric == "lcp").ToList();
                var fidMetrics = recentMetrics.Where(m => m.Metric == "fid").ToList();
                var clsMetrics = recentMetrics.Where(m => m.Metric == "cls").ToList();
                var ttfbMetrics = recentMetrics.Where(m => m.Metric == "ttfb").ToList();

                var summary = new WebVitalsSummaryDto
                {
                    AvgFcp = fcpMetrics.Any() ? fcpMetrics.Average(m => m.Value) : 0,
                    AvgLcp = lcpMetrics.Any() ? lcpMetrics.Average(m => m.Value) : 0,
                    AvgFid = fidMetrics.Any() ? fidMetrics.Average(m => m.Value) : 0,
                    AvgCls = clsMetrics.Any() ? clsMetrics.Average(m => m.Value) : 0,
                    AvgTtfb = ttfbMetrics.Any() ? ttfbMetrics.Average(m => m.Value) : 0,
                    SampleCount = recentMetrics.Count,
                    LastUpdated = DateTime.UtcNow
                };

                return Task.FromResult(summary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting web vitals summary");
                return Task.FromResult(new WebVitalsSummaryDto
                {
                    LastUpdated = DateTime.UtcNow
                });
            }
        }

        public Task<List<PagePerformanceDto>> GetTopSlowPagesAsync(int limit = 10)
        {
            try
            {
                List<RumMetricDto> allMetrics;
                lock (_lock)
                {
                    allMetrics = _rumMetrics.ToList();
                }

                var pageLoadMetrics = allMetrics
                    .Where(m => m.Metric == "page_load")
                    .GroupBy(m => m.Url)
                    .Select(g => new PagePerformanceDto
                    {
                        Url = g.Key,
                        AvgLoadTime = g.Average(m => m.Value),
                        ViewCount = g.Count(),
                        P50LoadTime = GetPercentile(g.Select(m => m.Value).OrderBy(v => v).ToList(), 50),
                        P95LoadTime = GetPercentile(g.Select(m => m.Value).OrderBy(v => v).ToList(), 95),
                        P99LoadTime = GetPercentile(g.Select(m => m.Value).OrderBy(v => v).ToList(), 99)
                    })
                    .OrderByDescending(p => p.AvgLoadTime)
                    .Take(limit)
                    .ToList();

                return Task.FromResult(pageLoadMetrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting top slow pages");
                return Task.FromResult(new List<PagePerformanceDto>());
            }
        }

        private static double GetPercentile(List<double> sortedValues, int percentile)
        {
            if (sortedValues.Count == 0) return 0;
            
            int index = (int)Math.Ceiling(percentile / 100.0 * sortedValues.Count) - 1;
            index = Math.Max(0, Math.Min(index, sortedValues.Count - 1));
            
            return sortedValues[index];
        }
    }
}
