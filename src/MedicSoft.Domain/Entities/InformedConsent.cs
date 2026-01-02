using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa o consentimento informado digital conforme CFM 1.821/2007 Art. 3º
    /// </summary>
    public class InformedConsent : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        
        // Navigation properties
        public virtual MedicalRecord MedicalRecord { get; private set; } = null!;
        
        public Guid PatientId { get; private set; }
        public virtual Patient Patient { get; private set; } = null!;
        
        // Texto do consentimento (obrigatório)
        public string ConsentText { get; private set; }
        
        // Aceite do paciente (obrigatório)
        public bool IsAccepted { get; private set; }
        
        // Data/hora do aceite (obrigatório quando aceito)
        public DateTime? AcceptedAt { get; private set; }
        
        // Endereço IP do aceite (rastreabilidade)
        public string? IPAddress { get; private set; }
        
        // Assinatura digital (se implementado)
        public string? DigitalSignature { get; private set; }
        
        // Usuário que registrou o consentimento
        public Guid? RegisteredByUserId { get; private set; }
        
        private InformedConsent()
        {
            // EF Constructor
            ConsentText = null!;
        }
        
        public InformedConsent(
            Guid medicalRecordId,
            Guid patientId,
            string tenantId,
            string consentText,
            Guid? registeredByUserId = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (string.IsNullOrWhiteSpace(consentText))
                throw new ArgumentException("Consent text is required", nameof(consentText));
            
            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            ConsentText = consentText.Trim();
            IsAccepted = false;
            RegisteredByUserId = registeredByUserId;
        }
        
        public void Accept(string? ipAddress = null, string? digitalSignature = null)
        {
            if (IsAccepted)
                throw new InvalidOperationException("Consent has already been accepted");
            
            IsAccepted = true;
            AcceptedAt = DateTime.UtcNow;
            IPAddress = ipAddress?.Trim();
            DigitalSignature = digitalSignature?.Trim();
            
            UpdateTimestamp();
        }
        
        public void Revoke()
        {
            if (!IsAccepted)
                throw new InvalidOperationException("Cannot revoke consent that was not accepted");
            
            IsAccepted = false;
            AcceptedAt = null;
            
            UpdateTimestamp();
        }
        
        public void UpdateConsentText(string consentText)
        {
            if (string.IsNullOrWhiteSpace(consentText))
                throw new ArgumentException("Consent text is required", nameof(consentText));
            
            if (IsAccepted)
                throw new InvalidOperationException("Cannot update consent text after it has been accepted");
            
            ConsentText = consentText.Trim();
            UpdateTimestamp();
        }
        
        public bool IsValid()
        {
            return IsAccepted && AcceptedAt.HasValue;
        }
    }
}
