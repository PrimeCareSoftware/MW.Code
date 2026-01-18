using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing health insurance plans
    /// </summary>
    public interface IHealthInsurancePlanService
    {
        /// <summary>
        /// Creates a new health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> CreateAsync(CreateHealthInsurancePlanDto dto, string tenantId);

        /// <summary>
        /// Updates an existing health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> UpdateAsync(Guid id, UpdateHealthInsurancePlanDto dto, string tenantId);

        /// <summary>
        /// Gets all health insurance plans for a tenant
        /// </summary>
        Task<IEnumerable<HealthInsurancePlanDto>> GetAllAsync(string tenantId, bool includeInactive = false);

        /// <summary>
        /// Gets a health insurance plan by ID
        /// </summary>
        Task<HealthInsurancePlanDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets health insurance plans by operator ID
        /// </summary>
        Task<IEnumerable<HealthInsurancePlanDto>> GetByOperatorIdAsync(Guid operatorId, string tenantId);

        /// <summary>
        /// Gets a health insurance plan by plan code
        /// </summary>
        Task<HealthInsurancePlanDto?> GetByPlanCodeAsync(string planCode, string tenantId);

        /// <summary>
        /// Enables TISS for a health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> EnableTissAsync(Guid id, string tenantId);

        /// <summary>
        /// Disables TISS for a health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> DisableTissAsync(Guid id, string tenantId);

        /// <summary>
        /// Activates a health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> ActivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deactivates a health insurance plan
        /// </summary>
        Task<HealthInsurancePlanDto> DeactivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deletes a health insurance plan (soft delete by deactivating)
        /// </summary>
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
