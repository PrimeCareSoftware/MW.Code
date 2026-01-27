using Hangfire.Dashboard;

namespace MedicSoft.Api.Filters
{
    /// <summary>
    /// Authorization filter for Hangfire Dashboard
    /// In development: allows all access
    /// In production: should be configured with proper authentication
    /// </summary>
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // In development, allow all access
            // In production, implement proper authentication
            // Example: Check if user is Admin role
            
            // For now, allow access only in development
            // TODO: Implement proper authentication for production
            return true; // Allow access (development only)
        }
    }
}
