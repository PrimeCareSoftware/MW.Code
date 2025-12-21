using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Shared.Authentication.Constants;
using MedicSoft.Shared.Authentication.Extensions;
using MedicSoft.Shared.Authentication.Models;

namespace MedicSoft.Shared.Authentication;

/// <summary>
/// Base controller for all microservice API controllers
/// Provides common functionality for authentication and tenant resolution
/// </summary>
[ApiController]
[Authorize]
public abstract class MicroserviceBaseController : ControllerBase
{
    /// <summary>
    /// Gets the current authenticated user from the JWT token
    /// </summary>
    protected AuthenticatedUser? CurrentUser => User.GetAuthenticatedUser();

    /// <summary>
    /// Gets the tenant ID from the current user's JWT token
    /// </summary>
    protected string GetTenantId()
    {
        var tenantId = User.GetTenantId();
        if (string.IsNullOrEmpty(tenantId))
        {
            // Fallback to header for backward compatibility
            tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
        }
        return !string.IsNullOrEmpty(tenantId) ? tenantId : "default-tenant";
    }

    /// <summary>
    /// Gets the clinic ID from the current user's JWT token
    /// </summary>
    protected Guid? GetClinicId()
    {
        return User.GetClinicId();
    }

    /// <summary>
    /// Gets the user ID from the current user's JWT token
    /// </summary>
    protected Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
            return userId;
        return null;
    }

    /// <summary>
    /// Gets the username from the current user's JWT token
    /// </summary>
    protected string? GetUsername()
    {
        return User.Identity?.Name;
    }

    /// <summary>
    /// Gets the user's email from the JWT token
    /// </summary>
    protected string? GetUserEmail()
    {
        return User.FindFirst(ClaimTypes.Email)?.Value ?? User.FindFirst("email")?.Value;
    }

    /// <summary>
    /// Checks if the current user is a system owner
    /// </summary>
    protected bool IsSystemOwner()
    {
        return User.IsSystemOwner();
    }

    /// <summary>
    /// Gets the user's role from the JWT token
    /// </summary>
    protected string? GetUserRole()
    {
        return User.FindFirst(ClaimTypes.Role)?.Value;
    }
}
