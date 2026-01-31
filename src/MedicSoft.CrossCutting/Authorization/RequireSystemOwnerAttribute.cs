using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace MedicSoft.CrossCutting.Authorization
{
    /// <summary>
    /// Authorization attribute that requires the user to be a System Owner.
    /// System Owners are identified by the 'is_system_owner' claim in their JWT token.
    /// This attribute should be used in addition to role-based authorization to ensure
    /// only platform administrators can access system-wide resources.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
    public class RequireSystemOwnerAttribute : TypeFilterAttribute
    {
        public RequireSystemOwnerAttribute() : base(typeof(RequireSystemOwnerFilter))
        {
        }
    }

    public class RequireSystemOwnerFilter : IAuthorizationFilter
    {
        private readonly ILogger<RequireSystemOwnerFilter> _logger;

        public RequireSystemOwnerFilter(ILogger<RequireSystemOwnerFilter> logger)
        {
            _logger = logger;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get the is_system_owner claim from the JWT token
            var isSystemOwnerClaim = context.HttpContext.User?.FindFirst(CustomClaimTypes.IsSystemOwner);
            
            // Check if the claim exists and is exactly "true" (case-insensitive)
            var isSystemOwner = isSystemOwnerClaim?.Value;
            
            if (string.IsNullOrEmpty(isSystemOwner) || !string.Equals(isSystemOwner, "true", StringComparison.OrdinalIgnoreCase))
            {
                // Log the denial for security monitoring
                var username = context.HttpContext.User?.Identity?.Name ?? "unknown";
                var role = context.HttpContext.User?.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? "unknown";
                
                _logger.LogWarning(
                    "Access denied to System Owner-only resource. User: {Username}, Role: {Role}, IsSystemOwner claim: {IsSystemOwner}",
                    username, role, isSystemOwner ?? "missing");
                
                // User is not a System Owner - deny access with generic message
                context.Result = new ObjectResult(new 
                { 
                    message = "Access denied. Insufficient permissions to access this resource."
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
