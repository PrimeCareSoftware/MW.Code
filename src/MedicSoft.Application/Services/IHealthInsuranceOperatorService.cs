using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing health insurance operators
    /// </summary>
    public interface IHealthInsuranceOperatorService
    {
        /// <summary>
        /// Creates a new health insurance operator
        /// </summary>
        Task<HealthInsuranceOperatorDto> CreateAsync(CreateHealthInsuranceOperatorDto dto, string tenantId);

        /// <summary>
        /// Updates an existing health insurance operator
        /// </summary>
        Task<HealthInsuranceOperatorDto> UpdateAsync(Guid id, UpdateHealthInsuranceOperatorDto dto, string tenantId);

        /// <summary>
        /// Configures operator integration settings
        /// </summary>
        Task<HealthInsuranceOperatorDto> ConfigureIntegrationAsync(Guid id, ConfigureOperatorIntegrationDto dto, string tenantId);

        /// <summary>
        /// Configures TISS settings for the operator
        /// </summary>
        Task<HealthInsuranceOperatorDto> ConfigureTissAsync(Guid id, ConfigureOperatorTissDto dto, string tenantId);

        /// <summary>
        /// Gets all health insurance operators for a tenant
        /// </summary>
        Task<IEnumerable<HealthInsuranceOperatorDto>> GetAllAsync(string tenantId, bool includeInactive = false);

        /// <summary>
        /// Gets a health insurance operator by ID
        /// </summary>
        Task<HealthInsuranceOperatorDto?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Gets a health insurance operator by ANS register number
        /// </summary>
        Task<HealthInsuranceOperatorDto?> GetByRegisterNumberAsync(string registerNumber, string tenantId);

        /// <summary>
        /// Searches operators by name
        /// </summary>
        Task<IEnumerable<HealthInsuranceOperatorDto>> SearchByNameAsync(string name, string tenantId);

        /// <summary>
        /// Activates an operator
        /// </summary>
        Task<HealthInsuranceOperatorDto> ActivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deactivates an operator
        /// </summary>
        Task<HealthInsuranceOperatorDto> DeactivateAsync(Guid id, string tenantId);

        /// <summary>
        /// Deletes an operator (soft delete by deactivating)
        /// </summary>
        Task<bool> DeleteAsync(Guid id, string tenantId);
    }
}
