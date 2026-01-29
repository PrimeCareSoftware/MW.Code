using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Implementation of SNGPC monitoring and alerting service.
    /// Monitors deadlines, validates compliance, and detects anomalies in controlled medication management.
    /// </summary>
    public class SngpcAlertService : ISngpcAlertService
    {
        private readonly ISNGPCReportRepository _reportRepository;
        private readonly IControlledMedicationRegistryRepository _registryRepository;
        private readonly ISngpcAlertRepository _alertRepository;
        private readonly ILogger<SngpcAlertService> _logger;
        
        // ANVISA deadline: reports must be submitted by the 15th of the following month
        private const int AnvisaDeadlineDay = 15;

        public SngpcAlertService(
            ISNGPCReportRepository reportRepository,
            IControlledMedicationRegistryRepository registryRepository,
            ISngpcAlertRepository alertRepository,
            ILogger<SngpcAlertService> logger)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _registryRepository = registryRepository ?? throw new ArgumentNullException(nameof(registryRepository));
            _alertRepository = alertRepository ?? throw new ArgumentNullException(nameof(alertRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SngpcAlertDto>> CheckApproachingDeadlinesAsync(
            string tenantId, 
            int daysBeforeDeadline = 5)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Checking for approaching SNGPC deadlines for tenant {TenantId} (alert {Days} days before)",
                tenantId, daysBeforeDeadline);

            var alerts = new List<SngpcAlertDto>();
            var now = DateTime.UtcNow;

            // Check current month and previous months
            for (int monthsBack = 0; monthsBack <= 2; monthsBack++)
            {
                var checkDate = now.AddMonths(-monthsBack);
                var year = checkDate.Year;
                var month = checkDate.Month;

                // Calculate deadline for this month (15th of next month)
                var deadline = new DateTime(year, month, 1)
                    .AddMonths(1)
                    .AddDays(AnvisaDeadlineDay - 1);

                var daysUntilDeadline = (deadline - now).Days;

                // Check if within alert window
                if (daysUntilDeadline > 0 && daysUntilDeadline <= daysBeforeDeadline)
                {
                    // Check if report exists and is not transmitted
                    var report = await _reportRepository.GetByMonthYearAsync(month, year, tenantId);

                    if (report == null || report.Status != SNGPCReportStatus.Transmitted)
                    {
                        var alert = await CreateAndPersistAlertAsync(
                            tenantId,
                            AlertType.DeadlineApproaching,
                            daysUntilDeadline <= 2 ? AlertSeverity.Error : AlertSeverity.Warning,
                            $"Prazo SNGPC se aproximando - {month:D2}/{year}",
                            $"O relatório SNGPC de {month:D2}/{year} deve ser enviado à ANVISA até {deadline:dd/MM/yyyy}. Faltam {daysUntilDeadline} dias.",
                            reportId: report?.Id
                        );
                        
                        alerts.Add(alert);
                    }
                }
            }

            _logger.LogInformation(
                "Found {Count} approaching deadlines for tenant {TenantId}",
                alerts.Count, tenantId);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlertDto>> CheckOverdueReportsAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Checking for overdue SNGPC reports for tenant {TenantId}", tenantId);

            var alerts = new List<SngpcAlertDto>();
            var now = DateTime.UtcNow;

            // Check previous 12 months for overdue reports
            for (int monthsBack = 1; monthsBack <= 12; monthsBack++)
            {
                var checkDate = now.AddMonths(-monthsBack);
                var year = checkDate.Year;
                var month = checkDate.Month;

                // Calculate deadline (15th of next month)
                var deadline = new DateTime(year, month, 1)
                    .AddMonths(1)
                    .AddDays(AnvisaDeadlineDay - 1);

                // Skip if not yet overdue
                if (deadline >= now)
                    continue;

                // Check if report exists and is transmitted
                var report = await _reportRepository.GetByMonthYearAsync(month, year, tenantId);

                if (report == null)
                {
                    var daysOverdue = (now - deadline).Days;
                    var alert = await CreateAndPersistAlertAsync(
                        tenantId,
                        AlertType.MissingReport,
                        AlertSeverity.Critical,
                        $"Relatório SNGPC não gerado - {month:D2}/{year}",
                        $"O relatório SNGPC de {month:D2}/{year} não foi gerado. Prazo vencido há {daysOverdue} dias. ANVISA pode aplicar multa."
                    );
                    alerts.Add(alert);
                }
                else if (report.Status != SNGPCReportStatus.Transmitted)
                {
                    var daysOverdue = (now - deadline).Days;
                    var alert = await CreateAndPersistAlertAsync(
                        tenantId,
                        AlertType.DeadlineOverdue,
                        AlertSeverity.Critical,
                        $"Relatório SNGPC vencido - {month:D2}/{year}",
                        $"O relatório SNGPC de {month:D2}/{year} está vencido há {daysOverdue} dias. Transmissão urgente necessária para evitar multa ANVISA.",
                        reportId: report.Id
                    );
                    alerts.Add(alert);
                }
            }

            _logger.LogInformation(
                "Found {Count} overdue reports for tenant {TenantId}",
                alerts.Count, tenantId);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlertDto>> ValidateComplianceAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Validating SNGPC compliance for tenant {TenantId}", tenantId);

            var alerts = new List<SngpcAlertDto>();
            var now = DateTime.UtcNow;

            // Get all registry entries for the current month
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
            
            var entries = await _registryRepository.GetByPeriodAsync(startOfMonth, endOfMonth, tenantId);
            var entriesList = entries.ToList();

            // Check for negative balances
            var negativeBalances = entriesList
                .Where(e => e.Balance < 0)
                .GroupBy(e => e.MedicationName)
                .ToList();

            foreach (var group in negativeBalances)
            {
                var latestEntry = group.OrderByDescending(e => e.Date).FirstOrDefault();
                if (latestEntry == null) continue;
                var alert = await CreateAndPersistAlertAsync(
                    tenantId,
                    AlertType.NegativeBalance,
                    AlertSeverity.Critical,
                    $"Saldo negativo detectado: {group.Key}",
                    $"O medicamento controlado '{group.Key}' apresenta saldo negativo ({latestEntry.Balance}). Isso viola as normas da ANVISA.",
                    registryId: latestEntry.Id,
                    medicationName: group.Key
                );
                alerts.Add(alert);
            }

            // Check for missing consecutive entries
            // Group by medication and check for gaps in dates
            var medicationGroups = entriesList
                .GroupBy(e => e.MedicationName)
                .ToList();

            foreach (var group in medicationGroups)
            {
                var orderedEntries = group.OrderBy(e => e.Date).ToList();
                
                // Check for balance inconsistencies
                for (int i = 1; i < orderedEntries.Count; i++)
                {
                    var previous = orderedEntries[i - 1];
                    var current = orderedEntries[i];
                    
                    // Calculate expected balance
                    var expectedBalance = previous.Balance + current.QuantityIn - current.QuantityOut;
                    
                    if (Math.Abs(expectedBalance - current.Balance) > 0.01m)
                    {
                        var alert = await CreateAndPersistAlertAsync(
                            tenantId,
                            AlertType.InvalidBalance,
                            AlertSeverity.Error,
                            $"Inconsistência no saldo: {group.Key}",
                            $"O saldo registrado ({current.Balance}) não corresponde ao saldo calculado ({expectedBalance}) para '{group.Key}' em {current.Date:dd/MM/yyyy}.",
                            registryId: current.Id,
                            medicationName: group.Key
                        );
                        alerts.Add(alert);
                    }
                }
            }

            _logger.LogInformation(
                "Compliance validation completed for tenant {TenantId}. Found {Count} issues.",
                tenantId, alerts.Count);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlertDto>> DetectAnomaliesAsync(
            string tenantId, 
            DateTime startDate, 
            DateTime endDate)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date");

            _logger.LogInformation(
                "Detecting anomalies for tenant {TenantId} from {StartDate} to {EndDate}",
                tenantId, startDate, endDate);

            var alerts = new List<SngpcAlertDto>();
            var now = DateTime.UtcNow;

            var entries = await _registryRepository.GetByPeriodAsync(startDate, endDate, tenantId);
            var entriesList = entries.ToList();

            if (!entriesList.Any())
            {
                _logger.LogInformation("No entries found for anomaly detection");
                return alerts;
            }

            // Group by medication for analysis
            var medicationGroups = entriesList
                .GroupBy(e => e.MedicationName)
                .ToList();

            foreach (var group in medicationGroups)
            {
                var outboundEntries = group.Where(e => e.RegistryType == RegistryType.Outbound).ToList();
                
                if (!outboundEntries.Any())
                    continue;

                // Calculate average daily dispensing
                var totalDays = (endDate - startDate).Days + 1;
                var totalDispensed = outboundEntries.Sum(e => e.QuantityOut);
                var averagePerDay = totalDispensed / totalDays;

                // Check for excessive single-day dispensing (more than 5x average)
                var excessiveDispensing = outboundEntries
                    .Where(e => e.QuantityOut > averagePerDay * 5 && averagePerDay > 0)
                    .ToList();

                foreach (var entry in excessiveDispensing)
                {
                    var alert = await CreateAndPersistAlertAsync(
                        tenantId,
                        AlertType.ExcessiveDispensing,
                        AlertSeverity.Warning,
                        $"Dispensação excessiva detectada: {group.Key}",
                        $"Foram dispensadas {entry.QuantityOut} unidades de '{group.Key}' em {entry.Date:dd/MM/yyyy}, significativamente acima da média diária ({averagePerDay:F2}).",
                        registryId: entry.Id,
                        medicationName: group.Key
                    );
                    alerts.Add(alert);
                }

                // Check for unusual movement patterns (large stock entries without corresponding dispensing)
                var inboundEntries = group.Where(e => e.RegistryType == RegistryType.Inbound).ToList();
                var largeInbound = inboundEntries.Where(e => e.QuantityIn > totalDispensed * 2).ToList();

                foreach (var entry in largeInbound)
                {
                    var alert = await CreateAndPersistAlertAsync(
                        tenantId,
                        AlertType.UnusualMovement,
                        AlertSeverity.Info,
                        $"Entrada de estoque incomum: {group.Key}",
                        $"Entrada de {entry.QuantityIn} unidades de '{group.Key}' em {entry.Date:dd/MM/yyyy} é significativamente maior que o padrão de dispensação.",
                        registryId: entry.Id,
                        medicationName: group.Key
                    );
                    alerts.Add(alert);
                }
            }

            _logger.LogInformation(
                "Anomaly detection completed for tenant {TenantId}. Found {Count} anomalies.",
                tenantId, alerts.Count);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlertDto>> GetActiveAlertsAsync(
            string tenantId, 
            AlertSeverity? severity = null)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));
            
            _logger.LogInformation(
                "Getting active alerts for tenant {TenantId} with severity filter: {Severity}",
                tenantId, severity);

            var alerts = await _alertRepository.GetActiveAlertsAsync(tenantId, severity);
            return alerts.Select(ToDto).ToList();
        }

        public async Task AcknowledgeAlertAsync(Guid alertId, Guid userId, string? notes = null)
        {
            _logger.LogInformation(
                "Acknowledging alert {AlertId} by user {UserId}",
                alertId, userId);

            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null)
                throw new InvalidOperationException($"Alert {alertId} not found");

            alert.Acknowledge(userId, notes);
            await _alertRepository.UpdateAsync(alert);
        }

        public async Task ResolveAlertAsync(Guid alertId, Guid userId, string resolution)
        {
            if (string.IsNullOrWhiteSpace(resolution))
                throw new ArgumentException("Resolution is required", nameof(resolution));
            
            _logger.LogInformation(
                "Resolving alert {AlertId} by user {UserId}: {Resolution}",
                alertId, userId, resolution);

            var alert = await _alertRepository.GetByIdAsync(alertId);
            if (alert == null)
                throw new InvalidOperationException($"Alert {alertId} not found");

            alert.Resolve(userId, resolution);
            await _alertRepository.UpdateAsync(alert);
        }

        /// <summary>
        /// Helper method to convert domain entity to DTO
        /// </summary>
        private SngpcAlertDto ToDto(SngpcAlert alert)
        {
            return new SngpcAlertDto
            {
                Id = alert.Id,
                TenantId = alert.TenantId,
                Type = alert.Type,
                Severity = alert.Severity,
                Title = alert.Title,
                Description = alert.Description,
                CreatedAt = alert.CreatedAt,
                AcknowledgedAt = alert.AcknowledgedAt,
                AcknowledgedByUserId = alert.AcknowledgedByUserId,
                AcknowledgmentNotes = alert.AcknowledgmentNotes,
                ResolvedAt = alert.ResolvedAt,
                ResolvedByUserId = alert.ResolvedByUserId,
                Resolution = alert.Resolution,
                RelatedReportId = alert.RelatedReportId,
                RelatedRegistryId = alert.RelatedRegistryId,
                RelatedMedication = alert.RelatedMedication,
                AdditionalData = alert.AdditionalData
            };
        }

        /// <summary>
        /// Helper method to create and persist an alert
        /// </summary>
        private async Task<SngpcAlertDto> CreateAndPersistAlertAsync(
            string tenantId,
            AlertType type,
            AlertSeverity severity,
            string title,
            string description,
            Guid? reportId = null,
            Guid? registryId = null,
            string? medicationName = null)
        {
            SngpcAlert alert;

            if (reportId.HasValue)
            {
                alert = SngpcAlert.CreateReportAlert(tenantId, type, severity, title, description, reportId.Value);
            }
            else if (registryId.HasValue)
            {
                alert = SngpcAlert.CreateRegistryAlert(tenantId, type, severity, title, description, registryId.Value, medicationName);
            }
            else
            {
                alert = new SngpcAlert(tenantId, type, severity, title, description);
            }

            await _alertRepository.AddAsync(alert);
            
            _logger.LogDebug("Created and persisted alert: {Type} - {Title}", type, title);
            
            return ToDto(alert);
        }
    }
}
