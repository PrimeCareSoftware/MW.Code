using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing controlled medication registry (Livro de Registro).
    /// Implements ANVISA RDC 27/2007 requirements for tracking controlled substances.
    /// </summary>
    public interface IControlledMedicationRegistryService
    {
        /// <summary>
        /// Automatically registers a controlled medication prescription in the registry.
        /// Called when a controlled prescription is created and finalized.
        /// </summary>
        /// <param name="prescriptionId">ID of the prescription containing controlled medications.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <param name="userId">ID of the user registering the prescription.</param>
        /// <returns>The created registry entry.</returns>
        /// <exception cref="ArgumentException">When prescription ID is invalid.</exception>
        /// <exception cref="InvalidOperationException">When prescription is already registered or not found.</exception>
        Task<ControlledMedicationRegistry> RegisterPrescriptionAsync(
            Guid prescriptionId, 
            string tenantId, 
            Guid userId);

        /// <summary>
        /// Manually registers a stock entry (inbound movement) of controlled medication.
        /// Used for purchases, transfers, or returns.
        /// </summary>
        /// <param name="dto">Stock entry details.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <param name="userId">ID of the user performing the registration.</param>
        /// <returns>The created registry entry.</returns>
        /// <exception cref="ArgumentNullException">When dto is null.</exception>
        /// <exception cref="ArgumentException">When dto contains invalid data.</exception>
        Task<ControlledMedicationRegistry> RegisterStockEntryAsync(
            StockEntryDto dto, 
            string tenantId, 
            Guid userId);

        /// <summary>
        /// Gets all registry entries within a specified date range.
        /// </summary>
        /// <param name="startDate">Start date of the period (inclusive).</param>
        /// <param name="endDate">End date of the period (inclusive).</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of registry entries in the specified period.</returns>
        Task<IEnumerable<ControlledMedicationRegistry>> GetRegistryByPeriodAsync(
            DateTime startDate, 
            DateTime endDate, 
            string tenantId);

        /// <summary>
        /// Gets all registry entries for a specific medication.
        /// </summary>
        /// <param name="medicationName">Name of the controlled medication.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of registry entries for the medication.</returns>
        /// <exception cref="ArgumentException">When medication name is null or empty.</exception>
        Task<IEnumerable<ControlledMedicationRegistry>> GetRegistryByMedicationAsync(
            string medicationName, 
            string tenantId);

        /// <summary>
        /// Calculates the current balance for a specific controlled medication.
        /// </summary>
        /// <param name="medicationName">Name of the controlled medication.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Current available balance.</returns>
        /// <exception cref="ArgumentException">When medication name is null or empty.</exception>
        Task<decimal> GetCurrentBalanceAsync(
            string medicationName, 
            string tenantId);
    }
}
