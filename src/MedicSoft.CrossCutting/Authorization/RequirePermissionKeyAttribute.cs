using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Common;
using System.Security.Claims;

namespace MedicSoft.CrossCutting.Authorization
{
    /// <summary>
    /// Authorization attribute that checks if the user has the required permission key
    /// based on their assigned access profile. This provides granular permission control
    /// and supports custom profiles created by clinic owners.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionKeyAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _requiredPermissionKey;

        public RequirePermissionKeyAttribute(string requiredPermissionKey)
        {
            _requiredPermissionKey = requiredPermissionKey;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Check if user is authenticated
            if (context.HttpContext.User.Identity?.IsAuthenticated != true)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "Authentication required",
                    code = "UNAUTHORIZED"
                });
                return;
            }

            // Get user ID from claims
            var userIdClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "Invalid user token",
                    code = "INVALID_TOKEN"
                });
                return;
            }

            // Get tenant ID from claims
            var tenantId = context.HttpContext.User.FindFirst("tenant_id")?.Value;
            if (string.IsNullOrEmpty(tenantId))
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "Invalid tenant context",
                    code = "INVALID_TENANT"
                });
                return;
            }

            // Get role from claims to determine if this is a system admin, owner or user
            var roleClaim = context.HttpContext.User.FindFirst(ClaimTypes.Role)?.Value;
            
            // If role is "SystemAdmin", grant full access
            // SystemAdmin users have full access to all system functionalities
            if (roleClaim == RoleNames.SystemAdmin)
            {
                return;
            }
            
            // If role is "ClinicOwner" or "Owner" (for backwards compatibility), check Owner permissions
            // Owners have full access to all features
            if (roleClaim == RoleNames.ClinicOwner || roleClaim == "Owner")
            {
                // Get owner repository from DI
                var ownerRepository = context.HttpContext.RequestServices.GetRequiredService<IOwnerRepository>();
                
                // Get owner
                var owner = await ownerRepository.GetByIdAsync(userId, tenantId);
                if (owner == null)
                {
                    context.Result = new UnauthorizedObjectResult(new 
                    { 
                        message = "Owner not found",
                        code = "OWNER_NOT_FOUND"
                    });
                    return;
                }

                // Check if owner is active
                if (!owner.IsActive)
                {
                    context.Result = new ForbidResult();
                    return;
                }

                // Owners have all permissions by default
                // They can manage users, profiles, and all clinic settings
                return;
            }

            // For regular users, check their profile-based permissions
            // Get user repository from DI
            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

            // Get user with profile
            var user = await userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
            {
                context.Result = new UnauthorizedObjectResult(new 
                { 
                    message = "User not found",
                    code = "USER_NOT_FOUND"
                });
                return;
            }

            // Check if user is active
            if (!user.IsActive)
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if user has the required permission
            if (!user.HasPermissionKey(_requiredPermissionKey))
            {
                context.Result = new ObjectResult(new 
                { 
                    message = $"You don't have permission to perform this action. Required permission: {_requiredPermissionKey}",
                    code = "FORBIDDEN",
                    requiredPermission = _requiredPermissionKey
                })
                {
                    StatusCode = 403
                };
                return;
            }
        }
    }
}
