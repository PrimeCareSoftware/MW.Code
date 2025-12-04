namespace MedicSoft.Shared.Authentication.Constants;

/// <summary>
/// Defines constant values for JWT claim types used across microservices
/// </summary>
public static class ClaimConstants
{
    public const string TenantId = "tenant_id";
    public const string ClinicId = "clinic_id";
    public const string SessionId = "session_id";
    public const string IsSystemOwner = "is_system_owner";
    public const string UserRole = "role";
}
