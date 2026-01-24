using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a detailed transmission attempt of SNGPC data to ANVISA.
    /// Tracks all transmission attempts, responses, and protocol numbers.
    /// </summary>
    public class SngpcTransmission : BaseEntity
    {
        public Guid SNGPCReportId { get; private set; }
        
        // Transmission Metadata
        public int AttemptNumber { get; private set; }
        public DateTime AttemptedAt { get; private set; }
        public TransmissionStatus Status { get; private set; }
        
        // ANVISA Response
        public string? ProtocolNumber { get; private set; }
        public string? AnvisaResponse { get; private set; }
        public string? ErrorMessage { get; private set; }
        public string? ErrorCode { get; private set; }
        
        // Transmission Details
        public string? TransmissionMethod { get; private set; } // "WebService", "Manual", "API"
        public string? EndpointUrl { get; private set; }
        public int? HttpStatusCode { get; private set; }
        public long? ResponseTimeMs { get; private set; }
        
        // File Information
        public string? XmlHash { get; private set; } // SHA-256 of transmitted XML
        public long? XmlSizeBytes { get; private set; }
        
        // User Tracking
        public Guid? InitiatedByUserId { get; private set; }
        
        // Navigation Properties
        public SNGPCReport? Report { get; private set; }
        public User? InitiatedBy { get; private set; }

        private SngpcTransmission()
        {
            // EF Constructor
        }

        public SngpcTransmission(
            string tenantId,
            Guid sngpcReportId,
            int attemptNumber,
            string transmissionMethod,
            string? endpointUrl,
            string xmlHash,
            long xmlSizeBytes,
            Guid? initiatedByUserId) : base(tenantId)
        {
            if (sngpcReportId == Guid.Empty)
                throw new ArgumentException("SNGPC Report ID cannot be empty", nameof(sngpcReportId));
            
            if (attemptNumber <= 0)
                throw new ArgumentException("Attempt number must be greater than zero", nameof(attemptNumber));
            
            if (string.IsNullOrWhiteSpace(transmissionMethod))
                throw new ArgumentException("Transmission method cannot be empty", nameof(transmissionMethod));
            
            if (string.IsNullOrWhiteSpace(xmlHash))
                throw new ArgumentException("XML hash cannot be empty", nameof(xmlHash));
            
            if (xmlSizeBytes <= 0)
                throw new ArgumentException("XML size must be greater than zero", nameof(xmlSizeBytes));

            SNGPCReportId = sngpcReportId;
            AttemptNumber = attemptNumber;
            AttemptedAt = DateTime.UtcNow;
            Status = TransmissionStatus.Pending;
            TransmissionMethod = transmissionMethod.Trim();
            EndpointUrl = endpointUrl?.Trim();
            XmlHash = xmlHash;
            XmlSizeBytes = xmlSizeBytes;
            InitiatedByUserId = initiatedByUserId;
        }

        public void MarkAsSuccessful(
            string protocolNumber,
            string? anvisaResponse,
            int httpStatusCode,
            long responseTimeMs)
        {
            if (string.IsNullOrWhiteSpace(protocolNumber))
                throw new ArgumentException("Protocol number cannot be empty", nameof(protocolNumber));
            
            if (Status != TransmissionStatus.Pending && Status != TransmissionStatus.InProgress)
                throw new InvalidOperationException("Can only mark pending or in-progress transmissions as successful");

            Status = TransmissionStatus.Successful;
            ProtocolNumber = protocolNumber.Trim();
            AnvisaResponse = anvisaResponse?.Trim();
            HttpStatusCode = httpStatusCode;
            ResponseTimeMs = responseTimeMs;
            ErrorMessage = null;
            ErrorCode = null;
            UpdateTimestamp();
        }

        public void MarkAsFailed(
            string errorMessage,
            string? errorCode,
            int? httpStatusCode,
            long? responseTimeMs)
        {
            if (string.IsNullOrWhiteSpace(errorMessage))
                throw new ArgumentException("Error message cannot be empty", nameof(errorMessage));
            
            if (Status != TransmissionStatus.Pending && Status != TransmissionStatus.InProgress)
                throw new InvalidOperationException("Can only mark pending or in-progress transmissions as failed");

            Status = TransmissionStatus.Failed;
            ErrorMessage = errorMessage.Trim();
            ErrorCode = errorCode?.Trim();
            HttpStatusCode = httpStatusCode;
            ResponseTimeMs = responseTimeMs;
            UpdateTimestamp();
        }

        public void MarkAsInProgress()
        {
            if (Status != TransmissionStatus.Pending)
                throw new InvalidOperationException("Can only mark pending transmissions as in progress");

            Status = TransmissionStatus.InProgress;
            UpdateTimestamp();
        }

        public void MarkAsTimedOut(long responseTimeMs)
        {
            if (Status != TransmissionStatus.Pending && Status != TransmissionStatus.InProgress)
                throw new InvalidOperationException("Can only mark pending or in-progress transmissions as timed out");

            Status = TransmissionStatus.TimedOut;
            ErrorMessage = "Transmission timed out";
            ErrorCode = "TIMEOUT";
            ResponseTimeMs = responseTimeMs;
            UpdateTimestamp();
        }

        public void MarkAsRetrying()
        {
            if (Status != TransmissionStatus.Failed && Status != TransmissionStatus.TimedOut)
                throw new InvalidOperationException("Can only retry failed or timed-out transmissions");

            Status = TransmissionStatus.Pending;
            UpdateTimestamp();
        }

        public bool CanRetry()
        {
            return Status == TransmissionStatus.Failed || Status == TransmissionStatus.TimedOut;
        }

        public bool IsSuccessful()
        {
            return Status == TransmissionStatus.Successful;
        }

        public bool IsPending()
        {
            return Status == TransmissionStatus.Pending || Status == TransmissionStatus.InProgress;
        }
    }

    /// <summary>
    /// Status of SNGPC transmission attempt
    /// </summary>
    public enum TransmissionStatus
    {
        /// <summary>
        /// Transmission is queued and waiting to be sent
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Transmission is currently being processed
        /// </summary>
        InProgress = 2,

        /// <summary>
        /// Transmission was successful and confirmed by ANVISA
        /// </summary>
        Successful = 3,

        /// <summary>
        /// Transmission failed with an error
        /// </summary>
        Failed = 4,

        /// <summary>
        /// Transmission timed out
        /// </summary>
        TimedOut = 5,

        /// <summary>
        /// Transmission was cancelled by user
        /// </summary>
        Cancelled = 6
    }
}
