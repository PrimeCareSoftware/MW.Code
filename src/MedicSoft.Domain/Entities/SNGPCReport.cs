using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados) report.
    /// Required by ANVISA for controlled substance dispensing tracking.
    /// Reports must be transmitted monthly to ANVISA.
    /// </summary>
    public class SNGPCReport : BaseEntity
    {
        public int Month { get; private set; }
        public int Year { get; private set; }
        public DateTime ReportPeriodStart { get; private set; }
        public DateTime ReportPeriodEnd { get; private set; }
        
        // Report metadata
        public SNGPCReportStatus Status { get; private set; }
        public DateTime GeneratedAt { get; private set; }
        public DateTime? TransmittedAt { get; private set; }
        public string? TransmissionProtocol { get; private set; } // ANVISA protocol number
        
        // XML data
        public string? XmlContent { get; private set; }
        public string? XmlHash { get; private set; } // SHA-256 hash for integrity
        
        // Statistics
        public int TotalPrescriptions { get; private set; }
        public int TotalItems { get; private set; }
        
        // Error tracking
        public string? ErrorMessage { get; private set; }
        public DateTime? LastAttemptAt { get; private set; }
        public int AttemptCount { get; private set; }

        private readonly List<Guid> _prescriptionIds = new();
        public IReadOnlyCollection<Guid> PrescriptionIds => _prescriptionIds.AsReadOnly();

        private SNGPCReport()
        {
            // EF Constructor
        }

        public SNGPCReport(
            int month,
            int year,
            string tenantId) : base(tenantId)
        {
            if (month < 1 || month > 12)
                throw new ArgumentException("Month must be between 1 and 12", nameof(month));
            
            if (year < 2000 || year > DateTime.UtcNow.Year)
                throw new ArgumentException("Invalid year", nameof(year));

            Month = month;
            Year = year;
            
            // Calculate period
            ReportPeriodStart = new DateTime(year, month, 1, 0, 0, 0, DateTimeKind.Utc);
            ReportPeriodEnd = ReportPeriodStart.AddMonths(1).AddDays(-1).AddHours(23).AddMinutes(59).AddSeconds(59);
            
            Status = SNGPCReportStatus.Draft;
            GeneratedAt = DateTime.UtcNow;
            AttemptCount = 0;
        }

        public void AddPrescription(Guid prescriptionId)
        {
            if (prescriptionId == Guid.Empty)
                throw new ArgumentException("Prescription ID cannot be empty", nameof(prescriptionId));
            
            if (Status != SNGPCReportStatus.Draft)
                throw new InvalidOperationException("Cannot add prescriptions to a report that is not in draft status");
            
            if (_prescriptionIds.Contains(prescriptionId))
                return; // Already added
            
            _prescriptionIds.Add(prescriptionId);
            TotalPrescriptions++;
            UpdateTimestamp();
        }

        public void GenerateXML(string xmlContent, int totalItems)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                throw new ArgumentException("XML content cannot be empty", nameof(xmlContent));
            
            if (Status != SNGPCReportStatus.Draft)
                throw new InvalidOperationException("Cannot generate XML for a report that is not in draft status");
            
            if (!_prescriptionIds.Any())
                throw new InvalidOperationException("Cannot generate XML for an empty report");

            XmlContent = xmlContent;
            TotalItems = totalItems;
            
            // Calculate SHA-256 hash for integrity
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(xmlContent));
                XmlHash = BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
            
            Status = SNGPCReportStatus.Generated;
            UpdateTimestamp();
        }

        public void MarkAsTransmitted(string transmissionProtocol)
        {
            if (string.IsNullOrWhiteSpace(transmissionProtocol))
                throw new ArgumentException("Transmission protocol cannot be empty", nameof(transmissionProtocol));
            
            if (Status != SNGPCReportStatus.Generated && Status != SNGPCReportStatus.TransmissionFailed)
                throw new InvalidOperationException("Can only transmit a generated report or retry a failed transmission");
            
            if (string.IsNullOrWhiteSpace(XmlContent))
                throw new InvalidOperationException("Cannot transmit a report without XML content");

            Status = SNGPCReportStatus.Transmitted;
            TransmittedAt = DateTime.UtcNow;
            TransmissionProtocol = transmissionProtocol.Trim();
            LastAttemptAt = DateTime.UtcNow;
            AttemptCount++;
            ErrorMessage = null;
            
            UpdateTimestamp();
        }

        public void MarkAsTransmissionFailed(string errorMessage)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));
            
            if (Status != SNGPCReportStatus.Generated && Status != SNGPCReportStatus.TransmissionFailed)
                throw new InvalidOperationException("Can only mark transmission as failed for generated reports");

            Status = SNGPCReportStatus.TransmissionFailed;
            ErrorMessage = errorMessage.Trim();
            LastAttemptAt = DateTime.UtcNow;
            AttemptCount++;
            
            UpdateTimestamp();
        }

        public void MarkAsValidated()
        {
            if (Status != SNGPCReportStatus.Transmitted)
                throw new InvalidOperationException("Can only validate a transmitted report");

            Status = SNGPCReportStatus.Validated;
            UpdateTimestamp();
        }

        public bool CanRetryTransmission()
        {
            return Status == SNGPCReportStatus.TransmissionFailed && AttemptCount < 5;
        }

        public bool IsOverdue()
        {
            // Reports must be transmitted by the 10th day of the following month
            var deadline = ReportPeriodEnd.AddDays(10);
            return DateTime.UtcNow > deadline && Status != SNGPCReportStatus.Transmitted && Status != SNGPCReportStatus.Validated;
        }

        public int DaysUntilDeadline()
        {
            var deadline = ReportPeriodEnd.AddDays(10);
            var days = (deadline - DateTime.UtcNow).Days;
            return days > 0 ? days : 0;
        }
    }

    /// <summary>
    /// Status of SNGPC report processing and transmission.
    /// </summary>
    public enum SNGPCReportStatus
    {
        /// <summary>
        /// Report is being prepared (prescriptions being added)
        /// </summary>
        Draft = 1,

        /// <summary>
        /// XML has been generated but not yet transmitted
        /// </summary>
        Generated = 2,

        /// <summary>
        /// Report has been transmitted to ANVISA successfully
        /// </summary>
        Transmitted = 3,

        /// <summary>
        /// Transmission failed - can be retried
        /// </summary>
        TransmissionFailed = 4,

        /// <summary>
        /// Report has been validated by ANVISA
        /// </summary>
        Validated = 5
    }
}
