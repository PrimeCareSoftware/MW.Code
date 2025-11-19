using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the N:N relationship between Owners and Clinics.
    /// An owner can own multiple clinics, each with its own separate license/subscription.
    /// This enables franchise-style clinic management where one owner manages multiple locations.
    /// </summary>
    public class OwnerClinicLink : BaseEntity
    {
        public Guid OwnerId { get; private set; }
        public Guid ClinicId { get; private set; }
        public DateTime LinkedDate { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsPrimaryOwner { get; private set; }
        public string? Role { get; private set; } // e.g., "Owner", "Co-Owner", "Partner"
        public decimal? OwnershipPercentage { get; private set; } // Optional: for tracking ownership stakes
        public DateTime? InactivatedDate { get; private set; }
        public string? InactivationReason { get; private set; }

        // Navigation properties
        public Owner? Owner { get; private set; }
        public Clinic? Clinic { get; private set; }

        private OwnerClinicLink()
        {
            // EF Constructor
        }

        public OwnerClinicLink(Guid ownerId, Guid clinicId, string tenantId, 
            bool isPrimaryOwner = true, string? role = null, decimal? ownershipPercentage = null) 
            : base(tenantId)
        {
            if (ownerId == Guid.Empty)
                throw new ArgumentException("Owner ID cannot be empty", nameof(ownerId));

            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (ownershipPercentage.HasValue && (ownershipPercentage.Value < 0 || ownershipPercentage.Value > 100))
                throw new ArgumentException("Ownership percentage must be between 0 and 100", nameof(ownershipPercentage));

            OwnerId = ownerId;
            ClinicId = clinicId;
            LinkedDate = DateTime.UtcNow;
            IsActive = true;
            IsPrimaryOwner = isPrimaryOwner;
            Role = role?.Trim();
            OwnershipPercentage = ownershipPercentage;
        }

        public void SetAsPrimary()
        {
            IsPrimaryOwner = true;
            UpdateTimestamp();
        }

        public void RemoveAsPrimary()
        {
            IsPrimaryOwner = false;
            UpdateTimestamp();
        }

        public void UpdateRole(string? role)
        {
            Role = role?.Trim();
            UpdateTimestamp();
        }

        public void UpdateOwnershipPercentage(decimal? percentage)
        {
            if (percentage.HasValue && (percentage.Value < 0 || percentage.Value > 100))
                throw new ArgumentException("Ownership percentage must be between 0 and 100", nameof(percentage));

            OwnershipPercentage = percentage;
            UpdateTimestamp();
        }

        public void Deactivate(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Inactivation reason is required", nameof(reason));

            IsActive = false;
            InactivatedDate = DateTime.UtcNow;
            InactivationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void Reactivate()
        {
            IsActive = true;
            InactivatedDate = null;
            InactivationReason = null;
            UpdateTimestamp();
        }
    }
}
