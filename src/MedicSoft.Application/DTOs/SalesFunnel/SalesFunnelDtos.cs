namespace MedicSoft.Application.DTOs.SalesFunnel
{
    /// <summary>
    /// DTO for tracking sales funnel events (unauthenticated)
    /// </summary>
    public class TrackSalesFunnelEventDto
    {
        /// <summary>
        /// Unique session identifier (generated client-side)
        /// </summary>
        public string SessionId { get; set; } = string.Empty;

        /// <summary>
        /// Registration step (1-6)
        /// </summary>
        public int Step { get; set; }

        /// <summary>
        /// Action performed: 'entered', 'completed', 'abandoned'
        /// </summary>
        public string Action { get; set; } = string.Empty;

        /// <summary>
        /// JSON representation of captured data (sanitized)
        /// </summary>
        public string? CapturedData { get; set; }

        /// <summary>
        /// Selected plan ID if applicable
        /// </summary>
        public string? PlanId { get; set; }

        /// <summary>
        /// Referrer URL
        /// </summary>
        public string? Referrer { get; set; }

        /// <summary>
        /// Duration in milliseconds spent on this step
        /// </summary>
        public long? DurationMs { get; set; }

        /// <summary>
        /// Additional metadata (UTM parameters, etc.)
        /// </summary>
        public string? Metadata { get; set; }
    }

    /// <summary>
    /// DTO for marking a session as converted
    /// </summary>
    public class MarkConversionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public Guid OwnerId { get; set; }
    }

    /// <summary>
    /// Response for tracking events
    /// </summary>
    public class TrackEventResponseDto
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
    }

    /// <summary>
    /// Sales funnel metric DTO for display
    /// </summary>
    public class SalesFunnelMetricDto
    {
        public Guid Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public int Step { get; set; }
        public string StepName { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string? CapturedData { get; set; }
        public string? PlanId { get; set; }
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public string? Referrer { get; set; }
        public Guid? ClinicId { get; set; }
        public Guid? OwnerId { get; set; }
        public bool IsConverted { get; set; }
        public long? DurationMs { get; set; }
        public string? Metadata { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Funnel statistics DTO
    /// </summary>
    public class FunnelStatsDto
    {
        public int TotalSessions { get; set; }
        public int Conversions { get; set; }
        public double ConversionRate { get; set; }
        public Dictionary<int, StepStatsDto> StepStats { get; set; } = new();
    }

    /// <summary>
    /// Statistics for a specific step
    /// </summary>
    public class StepStatsDto
    {
        public int StepNumber { get; set; }
        public string StepName { get; set; } = string.Empty;
        public int Entered { get; set; }
        public int Completed { get; set; }
        public int Abandoned { get; set; }
        public double CompletionRate { get; set; }
        public double AbandonmentRate { get; set; }
    }

    /// <summary>
    /// Summary of an incomplete session
    /// </summary>
    public class IncompleteSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public int LastStep { get; set; }
        public string LastStepName { get; set; } = string.Empty;
        public string? CapturedData { get; set; }
        public string? PlanId { get; set; }
        public DateTime LastActivity { get; set; }
        public int HoursSinceLastActivity { get; set; }
    }
}
