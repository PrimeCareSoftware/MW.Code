using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SystemAdmin
{
    /// <summary>
    /// Real User Monitoring metric data
    /// </summary>
    public class RumMetricDto
    {
        public string Metric { get; set; } = string.Empty;
        public double Value { get; set; }
        public string Url { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public string? ConnectionType { get; set; }
        public Dictionary<string, string>? AdditionalData { get; set; }
    }

    /// <summary>
    /// Error tracking data
    /// </summary>
    public class ErrorTrackingDto
    {
        public string Message { get; set; } = string.Empty;
        public string? Stack { get; set; }
        public string Severity { get; set; } = "medium";
        public string Url { get; set; } = string.Empty;
        public string? UserAgent { get; set; }
        public Dictionary<string, object>? Context { get; set; }
    }

    /// <summary>
    /// Web Vitals summary
    /// </summary>
    public class WebVitalsSummaryDto
    {
        public double AvgFcp { get; set; }
        public double AvgLcp { get; set; }
        public double AvgFid { get; set; }
        public double AvgCls { get; set; }
        public double AvgTtfb { get; set; }
        public int SampleCount { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Page performance summary
    /// </summary>
    public class PagePerformanceDto
    {
        public string Url { get; set; } = string.Empty;
        public double AvgLoadTime { get; set; }
        public int ViewCount { get; set; }
        public double P50LoadTime { get; set; }
        public double P95LoadTime { get; set; }
        public double P99LoadTime { get; set; }
    }
}
