using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Interfaces
{
    public interface IExternalServiceConfigurationRepository : IRepository<ExternalServiceConfiguration>
    {
        /// <summary>
        /// Gets all external service configurations for a tenant
        /// </summary>
        Task<IEnumerable<ExternalServiceConfiguration>> GetAllByTenantAsync(string tenantId);
        
        /// <summary>
        /// Gets external service configuration by service type
        /// </summary>
        Task<ExternalServiceConfiguration?> GetByServiceTypeAsync(
            ExternalServiceType serviceType, 
            string tenantId, 
            Guid? clinicId = null);
        
        /// <summary>
        /// Gets all external service configurations for a specific clinic
        /// </summary>
        Task<IEnumerable<ExternalServiceConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        
        /// <summary>
        /// Gets all active external service configurations
        /// </summary>
        Task<IEnumerable<ExternalServiceConfiguration>> GetActiveServicesAsync(string tenantId);
        
        /// <summary>
        /// Checks if a service type already exists for the tenant/clinic
        /// </summary>
        Task<bool> ExistsByServiceTypeAsync(
            ExternalServiceType serviceType, 
            string tenantId, 
            Guid? clinicId = null,
            Guid? excludeId = null);
    }
}
