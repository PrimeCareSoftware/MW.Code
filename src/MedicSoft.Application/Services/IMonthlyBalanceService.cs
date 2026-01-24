using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing monthly balance reconciliation of controlled medications.
    /// Implements ANVISA RDC 27/2007 requirements for monthly inventory control.
    /// </summary>
    public interface IMonthlyBalanceService
    {
        /// <summary>
        /// Calculates monthly balances for ALL controlled medications in a given month.
        /// Creates balance records with initial balance, total in, total out, and calculated final balance.
        /// </summary>
        /// <param name="year">Year of the balance period.</param>
        /// <param name="month">Month of the balance period (1-12).</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of monthly balance records for all controlled medications.</returns>
        /// <exception cref="ArgumentException">When year or month is invalid.</exception>
        Task<IEnumerable<MonthlyControlledBalance>> CalculateMonthlyBalancesAsync(
            int year, 
            int month, 
            string tenantId);

        /// <summary>
        /// Records the physical inventory count for a balance.
        /// Calculates and records any discrepancies between physical and calculated balances.
        /// </summary>
        /// <param name="balanceId">ID of the monthly balance record.</param>
        /// <param name="physicalCount">Actual counted quantity.</param>
        /// <param name="discrepancyReason">Reason for discrepancy (required if discrepancy exists).</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <param name="userId">ID of the user recording the inventory.</param>
        /// <returns>Updated monthly balance record.</returns>
        /// <exception cref="ArgumentException">When balance ID is invalid or physical count is negative.</exception>
        /// <exception cref="InvalidOperationException">When balance is already closed.</exception>
        Task<MonthlyControlledBalance> RecordPhysicalInventoryAsync(
            Guid balanceId, 
            decimal physicalCount, 
            string? discrepancyReason, 
            string tenantId, 
            Guid userId);

        /// <summary>
        /// Closes a monthly balance, locking it from further modifications.
        /// If no physical inventory was recorded, assumes calculated balance is correct.
        /// </summary>
        /// <param name="balanceId">ID of the monthly balance record.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <param name="userId">ID of the user closing the balance.</param>
        /// <returns>Closed monthly balance record.</returns>
        /// <exception cref="ArgumentException">When balance ID is invalid.</exception>
        /// <exception cref="InvalidOperationException">When balance is already closed or not found.</exception>
        Task<MonthlyControlledBalance> CloseBalanceAsync(
            Guid balanceId, 
            string tenantId, 
            Guid userId);

        /// <summary>
        /// Gets all balances that are overdue for closure.
        /// Balances should be closed by the 5th day of the following month.
        /// </summary>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of overdue balance records.</returns>
        Task<IEnumerable<MonthlyControlledBalance>> GetOverdueBalancesAsync(string tenantId);

        /// <summary>
        /// Gets all balances that have discrepancies between physical and calculated counts.
        /// </summary>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of balance records with discrepancies.</returns>
        Task<IEnumerable<MonthlyControlledBalance>> GetBalancesWithDiscrepanciesAsync(string tenantId);
    }
}
