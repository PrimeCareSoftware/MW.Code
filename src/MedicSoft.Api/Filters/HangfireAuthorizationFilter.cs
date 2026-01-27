using Hangfire.Dashboard;
using Microsoft.AspNetCore.Http;

namespace MedicSoft.Api.Filters
{
    /// <summary>
    /// Authorization filter for Hangfire Dashboard
    /// Requires authenticated user with Admin or Owner role
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            
            // Check if user is authenticated
            if (httpContext.User.Identity?.IsAuthenticated != true)
            {
                return false;
            }
            
            // Check if user has Admin or Owner role
            return httpContext.User.IsInRole("Admin") || 
                   httpContext.User.IsInRole("Owner");
        }
    }
}
