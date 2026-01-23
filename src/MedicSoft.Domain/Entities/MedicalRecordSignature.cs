using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// CFM 1.638/2002 - Digital signature entity for medical record versions
    /// Preparation for ICP-Brasil digital signature implementation
    /// Full implementation will be done in a separate task
    /// </summary>
    public class MedicalRecordSignature : BaseEntity
    {
        public Guid MedicalRecordVersionId { get; private set; }
        public Guid SignedByUserId { get; private set; }
        public DateTime SignedAt { get; private set; }
        public string SignatureType { get; private set; } // ICP-Brasil, Simple, etc.
        public string? SignatureValue { get; private set; } // Digital signature data
        public string? CertificateData { get; private set; } // Certificate information
        
        // Navigation properties
        public MedicalRecordVersion Version { get; private set; } = null!;
        public User SignedBy { get; private set; } = null!;

        private MedicalRecordSignature()
        {
            // EF Constructor
            SignatureType = null!;
        }

        public MedicalRecordSignature(
            Guid medicalRecordVersionId,
            Guid signedByUserId,
            string signatureType,
            string tenantId,
            string? signatureValue = null,
            string? certificateData = null) : base(tenantId)
        {
            if (medicalRecordVersionId == Guid.Empty)
                throw new ArgumentException("Medical record version ID cannot be empty", nameof(medicalRecordVersionId));
            
            if (signedByUserId == Guid.Empty)
                throw new ArgumentException("Signed by user ID cannot be empty", nameof(signedByUserId));
            
            if (string.IsNullOrWhiteSpace(signatureType))
                throw new ArgumentException("Signature type is required", nameof(signatureType));

            MedicalRecordVersionId = medicalRecordVersionId;
            SignedByUserId = signedByUserId;
            SignatureType = signatureType;
            SignedAt = DateTime.UtcNow;
            SignatureValue = signatureValue?.Trim();
            CertificateData = certificateData?.Trim();
        }
    }
}
