using System;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services.Reports
{
    /// <summary>
    /// Background job service for executing scheduled reports
    /// Phase 3: Analytics and BI
    /// </summary>
    public class ScheduledReportJob
    {
        private readonly IReportService _reportService;
        private readonly ILogger<ScheduledReportJob> _logger;

        public ScheduledReportJob(IReportService reportService, ILogger<ScheduledReportJob> logger)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Execute a scheduled report by ID
        /// Called by Hangfire background job
        /// </summary>
        [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 60, 300, 600 })]
        public async Task ExecuteAsync(int scheduledReportId)
        {
            _logger.LogInformation("Starting scheduled report job execution for report ID: {ReportId}", scheduledReportId);

            try
            {
                await _reportService.ExecuteScheduledReportAsync(scheduledReportId);
                _logger.LogInformation("Scheduled report job completed successfully for report ID: {ReportId}", scheduledReportId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Scheduled report job failed for report ID: {ReportId}", scheduledReportId);
                throw; // Re-throw to allow Hangfire retry mechanism
            }
        }

        /// <summary>
        /// Schedule a recurring report job
        /// </summary>
        public static void ScheduleRecurring(int scheduledReportId, string cronExpression, string? jobName = null)
        {
            var jobId = jobName ?? $"scheduled-report-{scheduledReportId}";
            
            RecurringJob.AddOrUpdate<ScheduledReportJob>(
                jobId,
                job => job.ExecuteAsync(scheduledReportId),
                cronExpression,
                TimeZoneInfo.Utc
            );
        }

        /// <summary>
        /// Remove a scheduled report job
        /// </summary>
        public static void RemoveSchedule(int scheduledReportId, string? jobName = null)
        {
            var jobId = jobName ?? $"scheduled-report-{scheduledReportId}";
            RecurringJob.RemoveIfExists(jobId);
        }
    }
}
