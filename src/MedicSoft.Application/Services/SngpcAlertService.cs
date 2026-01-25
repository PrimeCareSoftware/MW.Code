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
        private readonly ILogger<SngpcAlertService> _logger;
        
        // ANVISA deadline: reports must be submitted by the 15th of the following month
        private const int AnvisaDeadlineDay = 15;

        public SngpcAlertService(
            ISNGPCReportRepository reportRepository,
            IControlledMedicationRegistryRepository registryRepository,
            ILogger<SngpcAlertService> logger)
        {
            _reportRepository = reportRepository ?? throw new ArgumentNullException(nameof(reportRepository));
            _registryRepository = registryRepository ?? throw new ArgumentNullException(nameof(registryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<SngpcAlert>> CheckApproachingDeadlinesAsync(
            string tenantId, 
            int daysBeforeDeadline = 5)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Checking for approaching SNGPC deadlines for tenant {TenantId} (alert {Days} days before)",
                tenantId, daysBeforeDeadline);

            var alerts = new List<SngpcAlert>();
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
                        alerts.Add(new SngpcAlert
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenantId,
                            Type = AlertType.DeadlineApproaching,
                            Severity = daysUntilDeadline <= 2 ? AlertSeverity.Error : AlertSeverity.Warning,
                            Title = $"Prazo SNGPC se aproximando - {month:D2}/{year}",
                            Description = $"O relatório SNGPC de {month:D2}/{year} deve ser enviado à ANVISA até {deadline:dd/MM/yyyy}. Faltam {daysUntilDeadline} dias.",
                            CreatedAt = now,
                            RelatedReportId = report?.Id
                        });
                    }
                }
            }

            _logger.LogInformation(
                "Found {Count} approaching deadlines for tenant {TenantId}",
                alerts.Count, tenantId);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlert>> CheckOverdueReportsAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Checking for overdue SNGPC reports for tenant {TenantId}", tenantId);

            var alerts = new List<SngpcAlert>();
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
                    alerts.Add(new SngpcAlert
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Type = AlertType.MissingReport,
                        Severity = AlertSeverity.Critical,
                        Title = $"Relatório SNGPC não gerado - {month:D2}/{year}",
                        Description = $"O relatório SNGPC de {month:D2}/{year} não foi gerado. Prazo vencido há {daysOverdue} dias. ANVISA pode aplicar multa.",
                        CreatedAt = now
                    });
                }
                else if (report.Status != SNGPCReportStatus.Transmitted)
                {
                    var daysOverdue = (now - deadline).Days;
                    alerts.Add(new SngpcAlert
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Type = AlertType.DeadlineOverdue,
                        Severity = AlertSeverity.Critical,
                        Title = $"Relatório SNGPC não transmitido - {month:D2}/{year}",
                        Description = $"O relatório SNGPC de {month:D2}/{year} foi gerado mas não transmitido à ANVISA. Prazo vencido há {daysOverdue} dias.",
                        CreatedAt = now,
                        RelatedReportId = report.Id
                    });
                }
            }

            _logger.LogInformation(
                "Found {Count} overdue reports for tenant {TenantId}",
                alerts.Count, tenantId);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlert>> ValidateComplianceAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Validating SNGPC compliance for tenant {TenantId}", tenantId);

            var alerts = new List<SngpcAlert>();
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
                var latestEntry = group.OrderByDescending(e => e.Date).First();
                alerts.Add(new SngpcAlert
                {
                    Id = Guid.NewGuid(),
                    TenantId = tenantId,
                    Type = AlertType.NegativeBalance,
                    Severity = AlertSeverity.Critical,
                    Title = $"Saldo negativo detectado: {group.Key}",
                    Description = $"O medicamento controlado '{group.Key}' apresenta saldo negativo ({latestEntry.Balance}). Isso viola as normas da ANVISA.",
                    CreatedAt = now,
                    RelatedRegistryId = latestEntry.Id,
                    RelatedMedication = group.Key
                });
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
                        alerts.Add(new SngpcAlert
                        {
                            Id = Guid.NewGuid(),
                            TenantId = tenantId,
                            Type = AlertType.InvalidBalance,
                            Severity = AlertSeverity.Error,
                            Title = $"Inconsistência no saldo: {group.Key}",
                            Description = $"O saldo registrado ({current.Balance}) não corresponde ao saldo calculado ({expectedBalance}) para '{group.Key}' em {current.Date:dd/MM/yyyy}.",
                            CreatedAt = now,
                            RelatedRegistryId = current.Id,
                            RelatedMedication = group.Key
                        });
                    }
                }
            }

            _logger.LogInformation(
                "Compliance validation completed for tenant {TenantId}. Found {Count} issues.",
                tenantId, alerts.Count);

            return alerts;
        }

        public async Task<IEnumerable<SngpcAlert>> DetectAnomaliesAsync(
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

            var alerts = new List<SngpcAlert>();
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
                    alerts.Add(new SngpcAlert
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Type = AlertType.ExcessiveDispensing,
                        Severity = AlertSeverity.Warning,
                        Title = $"Dispensação excessiva detectada: {group.Key}",
                        Description = $"Foram dispensadas {entry.QuantityOut} unidades de '{group.Key}' em {entry.Date:dd/MM/yyyy}, significativamente acima da média diária ({averagePerDay:F2}).",
                        CreatedAt = now,
                        RelatedRegistryId = entry.Id,
                        RelatedMedication = group.Key
                    });
                }

                // Check for unusual movement patterns (large stock entries without corresponding dispensing)
                var inboundEntries = group.Where(e => e.RegistryType == RegistryType.Inbound).ToList();
                var largeInbound = inboundEntries.Where(e => e.QuantityIn > totalDispensed * 2).ToList();

                foreach (var entry in largeInbound)
                {
                    alerts.Add(new SngpcAlert
                    {
                        Id = Guid.NewGuid(),
                        TenantId = tenantId,
                        Type = AlertType.UnusualMovement,
                        Severity = AlertSeverity.Info,
                        Title = $"Entrada de estoque incomum: {group.Key}",
                        Description = $"Entrada de {entry.QuantityIn} unidades de '{group.Key}' em {entry.Date:dd/MM/yyyy} é significativamente maior que o padrão de dispensação.",
                        CreatedAt = now,
                        RelatedRegistryId = entry.Id,
                        RelatedMedication = group.Key
                    });
                }
            }

            _logger.LogInformation(
                "Anomaly detection completed for tenant {TenantId}. Found {Count} anomalies.",
                tenantId, alerts.Count);

            return alerts;
        }

        public Task<IEnumerable<SngpcAlert>> GetActiveAlertsAsync(
            string tenantId, 
            AlertSeverity? severity = null)
        {
            // This would be implemented with a proper alerts repository
            // For now, we'll return an empty list as alerts are generated on-demand
            _logger.LogInformation(
                "Getting active alerts for tenant {TenantId} with severity filter: {Severity}",
                tenantId, severity);

            return Task.FromResult(Enumerable.Empty<SngpcAlert>());
        }

        public Task AcknowledgeAlertAsync(Guid alertId, Guid userId, string? notes = null)
        {
            // NOTE: This is a stub implementation. A full implementation would:
            // 1. Have an ISngpcAlertRepository to persist alerts
            // 2. Load the alert from the repository
            // 3. Mark it as acknowledged with timestamp and user
            // 4. Save the changes back to the database
            // 
            // For now, alerts are generated on-demand and not persisted.
            // This is sufficient for compliance monitoring and notifications,
            // but a production system should persist alerts for audit trail.
            
            _logger.LogInformation(
                "Alert acknowledgment called for {AlertId} by user {UserId} (stub implementation)",
                alertId, userId);

            return Task.CompletedTask;
        }

        public Task ResolveAlertAsync(Guid alertId, Guid userId, string resolution)
        {
            // NOTE: This is a stub implementation. A full implementation would:
            // 1. Have an ISngpcAlertRepository to persist alerts
            // 2. Load the alert from the repository
            // 3. Mark it as resolved with timestamp, user, and resolution notes
            // 4. Save the changes back to the database
            // 
            // For now, alerts are generated on-demand and not persisted.
            // This is sufficient for compliance monitoring and notifications,
            // but a production system should persist alerts for audit trail.
            
            _logger.LogInformation(
                "Alert resolution called for {AlertId} by user {UserId}: {Resolution} (stub implementation)",
                alertId, userId, resolution);

            return Task.CompletedTask;
        }
    }
}
