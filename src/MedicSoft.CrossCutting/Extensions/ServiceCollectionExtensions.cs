using Microsoft.Extensions.DependencyInjection;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.CrossCutting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMedicSoftCrossCutting(this IServiceCollection services)
        {
            // Register tenant context as scoped to ensure it's available per request
            services.AddScoped<ITenantContext, TenantContext>();

            return services;
        }
    }
}