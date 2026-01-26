using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Log de registro de consentimentos e revogações (LGPD Art. 8)
    /// Mantém histórico completo de consentimentos para fins de auditoria
    /// </summary>
    public class DataConsentLog : BaseEntity
    {
        // Titular dos dados
        public Guid PatientId { get; private set; }
        public string PatientName { get; private set; }
        
        // Consentimento
        public ConsentType Type { get; private set; }
        public ConsentPurpose Purpose { get; private set; }
        public string Description { get; private set; }
        
        // Status
        public ConsentStatus Status { get; private set; }
        public DateTime ConsentDate { get; private set; }
        public DateTime? ExpirationDate { get; private set; }
        public DateTime? RevokedDate { get; private set; }
        public string? RevocationReason { get; private set; }
        
        // Contexto
        public string IpAddress { get; private set; }
        public string ConsentText { get; private set; }  // Texto exato apresentado
        public string ConsentVersion { get; private set; }
        
        // Evidência
        public string ConsentMethod { get; private set; }  // WEB, MOBILE, PAPER
        public string UserAgent { get; private set; }

        // Construtor privado para EF Core
        private DataConsentLog()
        {
            PatientName = null!;
            Description = null!;
            IpAddress = null!;
            ConsentText = null!;
            ConsentVersion = null!;
            ConsentMethod = null!;
            UserAgent = null!;
        }

        public DataConsentLog(
            Guid patientId,
            string patientName,
            ConsentType type,
            ConsentPurpose purpose,
            string description,
            ConsentStatus status,
            DateTime? expirationDate,
            string ipAddress,
            string consentText,
            string consentVersion,
            string consentMethod,
            string userAgent,
            string tenantId) : base(tenantId)
        {
            PatientId = patientId;
            PatientName = patientName ?? throw new ArgumentNullException(nameof(patientName));
            Type = type;
            Purpose = purpose;
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Status = status;
            ConsentDate = DateTime.UtcNow;
            ExpirationDate = expirationDate;
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            ConsentText = consentText ?? throw new ArgumentNullException(nameof(consentText));
            ConsentVersion = consentVersion ?? throw new ArgumentNullException(nameof(consentVersion));
            ConsentMethod = consentMethod ?? throw new ArgumentNullException(nameof(consentMethod));
            UserAgent = userAgent ?? throw new ArgumentNullException(nameof(userAgent));
        }

        public void Revoke(string reason)
        {
            if (Status == ConsentStatus.Revoked)
            {
                throw new InvalidOperationException("Consentimento já foi revogado anteriormente.");
            }

            Status = ConsentStatus.Revoked;
            RevokedDate = DateTime.UtcNow;
            RevocationReason = reason;
        }

        public void Expire()
        {
            if (Status == ConsentStatus.Expired)
            {
                throw new InvalidOperationException("Consentimento já expirou.");
            }

            Status = ConsentStatus.Expired;
        }
    }

    /// <summary>
    /// Tipo de consentimento
    /// </summary>
    public enum ConsentType
    {
        MedicalTreatment,      // Tratamento médico
        DataSharing,           // Compartilhamento de dados
        Marketing,             // Comunicações de marketing
        Research,              // Pesquisa clínica
        Telemedicine          // Telemedicina
    }

    /// <summary>
    /// Finalidade do consentimento
    /// </summary>
    public enum ConsentPurpose
    {
        Treatment,                    // Tratamento médico
        DiagnosticProcedures,        // Procedimentos diagnósticos
        DataSharing,                 // Compartilhamento de dados
        MarketingCommunication,      // Comunicação de marketing
        ClinicalResearch,            // Pesquisa clínica
        QualityImprovement          // Melhoria de qualidade
    }

    /// <summary>
    /// Status do consentimento
    /// </summary>
    public enum ConsentStatus
    {
        Active,      // Ativo
        Revoked,     // Revogado
        Expired      // Expirado
    }
}
