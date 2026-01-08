namespace MedicSoft.Domain.Common
{
    /// <summary>
    /// System-wide tenant identifiers and constants
    /// </summary>
    public static class TenantConstants
    {
        /// <summary>
        /// Tenant ID for system-wide data that is not tied to a specific clinic
        /// Used for: subscription plans, sales funnel metrics, system configurations
        /// </summary>
        public const string SystemTenantId = "system";
    }
}
