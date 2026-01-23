using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// CFM 1.638/2002 - Version history entity for medical records
    /// Implements event sourcing pattern to maintain complete audit trail
    /// </summary>
    public class MedicalRecordVersion : BaseEntity
    {
        public Guid MedicalRecordId { get; private set; }
        public int Version { get; private set; }
        public string ChangeType { get; private set; } // Created, Updated, Closed, Reopened
        public DateTime ChangedAt { get; private set; }
        public Guid ChangedByUserId { get; private set; }
        public string? ChangeReason { get; private set; } // Required for reopenings
        public string SnapshotJson { get; private set; } // Complete state snapshot
        public string? ChangesSummary { get; private set; } // Summary of changes
        
        // CFM 1.638/2002 - Content integrity validation
        public string ContentHash { get; private set; } // SHA-256 hash
        public string? PreviousVersionHash { get; private set; } // Blockchain-like chain
        
        // Navigation properties
        public MedicalRecord MedicalRecord { get; private set; } = null!;
        public User ChangedBy { get; private set; } = null!;

        private MedicalRecordVersion()
        {
            // EF Constructor
            ChangeType = null!;
            SnapshotJson = null!;
            ContentHash = null!;
        }

        public MedicalRecordVersion(
            Guid medicalRecordId,
            int version,
            string changeType,
            Guid changedByUserId,
            string snapshotJson,
            string contentHash,
            string tenantId,
            string? changeReason = null,
            string? changesSummary = null,
            string? previousVersionHash = null) : base(tenantId)
        {
            if (medicalRecordId == Guid.Empty)
                throw new ArgumentException("Medical record ID cannot be empty", nameof(medicalRecordId));
            
            if (version < 1)
                throw new ArgumentException("Version must be greater than 0", nameof(version));
            
            if (string.IsNullOrWhiteSpace(changeType))
                throw new ArgumentException("Change type is required", nameof(changeType));
            
            if (changedByUserId == Guid.Empty)
                throw new ArgumentException("Changed by user ID cannot be empty", nameof(changedByUserId));
            
            if (string.IsNullOrWhiteSpace(snapshotJson))
                throw new ArgumentException("Snapshot JSON is required", nameof(snapshotJson));
            
            if (string.IsNullOrWhiteSpace(contentHash))
                throw new ArgumentException("Content hash is required", nameof(contentHash));

            MedicalRecordId = medicalRecordId;
            Version = version;
            ChangeType = changeType;
            ChangedAt = DateTime.UtcNow;
            ChangedByUserId = changedByUserId;
            SnapshotJson = snapshotJson;
            ContentHash = contentHash;
            ChangeReason = changeReason?.Trim();
            ChangesSummary = changesSummary?.Trim();
            PreviousVersionHash = previousVersionHash?.Trim();
        }
    }
}
