using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

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
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Get the is_system_owner claim from the JWT token
            var isSystemOwnerClaim = context.HttpContext.User?.FindFirst("is_system_owner");
            
            // Check if the claim exists and is exactly "true" (as a string)
            var isSystemOwner = isSystemOwnerClaim?.Value;
            
            if (string.IsNullOrEmpty(isSystemOwner) || !string.Equals(isSystemOwner, "true", StringComparison.OrdinalIgnoreCase))
            {
                // User is not a System Owner - deny access
                context.Result = new ObjectResult(new 
                { 
                    message = "Access denied. This endpoint is only accessible to System Owners. " +
                             "Please authenticate via /api/auth/owner-login with tenantId='system'." 
                })
                {
                    StatusCode = 403
                };
            }
        }
    }
}
