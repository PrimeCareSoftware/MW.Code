using Microsoft.AspNetCore.Mvc;

namespace PatientPortal.Api.Controllers;

/// <summary>
/// Base controller providing common functionality for all API controllers
/// </summary>
public abstract class BaseController : ControllerBase
{
    /// <summary>
    /// Extracts the user ID from the authenticated user's claims
    /// </summary>
    /// <returns>User ID as Guid, or null if not found or invalid</returns>
    protected Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }

    /// <summary>
    /// Extracts the tenant ID from the authenticated user's claims
    /// </summary>
    /// <returns>Tenant ID as string, or default tenant if not found</returns>
    protected string GetTenantId()
    {
        // Extract tenant ID from JWT claims
        var tenantClaim = User.FindFirst("TenantId")?.Value;
        if (!string.IsNullOrEmpty(tenantClaim))
            return tenantClaim;

        // Fallback to default tenant
        // TODO: Implement proper tenant resolution strategy
        return "default-tenant";
    }
}
