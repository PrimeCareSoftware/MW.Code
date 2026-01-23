using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the N:N relationship between Users and Clinics.
    /// A user can work at multiple clinics within the same company.
    /// This allows flexibility for healthcare professionals who work at different locations.
    /// </summary>
    public class UserClinicLink : BaseEntity
    {
        public Guid UserId { get; private set; }
        public Guid ClinicId { get; private set; }
        public DateTime LinkedDate { get; private set; }
        public bool IsActive { get; private set; }
        public bool IsPreferredClinic { get; private set; } // The default clinic where user authenticates
        public DateTime? InactivatedDate { get; private set; }
        public string? InactivationReason { get; private set; }

        // Navigation properties
        public User? User { get; private set; }
        public Clinic? Clinic { get; private set; }

        private UserClinicLink()
        {
            // EF Constructor
        }

        public UserClinicLink(Guid userId, Guid clinicId, string tenantId, bool isPreferredClinic = false) 
            : base(tenantId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            UserId = userId;
            ClinicId = clinicId;
            LinkedDate = DateTime.UtcNow;
            IsActive = true;
            IsPreferredClinic = isPreferredClinic;
        }

        public void SetAsPreferred()
        {
            IsPreferredClinic = true;
            UpdateTimestamp();
        }

        public void RemoveAsPreferred()
        {
            IsPreferredClinic = false;
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
