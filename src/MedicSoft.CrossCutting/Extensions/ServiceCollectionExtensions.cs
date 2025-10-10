using Microsoft.Extensions.DependencyInjection;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.CrossCutting.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMedicSoftCrossCutting(this IServiceCollection services)
        {
            // Register tenant context as scoped to ensure it's available per request
            services.AddScoped<ITenantContext, TenantContext>();

            // Register security services
            services.AddSingleton<IPasswordHasher, PasswordHasher>();

            return services;
        }
    }
}