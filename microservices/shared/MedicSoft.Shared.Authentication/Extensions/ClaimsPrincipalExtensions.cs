using System.Security.Claims;
using MedicSoft.Shared.Authentication.Constants;
using MedicSoft.Shared.Authentication.Models;

namespace MedicSoft.Shared.Authentication.Extensions;

/// <summary>
/// Extension methods for extracting authenticated user information from ClaimsPrincipal
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Gets the authenticated user from the claims principal
    /// </summary>
    public static AuthenticatedUser? GetAuthenticatedUser(this ClaimsPrincipal principal)
    {
        if (principal?.Identity?.IsAuthenticated != true)
            return null;

        var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;

        var clinicIdClaim = principal.FindFirst(ClaimConstants.ClinicId)?.Value;
        Guid? clinicId = null;
        if (!string.IsNullOrEmpty(clinicIdClaim) && Guid.TryParse(clinicIdClaim, out var parsedClinicId))
            clinicId = parsedClinicId;

        var isSystemOwnerClaim = principal.FindFirst(ClaimConstants.IsSystemOwner)?.Value;
        var isSystemOwner = isSystemOwnerClaim?.ToLower() == "true";

        return new AuthenticatedUser
        {
            UserId = userId,
            Username = principal.FindFirst(ClaimTypes.Name)?.Value ?? string.Empty,
            TenantId = principal.FindFirst(ClaimConstants.TenantId)?.Value ?? string.Empty,
            ClinicId = clinicId,
            Role = principal.FindFirst(ClaimTypes.Role)?.Value ?? string.Empty,
            SessionId = principal.FindFirst(ClaimConstants.SessionId)?.Value,
            IsSystemOwner = isSystemOwner
        };
    }

    /// <summary>
    /// Gets the tenant ID from the claims principal
    /// </summary>
    public static string GetTenantId(this ClaimsPrincipal principal)
    {
        return principal.FindFirst(ClaimConstants.TenantId)?.Value ?? string.Empty;
    }

    /// <summary>
    /// Gets the clinic ID from the claims principal
    /// </summary>
    public static Guid? GetClinicId(this ClaimsPrincipal principal)
    {
        var clinicIdClaim = principal.FindFirst(ClaimConstants.ClinicId)?.Value;
        if (!string.IsNullOrEmpty(clinicIdClaim) && Guid.TryParse(clinicIdClaim, out var clinicId))
            return clinicId;
        return null;
    }

    /// <summary>
    /// Checks if the user is a system owner
    /// </summary>
    public static bool IsSystemOwner(this ClaimsPrincipal principal)
    {
        var isSystemOwnerClaim = principal.FindFirst(ClaimConstants.IsSystemOwner)?.Value;
        return isSystemOwnerClaim?.ToLower() == "true";
    }
}
