using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MedicSoft.Api.Filters
{
    /// <summary>
    /// Swagger operation filter that removes the security requirement from endpoints with [AllowAnonymous]
    /// This ensures that swagger.json is accessible without authentication and properly documents
    /// which endpoints require authentication.
    /// </summary>
    public class AuthorizeCheckOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Check if the endpoint has [AllowAnonymous] attribute
            var hasAllowAnonymous = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AllowAnonymousAttribute>()
                .Any() ?? false;

            if (!hasAllowAnonymous)
            {
                hasAllowAnonymous = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any();
            }

            // If the endpoint has [AllowAnonymous], remove security requirements
            if (hasAllowAnonymous)
            {
                operation.Security?.Clear();
                return;
            }

            // Check if the endpoint has [Authorize] attribute
            var hasAuthorize = context.MethodInfo.DeclaringType?.GetCustomAttributes(true)
                .OfType<AuthorizeAttribute>()
                .Any() ?? false;

            if (!hasAuthorize)
            {
                hasAuthorize = context.MethodInfo.GetCustomAttributes(true)
                    .OfType<AuthorizeAttribute>()
                    .Any();
            }

            // If no [Authorize] and no [AllowAnonymous], endpoint doesn't require auth explicitly
            // In this case, remove security requirements since we're not enforcing global auth
            if (!hasAuthorize)
            {
                operation.Security?.Clear();
            }
        }
    }
}
