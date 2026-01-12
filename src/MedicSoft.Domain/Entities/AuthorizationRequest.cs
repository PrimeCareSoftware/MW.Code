using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa uma solicitação de autorização prévia de procedimentos
    /// </summary>
    public class AuthorizationRequest : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid PatientHealthInsuranceId { get; private set; }
        public Guid? AppointmentId { get; private set; }
        public string RequestNumber { get; private set; } // Número sequencial da solicitação
        public DateTime RequestDate { get; private set; }
        public AuthorizationStatus Status { get; private set; }
        
        // Autorização
        public string? AuthorizationNumber { get; private set; } // Número fornecido pela operadora
        public DateTime? AuthorizationDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public string? DenialReason { get; private set; }
        
        // Procedimento solicitado
        public string ProcedureCode { get; private set; } // Código TUSS
        public string ProcedureDescription { get; private set; }
        public int Quantity { get; private set; }
        
        // Informações clínicas
        public string? ClinicalIndication { get; private set; }
        public string? Diagnosis { get; private set; } // CID-10
        
        // Navigation properties
        public Patient? Patient { get; private set; }
        public PatientHealthInsurance? PatientHealthInsurance { get; private set; }
        public Appointment? Appointment { get; private set; }
        
        private AuthorizationRequest() 
        { 
            // EF Constructor
            RequestNumber = null!;
            ProcedureCode = null!;
            ProcedureDescription = null!;
        }

        public AuthorizationRequest(
            Guid patientId,
            Guid patientHealthInsuranceId,
            string requestNumber,
            string procedureCode,
            string procedureDescription,
            int quantity,
            string tenantId,
            Guid? appointmentId = null,
            string? clinicalIndication = null,
            string? diagnosis = null) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (patientHealthInsuranceId == Guid.Empty)
                throw new ArgumentException("Patient Health Insurance ID cannot be empty", nameof(patientHealthInsuranceId));
            
            if (string.IsNullOrWhiteSpace(requestNumber))
                throw new ArgumentException("Request number cannot be empty", nameof(requestNumber));
            
            if (string.IsNullOrWhiteSpace(procedureCode))
                throw new ArgumentException("Procedure code cannot be empty", nameof(procedureCode));
            
            if (string.IsNullOrWhiteSpace(procedureDescription))
                throw new ArgumentException("Procedure description cannot be empty", nameof(procedureDescription));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            PatientId = patientId;
            PatientHealthInsuranceId = patientHealthInsuranceId;
            AppointmentId = appointmentId;
            RequestNumber = requestNumber.Trim();
            RequestDate = DateTime.UtcNow;
            Status = AuthorizationStatus.Pending;
            ProcedureCode = procedureCode.Trim();
            ProcedureDescription = procedureDescription.Trim();
            Quantity = quantity;
            ClinicalIndication = clinicalIndication?.Trim();
            Diagnosis = diagnosis?.Trim();
        }

        public void Approve(string authorizationNumber, DateTime? expirationDate = null)
        {
            if (string.IsNullOrWhiteSpace(authorizationNumber))
                throw new ArgumentException("Authorization number cannot be empty", nameof(authorizationNumber));

            if (Status != AuthorizationStatus.Pending)
                throw new InvalidOperationException($"Cannot approve authorization in status {Status}");

            AuthorizationNumber = authorizationNumber.Trim();
            AuthorizationDate = DateTime.UtcNow;
            ExpirationDate = expirationDate;
            Status = AuthorizationStatus.Approved;
            DenialReason = null; // Clear any previous denial reason
            UpdateTimestamp();
        }

        public void Deny(string denialReason)
        {
            if (string.IsNullOrWhiteSpace(denialReason))
                throw new ArgumentException("Denial reason cannot be empty", nameof(denialReason));

            if (Status != AuthorizationStatus.Pending)
                throw new InvalidOperationException($"Cannot deny authorization in status {Status}");

            DenialReason = denialReason.Trim();
            Status = AuthorizationStatus.Denied;
            AuthorizationNumber = null; // Clear any authorization number
            AuthorizationDate = null;
            UpdateTimestamp();
        }

        public void Cancel()
        {
            if (Status == AuthorizationStatus.Cancelled)
                throw new InvalidOperationException("Authorization is already cancelled");

            Status = AuthorizationStatus.Cancelled;
            UpdateTimestamp();
        }

        public void MarkAsExpired()
        {
            if (Status != AuthorizationStatus.Approved)
                throw new InvalidOperationException("Only approved authorizations can be expired");

            Status = AuthorizationStatus.Expired;
            UpdateTimestamp();
        }

        public bool IsExpired()
        {
            if (!ExpirationDate.HasValue)
                return false;

            return DateTime.UtcNow > ExpirationDate.Value;
        }

        public bool IsValidForUse()
        {
            return Status == AuthorizationStatus.Approved && !IsExpired();
        }
    }

    /// <summary>
    /// Status da solicitação de autorização
    /// </summary>
    public enum AuthorizationStatus
    {
        Pending = 1,     // Aguardando análise da operadora
        Approved = 2,    // Aprovada pela operadora
        Denied = 3,      // Negada pela operadora
        Expired = 4,     // Expirada (após data de validade)
        Cancelled = 5    // Cancelada pela clínica
    }
}
