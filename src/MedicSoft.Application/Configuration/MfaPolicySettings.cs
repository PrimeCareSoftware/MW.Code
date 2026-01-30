using System.Collections.Generic;

namespace MedicSoft.Application.Configuration
{
    /// <summary>
    /// Configuration for MFA enforcement policy
    /// </summary>
    public class MfaPolicySettings
    {
        public bool EnforcementEnabled { get; set; } = true;
        public List<string> RequiredForRoles { get; set; } = new() { "SystemAdmin", "ClinicOwner" };
        public int GracePeriodDays { get; set; } = 7;
        public bool AllowBypass { get; set; } = false;
    }
}
