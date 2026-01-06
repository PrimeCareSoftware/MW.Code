using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a digital medical prescription compliant with CFM 1.643/2002 and ANVISA 344/1998.
    /// Implements controlled substance tracking and SNGPC requirements.
    /// </summary>
    public class DigitalPrescription : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid DoctorId { get; private set; }
        
        // Prescription Type (CFM/ANVISA)
        public PrescriptionType Type { get; private set; }
        
        // Sequential numbering for controlled prescriptions (ANVISA requirement)
        public string? SequenceNumber { get; private set; }
        
        // Prescription metadata
        public DateTime IssuedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsActive { get; private set; }
        
        // Doctor information (CFM 1.643/2002 requirement)
        public string DoctorName { get; private set; }
        public string DoctorCRM { get; private set; }
        public string DoctorCRMState { get; private set; }
        
        // Patient information (stored for prescription integrity)
        public string PatientName { get; private set; }
        public string PatientDocument { get; private set; } // CPF or RG
        
        // Digital signature (ICP-Brasil ready)
        public string? DigitalSignature { get; private set; }
        public DateTime? SignedAt { get; private set; }
        public string? SignatureCertificate { get; private set; } // Certificate thumbprint
        
        // QR Code / Barcode for verification
        public string? VerificationCode { get; private set; }
        
        // SNGPC tracking (for controlled substances)
        public bool RequiresSNGPCReport { get; private set; }
        public DateTime? ReportedToSNGPCAt { get; private set; }
        
        // Retention requirement (CFM requires 20 years)
        public string? Notes { get; private set; }
        
        // Navigation properties
        public MedicalRecord? MedicalRecord { get; private set; }
        public Patient? Patient { get; private set; }
        public User? Doctor { get; private set; }
        
        private readonly List<DigitalPrescriptionItem> _items = new();
        public IReadOnlyCollection<DigitalPrescriptionItem> Items => _items.AsReadOnly();

        private DigitalPrescription()
        {
            // EF Constructor
            DoctorName = null!;
            DoctorCRM = null!;
            DoctorCRMState = null!;
            PatientName = null!;
            PatientDocument = null!;
        }

        public DigitalPrescription(
            Guid medicalRecordId,
            Guid patientId,
            Guid doctorId,
            PrescriptionType type,
            string doctorName,
            string doctorCRM,
            string doctorCRMState,
            string patientName,
            string patientDocument,
            string tenantId,
            string? sequenceNumber = null,
            string? notes = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (doctorId == Guid.Empty)
                throw new ArgumentException("Doctor ID cannot be empty", nameof(doctorId));
            
            if (string.IsNullOrWhiteSpace(doctorName))
                throw new ArgumentException("Doctor name is required", nameof(doctorName));
            
            if (string.IsNullOrWhiteSpace(doctorCRM))
                throw new ArgumentException("Doctor CRM is required (CFM 1.643/2002)", nameof(doctorCRM));
            
            if (string.IsNullOrWhiteSpace(doctorCRMState))
                throw new ArgumentException("Doctor CRM state is required", nameof(doctorCRMState));
            
            if (string.IsNullOrWhiteSpace(patientName))
                throw new ArgumentException("Patient name is required", nameof(patientName));
            
            if (string.IsNullOrWhiteSpace(patientDocument))
                throw new ArgumentException("Patient document is required", nameof(patientDocument));

            MedicalRecordId = medicalRecordId;
            PatientId = patientId;
            DoctorId = doctorId;
            Type = type;
            
            DoctorName = doctorName.Trim();
            DoctorCRM = doctorCRM.Trim();
            DoctorCRMState = doctorCRMState.Trim().ToUpperInvariant();
            
            PatientName = patientName.Trim();
            PatientDocument = patientDocument.Trim();
            
            SequenceNumber = sequenceNumber?.Trim();
            Notes = notes?.Trim();
            
            IssuedAt = DateTime.UtcNow;
            ExpiresAt = CalculateExpirationDate(type);
            IsActive = true;
            
            // Determine if SNGPC reporting is required based on type
            RequiresSNGPCReport = type == PrescriptionType.SpecialControlA || 
                                   type == PrescriptionType.SpecialControlB ||
                                   type == PrescriptionType.SpecialControlC1;
            
            // Generate verification code (QR Code / Barcode)
            VerificationCode = GenerateVerificationCode();
        }

        private DateTime CalculateExpirationDate(PrescriptionType type)
        {
            return type switch
            {
                PrescriptionType.Simple => IssuedAt.AddDays(30), // 30 days for simple prescriptions
                PrescriptionType.SpecialControlA => IssuedAt.AddDays(30), // 30 days (entorpecentes)
                PrescriptionType.SpecialControlB => IssuedAt.AddDays(30), // 30 days (psicotrópicos)
                PrescriptionType.SpecialControlC1 => IssuedAt.AddDays(30), // 30 days (outros controlados)
                PrescriptionType.Antimicrobial => IssuedAt.AddDays(10), // 10 days for antibiotics
                _ => IssuedAt.AddDays(30)
            };
        }

        private string GenerateVerificationCode()
        {
            // Generate a unique verification code based on prescription data
            // Format: TENANT-TYPE-YYYYMMDD-SEQUENCE
            var dateStr = IssuedAt.ToString("yyyyMMdd");
            var typeCode = ((int)Type).ToString("D2");
            var uniquePart = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
            return $"{typeCode}-{dateStr}-{uniquePart}";
        }

        public void AddItem(DigitalPrescriptionItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            
            if (!IsActive)
                throw new InvalidOperationException("Cannot add items to an inactive prescription");
            
            if (DigitalSignature != null)
                throw new InvalidOperationException("Cannot modify a signed prescription");
            
            _items.Add(item);
            UpdateTimestamp();
        }

        public void RemoveItem(Guid itemId)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot remove items from an inactive prescription");
            
            if (DigitalSignature != null)
                throw new InvalidOperationException("Cannot modify a signed prescription");
            
            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _items.Remove(item);
                UpdateTimestamp();
            }
        }

        public void SignPrescription(string digitalSignature, string certificateThumbprint)
        {
            if (string.IsNullOrWhiteSpace(digitalSignature))
                throw new ArgumentException("Digital signature cannot be empty", nameof(digitalSignature));
            
            if (string.IsNullOrWhiteSpace(certificateThumbprint))
                throw new ArgumentException("Certificate thumbprint cannot be empty", nameof(certificateThumbprint));
            
            if (DigitalSignature != null)
                throw new InvalidOperationException("Prescription is already signed");
            
            if (!_items.Any())
                throw new InvalidOperationException("Cannot sign an empty prescription");

            DigitalSignature = digitalSignature;
            SignatureCertificate = certificateThumbprint;
            SignedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsReportedToSNGPC()
        {
            if (!RequiresSNGPCReport)
                throw new InvalidOperationException("This prescription type does not require SNGPC reporting");
            
            ReportedToSNGPCAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void Reactivate()
        {
            if (DateTime.UtcNow > ExpiresAt)
                throw new InvalidOperationException("Cannot reactivate an expired prescription");
            
            IsActive = true;
            UpdateTimestamp();
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        public bool IsValid()
        {
            return IsActive && !IsExpired() && _items.Any();
        }

        public int DaysUntilExpiration()
        {
            if (IsExpired())
                return 0;
            
            return (int)(ExpiresAt - DateTime.UtcNow).TotalDays;
        }
    }

    /// <summary>
    /// Types of medical prescriptions according to CFM 1.643/2002 and ANVISA 344/1998.
    /// </summary>
    public enum PrescriptionType
    {
        /// <summary>
        /// Receita simples - medicamentos comuns (validade 30 dias)
        /// </summary>
        Simple = 1,

        /// <summary>
        /// Receita de Controle Especial B - Psicotrópicos (validade 30 dias, retenção obrigatória)
        /// Lista B1 e B2 da Portaria 344/98
        /// Ex: benzodiazepínicos, anfetaminas
        /// </summary>
        SpecialControlB = 2,

        /// <summary>
        /// Receita de Controle Especial A - Entorpecentes (validade 30 dias, 2 vias, retenção)
        /// Lista A1, A2, A3 da Portaria 344/98
        /// Ex: morfina, codeína, metadona
        /// </summary>
        SpecialControlA = 3,

        /// <summary>
        /// Receita Antimicrobiana - Notificação especial (validade 10 dias)
        /// Antibióticos requerem retenção da receita
        /// </summary>
        Antimicrobial = 4,

        /// <summary>
        /// Receita de Controle Especial C1 - Outros controlados (validade 30 dias)
        /// Lista C1 da Portaria 344/98
        /// Ex: anticonvulsivantes, imunossupressores
        /// </summary>
        SpecialControlC1 = 5
    }

    /// <summary>
    /// ANVISA controlled substance classification (Portaria 344/1998).
    /// </summary>
    public enum ControlledSubstanceList
    {
        /// <summary>
        /// Not a controlled substance
        /// </summary>
        None = 0,

        /// <summary>
        /// Lista A1 - Entorpecentes (narcóticos)
        /// </summary>
        A1_Narcotics = 1,

        /// <summary>
        /// Lista A2 - Entorpecentes (psicotrópicos)
        /// </summary>
        A2_Psychotropics = 2,

        /// <summary>
        /// Lista A3 - Psicotrópicos
        /// </summary>
        A3_Psychotropics = 3,

        /// <summary>
        /// Lista B1 - Psicotrópicos
        /// </summary>
        B1_Psychotropics = 4,

        /// <summary>
        /// Lista B2 - Psicotrópicos anorexígenos
        /// </summary>
        B2_Anorexigenics = 5,

        /// <summary>
        /// Lista C1 - Outras substâncias sujeitas a controle especial
        /// </summary>
        C1_OtherControlled = 6,

        /// <summary>
        /// Lista C2 - Retinóides de uso sistêmico
        /// </summary>
        C2_Retinoids = 7,

        /// <summary>
        /// Lista C3 - Imunossupressores
        /// </summary>
        C3_Immunosuppressants = 8,

        /// <summary>
        /// Lista C4 - Antirretrovirais
        /// </summary>
        C4_Antiretrovirals = 9,

        /// <summary>
        /// Lista C5 - Anabolizantes
        /// </summary>
        C5_Anabolics = 10
    }
}
