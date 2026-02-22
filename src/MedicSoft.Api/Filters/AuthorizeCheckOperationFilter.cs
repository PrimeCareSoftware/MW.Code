using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MedicSoft.Api.Filters
{
    /// <summary>
    /// Swagger operation filter that removes the security requirement from endpoints with [AllowAnonymous]
    /// This ensures that swagger.json is accessible without authentication and properly documents
    /// which endpoints require authentication.
    /// 
    /// Optimized version using caching to avoid expensive reflection operations during Swagger generation.
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        // Cache to store authorization status per method to avoid repeated reflection
        private static readonly Dictionary<System.Reflection.MethodInfo, bool> AllowAnonymousCache = new();
        private static readonly Dictionary<System.Type, bool> AuthorizeByControllerCache = new();

        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check for [AllowAnonymous] on the method with caching
            if (!AllowAnonymousCache.TryGetValue(context.MethodInfo, out var hasAllowAnonymous))
            {
                hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
                
                // If not found on method, check on controller
                if (!hasAllowAnonymous && context.MethodInfo.DeclaringType != null)
                {
                    hasAllowAnonymous = context.MethodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<AllowAnonymousAttribute>()
                        .Any();
                }
                
                AllowAnonymousCache[context.MethodInfo] = hasAllowAnonymous;
            }

            // If the endpoint has [AllowAnonymous], remove security requirements
            if (hasAllowAnonymous)
            {
                operation.Security?.Clear();
                return;
            }

            // Check for [Authorize] on the controller with caching
            if (context.MethodInfo.DeclaringType != null)
            {
                if (!AuthorizeByControllerCache.TryGetValue(context.MethodInfo.DeclaringType, out var hasControllerAuthorize))
                {
                    hasControllerAuthorize = context.MethodInfo.DeclaringType
                        .GetCustomAttributes(true)
                        .OfType<AuthorizeAttribute>()
                        .Any();
                    
                    AuthorizeByControllerCache[context.MethodInfo.DeclaringType] = hasControllerAuthorize;
                }

                // If controller has [Authorize], keep security requirements
                // Otherwise, remove security requirements since endpoint doesn't require auth explicitly
                if (!hasControllerAuthorize)
                {
                    operation.Security?.Clear();
                }
            }
        }
    }
}
