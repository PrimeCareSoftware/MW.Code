using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Requisição de direito ao esquecimento (LGPD Art. 18)
    /// Registra solicitações de exclusão ou anonimização de dados
    /// </summary>
    public class DataDeletionRequest : BaseEntity
    {
        // Solicitante
        public Guid PatientId { get; private set; }
        public string PatientName { get; private set; }
        public string PatientEmail { get; private set; }
        
        // Requisição
        public DateTime RequestDate { get; private set; }
        public string Reason { get; private set; }
        public DeletionRequestType RequestType { get; private set; }
        
        // Status
        public DeletionRequestStatus Status { get; private set; }
        public DateTime? ProcessedDate { get; private set; }
        public DateTime? CompletedDate { get; private set; }
        
        // Processamento
        public string? ProcessedByUserId { get; private set; }
        public string? ProcessedByUserName { get; private set; }
        public string? ProcessingNotes { get; private set; }
        public string? RejectionReason { get; private set; }
        
        // Evidência
        public string IpAddress { get; private set; }
        public string UserAgent { get; private set; }
        
        // Legal
        public bool RequiresLegalApproval { get; private set; }
        public DateTime? LegalApprovalDate { get; private set; }
        public string? LegalApprover { get; private set; }

        // Construtor privado para EF Core
        private DataDeletionRequest()
        {
            PatientName = null!;
            PatientEmail = null!;
            Reason = null!;
            IpAddress = null!;
            UserAgent = null!;
        }

        public DataDeletionRequest(
            Guid patientId,
            string patientName,
            string patientEmail,
            string reason,
            DeletionRequestType requestType,
            string ipAddress,
            string userAgent,
            bool requiresLegalApproval,
            string tenantId) : base(tenantId)
        {
            PatientId = patientId;
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
            PatientEmail = patientEmail ?? throw new ArgumentNullException(nameof(patientEmail));
            RequestDate = DateTime.UtcNow;
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            RequestType = requestType;
            Status = DeletionRequestStatus.Pending;
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
            RequiresLegalApproval = requiresLegalApproval;
        }

        public void Process(string userId, string userName, string notes)
        {
            if (Status != DeletionRequestStatus.Pending)
            {
                throw new InvalidOperationException($"Cannot process request in status: {Status}");
            }

            Status = DeletionRequestStatus.Processing;
            ProcessedDate = DateTime.UtcNow;
            ProcessedByUserId = userId;
            ProcessedByUserName = userName;
            ProcessingNotes = notes;
        }

        public void Complete()
        {
            if (Status != DeletionRequestStatus.Processing)
            {
                throw new InvalidOperationException($"Cannot complete request in status: {Status}");
            }

            Status = DeletionRequestStatus.Completed;
            CompletedDate = DateTime.UtcNow;
        }

        public void Reject(string reason)
        {
            if (Status == DeletionRequestStatus.Completed)
            {
                throw new InvalidOperationException("Cannot reject a completed request");
            }

            Status = DeletionRequestStatus.Rejected;
            RejectionReason = reason;
            CompletedDate = DateTime.UtcNow;
        }

        public void ApproveLegal(string approver)
        {
            if (!RequiresLegalApproval)
            {
                throw new InvalidOperationException("This request does not require legal approval");
            }

            LegalApprovalDate = DateTime.UtcNow;
            LegalApprover = approver;
        }
    }

    /// <summary>
    /// Tipo de requisição de exclusão
    /// </summary>
    public enum DeletionRequestType
    {
        Complete,         // Exclusão completa
        Anonymization,    // Anonimização (mantém dados estatísticos)
        Partial          // Exclusão parcial (específica)
    }

    /// <summary>
    /// Status da requisição de exclusão
    /// </summary>
    public enum DeletionRequestStatus
    {
        Pending,      // Pendente
        Processing,   // Em processamento
        Completed,    // Concluída
        Rejected      // Rejeitada
    }
}
