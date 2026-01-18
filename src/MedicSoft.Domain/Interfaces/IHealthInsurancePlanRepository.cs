using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for Health Insurance Plans
    /// </summary>
    public interface IHealthInsurancePlanRepository : IRepository<HealthInsurancePlan>
    {
        /// <summary>
        /// Gets all plans for a specific operator
        /// </summary>
        Task<IEnumerable<HealthInsurancePlan>> GetByOperatorIdAsync(Guid operatorId, string tenantId);

        /// <summary>
        /// Gets a plan by its code
        /// </summary>
        Task<HealthInsurancePlan?> GetByPlanCodeAsync(string planCode, string tenantId);

        /// <summary>
        /// Gets active plans only
        /// </summary>
        Task<IEnumerable<HealthInsurancePlan>> GetActiveAsync(string tenantId);
    }
}
