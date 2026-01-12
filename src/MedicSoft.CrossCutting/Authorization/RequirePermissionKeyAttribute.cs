using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MedicSoft.Domain.Interfaces;
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
            if (!context.HttpContext.User.Identity?.IsAuthenticated ?? true)
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

            // Get user repository from DI
            var userRepository = context.HttpContext.RequestServices.GetService(typeof(IUserRepository)) as IUserRepository;
            if (userRepository == null)
            {
                context.Result = new StatusCodeResult(500);
                return;
            }

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

            await Task.CompletedTask;
        }
    }
}
