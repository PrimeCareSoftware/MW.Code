using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// CFM 1.638/2002 - Access log entity for medical records audit
    /// Tracks all access to medical records for compliance and security
    /// </summary>
    public class MedicalRecordAccessLog : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public Guid UserId { get; private set; }
        public string AccessType { get; private set; } // View, Edit, Close, Reopen, Print, Export
        public DateTime AccessedAt { get; private set; }
        public string? IpAddress { get; private set; }
        public string? UserAgent { get; private set; }
        public string? Details { get; private set; } // Additional information
        
        // Navigation properties
        public MedicalRecord MedicalRecord { get; private set; } = null!;
        public User User { get; private set; } = null!;

        private MedicalRecordAccessLog()
        {
            // EF Constructor
            AccessType = null!;
        }

        public MedicalRecordAccessLog(
            Guid medicalRecordId,
            Guid userId,
            string accessType,
            string tenantId,
            string? ipAddress = null,
            string? userAgent = null,
            string? details = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(accessType))
                throw new ArgumentException("Access type is required", nameof(accessType));

            MedicalRecordId = medicalRecordId;
            UserId = userId;
            AccessType = accessType;
            AccessedAt = DateTime.UtcNow;
            IpAddress = ipAddress?.Trim();
            UserAgent = userAgent?.Trim();
            Details = details?.Trim();
        }
    }
}
