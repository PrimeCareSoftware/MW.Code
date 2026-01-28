using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the many-to-many relationship between clinics and tags.
    /// Tracks who assigned the tag and when.
    /// </summary>
    public class ClinicTag : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid TagId { get; private set; }
        public string? AssignedBy { get; private set; } // User who assigned (if manual)
        public DateTime AssignedAt { get; private set; }
        public bool IsAutoAssigned { get; private set; } // True if assigned by automation

        // Navigation properties
        public Clinic? Clinic { get; private set; }
        public Tag? Tag { get; private set; }

        private ClinicTag()
        {
            // EF Constructor
        }

        public ClinicTag(Guid clinicId, Guid tagId, string tenantId,
            string? assignedBy = null, bool isAutoAssigned = false)
            : base(tenantId)
        {
            ClinicId = clinicId;
            TagId = tagId;
            AssignedBy = assignedBy;
            AssignedAt = DateTime.UtcNow;
            IsAutoAssigned = isAutoAssigned;
        }
    }
}
