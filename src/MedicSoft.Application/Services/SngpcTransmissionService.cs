using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing SNGPC data transmission to ANVISA.
    /// Handles report submission, retry logic, and transmission tracking.
    /// </summary>
    public class SngpcTransmissionService : ISngpcTransmissionService
    {
        private const int MaxTransmissionAttempts = 5;
        private const string TransmissionMethod = "WebService";

        private readonly ISngpcTransmissionRepository _transmissionRepository;
        private readonly ISNGPCReportRepository _reportRepository;
        private readonly IAnvisaSngpcClient _anvisaClient;
        private readonly ISNGPCXmlGeneratorService _xmlGenerator;
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly ILogger<SngpcTransmissionService> _logger;

        public SngpcTransmissionService(
            ISngpcTransmissionRepository transmissionRepository,
            ISNGPCReportRepository reportRepository,
            IAnvisaSngpcClient anvisaClient,
            ISNGPCXmlGeneratorService xmlGenerator,
            IDigitalPrescriptionRepository prescriptionRepository,
            ILogger<SngpcTransmissionService> logger)
        {
            _transmissionRepository = transmissionRepository ?? throw new ArgumentNullException(nameof(transmissionRepository));
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _anvisaClient = anvisaClient ?? throw new ArgumentNullException(nameof(anvisaClient));
            _xmlGenerator = xmlGenerator ?? throw new ArgumentNullException(nameof(xmlGenerator));
            _prescriptionRepository = prescriptionRepository ?? throw new ArgumentNullException(nameof(prescriptionRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<SngpcTransmission> TransmitReportAsync(
            Guid reportId, 
            string tenantId, 
            Guid? userId)
        {
            if (reportId == Guid.Empty)
                throw new ArgumentException("Report ID cannot be empty", nameof(reportId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Initiating transmission for SNGPC report {ReportId} for tenant {TenantId}",
                reportId, tenantId);

            // Get the report
            var report = await _reportRepository.GetByIdAsync(reportId, tenantId);
            if (report == null)
            {
                _logger.LogError("SNGPC report {ReportId} not found", reportId);
                throw new InvalidOperationException($"SNGPC report {reportId} not found");
            }

            // Check if report is ready for transmission
            if (report.Status != SNGPCReportStatus.Generated && report.Status != SNGPCReportStatus.TransmissionFailed)
            {
                _logger.LogWarning(
                    "Report {ReportId} is not ready for transmission (Status: {Status})",
                    reportId, report.Status);
                throw new InvalidOperationException($"Report is not ready for transmission. Current status: {report.Status}");
            }

            // Get attempt count
            var attemptCount = await _transmissionRepository.GetAttemptCountAsync(reportId, tenantId);
            if (attemptCount >= MaxTransmissionAttempts)
            {
                _logger.LogError(
                    "Report {ReportId} has exceeded maximum transmission attempts ({MaxAttempts})",
                    reportId, MaxTransmissionAttempts);
                throw new InvalidOperationException($"Maximum transmission attempts ({MaxTransmissionAttempts}) exceeded");
            }

            // Generate XML content using the XML generator service
            var prescriptions = await GetPrescriptionsForReportAsync(report.TenantId, report.Year, report.Month);
            var xmlContent = await _xmlGenerator.GenerateXmlAsync(report, prescriptions);
            var xmlHash = ComputeSha256Hash(xmlContent);
            var xmlSize = Encoding.UTF8.GetByteCount(xmlContent);

            // Create transmission record
            var transmission = new SngpcTransmission(
                tenantId: tenantId,
                sngpcReportId: reportId,
                attemptNumber: attemptCount + 1,
                transmissionMethod: TransmissionMethod,
                endpointUrl: _anvisaClient.GetType().Name, // Use client type as endpoint identifier
                xmlHash: xmlHash,
                xmlSizeBytes: xmlSize,
                initiatedByUserId: userId
            );

            await _transmissionRepository.AddAsync(transmission);

            // Mark as in progress
            transmission.MarkAsInProgress();
            await _transmissionRepository.UpdateAsync(transmission);

            // Transmit to ANVISA using the real client
            var stopwatch = Stopwatch.StartNew();
            
            try
            {
                var response = await _anvisaClient.SendSngpcXmlAsync(xmlContent);
                stopwatch.Stop();

                if (response.Success)
                {
                    // Mark as successful
                    transmission.MarkAsSuccessful(
                        protocolNumber: response.ProtocolNumber ?? "N/A",
                        anvisaResponse: response.Message ?? "Transmission successful",
                        httpStatusCode: response.HttpStatusCode ?? 200,
                        responseTimeMs: stopwatch.ElapsedMilliseconds
                    );

                    _logger.LogInformation(
                        "Successfully transmitted report {ReportId}. Protocol: {Protocol}",
                        reportId, response.ProtocolNumber);
                }
                else
                {
                    // Mark as failed
                    transmission.MarkAsFailed(
                        errorMessage: response.ErrorMessage ?? "Unknown error",
                        errorCode: response.ErrorCode ?? "UNKNOWN",
                        httpStatusCode: response.HttpStatusCode ?? 500,
                        responseTimeMs: stopwatch.ElapsedMilliseconds
                    );

                    _logger.LogError(
                        "Failed to transmit report {ReportId}: {ErrorMessage}",
                        reportId, response.ErrorMessage);
                }

                await _transmissionRepository.UpdateAsync(transmission);
                return transmission;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                _logger.LogError(
                    ex,
                    "Failed to transmit report {ReportId}",
                    reportId);

                transmission.MarkAsFailed(
                    errorMessage: ex.Message,
                    errorCode: "TRANSMISSION_ERROR",
                    httpStatusCode: 500,
                    responseTimeMs: stopwatch.ElapsedMilliseconds
                );

                await _transmissionRepository.UpdateAsync(transmission);

                return transmission;
            }
        }

        public async Task<SngpcTransmission> RetryTransmissionAsync(
            Guid transmissionId, 
            string tenantId)
        {
            if (transmissionId == Guid.Empty)
                throw new ArgumentException("Transmission ID cannot be empty", nameof(transmissionId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Retrying transmission {TransmissionId} for tenant {TenantId}",
                transmissionId, tenantId);

            // Get the failed transmission
            var failedTransmission = await _transmissionRepository.GetByIdAsync(transmissionId, tenantId);
            if (failedTransmission == null)
            {
                _logger.LogError("Transmission {TransmissionId} not found", transmissionId);
                throw new InvalidOperationException($"Transmission {transmissionId} not found");
            }

            if (!failedTransmission.CanRetry())
            {
                _logger.LogWarning(
                    "Transmission {TransmissionId} cannot be retried (Status: {Status})",
                    transmissionId, failedTransmission.Status);
                throw new InvalidOperationException($"Transmission cannot be retried. Status: {failedTransmission.Status}");
            }

            // Check if max attempts would be exceeded
            var attemptCount = await _transmissionRepository.GetAttemptCountAsync(
                failedTransmission.SNGPCReportId, tenantId);
            
            if (attemptCount >= MaxTransmissionAttempts)
            {
                _logger.LogError(
                    "Report {ReportId} has exceeded maximum transmission attempts ({MaxAttempts})",
                    failedTransmission.SNGPCReportId, MaxTransmissionAttempts);
                throw new InvalidOperationException($"Maximum transmission attempts ({MaxTransmissionAttempts}) exceeded");
            }

            // Create a new transmission attempt
            return await TransmitReportAsync(
                failedTransmission.SNGPCReportId, 
                tenantId, 
                failedTransmission.InitiatedByUserId);
        }

        public async Task<IEnumerable<SngpcTransmission>> GetTransmissionHistoryAsync(
            Guid reportId, 
            string tenantId)
        {
            if (reportId == Guid.Empty)
                throw new ArgumentException("Report ID cannot be empty", nameof(reportId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Getting transmission history for report {ReportId}",
                reportId);

            return await _transmissionRepository.GetByReportIdAsync(reportId, tenantId);
        }

        public async Task<TransmissionStatistics> GetStatisticsAsync(
            DateTime startDate, 
            DateTime endDate, 
            string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (startDate > endDate)
                throw new ArgumentException("Start date must be before or equal to end date");

            _logger.LogInformation(
                "Getting transmission statistics from {StartDate} to {EndDate} for tenant {TenantId}",
                startDate, endDate, tenantId);

            return await _transmissionRepository.GetTransmissionStatisticsAsync(startDate, endDate, tenantId);
        }

        #region Private Helper Methods

        /// <summary>
        /// Gets controlled medication prescriptions for the specified period
        /// </summary>
        private async Task<IEnumerable<DigitalPrescription>> GetPrescriptionsForReportAsync(
            string tenantId, 
            int year, 
            int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var prescriptions = await _prescriptionRepository.GetControlledPrescriptionsByPeriodAsync(
                tenantId, startDate, endDate);

            return prescriptions;
        }

        /// <summary>
        /// Computes SHA-256 hash of the XML content.
        /// </summary>
        private string ComputeSha256Hash(string content)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(content);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        #endregion
    }
}
