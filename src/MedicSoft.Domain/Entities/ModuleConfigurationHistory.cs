using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Tracks changes to module configurations for audit purposes
    /// </summary>
    public class ModuleConfigurationHistory : BaseEntity
    {
        public Guid ModuleConfigurationId { get; private set; }
        public Guid ClinicId { get; private set; }
        public string ModuleName { get; private set; }
        public string Action { get; private set; } // "Enabled", "Disabled", "ConfigUpdated"
        public string? PreviousConfiguration { get; private set; }
        public string? NewConfiguration { get; private set; }
        public string ChangedBy { get; private set; } // User ID or "System"
        public DateTime ChangedAt { get; private set; }
        public string? Reason { get; private set; } // Optional reason for the change

        // Navigation properties
        public ModuleConfiguration? ModuleConfiguration { get; private set; }

        private ModuleConfigurationHistory()
        {
            // EF Constructor
            ModuleName = null!;
            Action = null!;
            ChangedBy = null!;
        }

        public ModuleConfigurationHistory(
            Guid moduleConfigurationId,
            Guid clinicId,
            string moduleName,
            string action,
            string changedBy,
            string tenantId,
            string? previousConfig = null,
            string? newConfig = null,
            string? reason = null) : base(tenantId)
        {
            if (moduleConfigurationId == Guid.Empty)
                throw new ArgumentException("Module configuration ID cannot be empty", nameof(moduleConfigurationId));

            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (string.IsNullOrWhiteSpace(moduleName))
                throw new ArgumentException("Module name cannot be empty", nameof(moduleName));

            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty", nameof(action));

            if (string.IsNullOrWhiteSpace(changedBy))
                throw new ArgumentException("Changed by cannot be empty", nameof(changedBy));

            ModuleConfigurationId = moduleConfigurationId;
            ClinicId = clinicId;
            ModuleName = moduleName.Trim();
            Action = action.Trim();
            ChangedBy = changedBy.Trim();
            ChangedAt = DateTime.UtcNow;
            PreviousConfiguration = previousConfig;
            NewConfiguration = newConfig;
            Reason = reason;
        }
    }
}
