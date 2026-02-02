using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IBusinessConfigurationRepository : IRepository<BusinessConfiguration>
    {
        /// <summary>
        /// Gets the business configuration for a specific clinic
        /// </summary>
        Task<BusinessConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId);
        
        /// <summary>
        /// Checks if a business configuration exists for a clinic
        /// </summary>
        Task<bool> ExistsByClinicIdAsync(Guid clinicId, string tenantId);
    }
}
