using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMonthlyControlledBalanceRepository : IRepository<MonthlyControlledBalance>
    {
        /// <summary>
        /// Gets the balance for a specific medication in a specific month/year.
        /// </summary>
        Task<MonthlyControlledBalance?> GetByMonthYearMedicationAsync(
            int year,
            int month,
            string medicationName,
            string tenantId);

        /// <summary>
        /// Gets all balances for a specific month/year.
        /// </summary>
        Task<IEnumerable<MonthlyControlledBalance>> GetByMonthYearAsync(
            int year,
            int month,
            string tenantId);

        /// <summary>
        /// Gets all open (unclosed) balances.
        /// </summary>
        Task<IEnumerable<MonthlyControlledBalance>> GetOpenBalancesAsync(string tenantId);

        /// <summary>
        /// Gets balances with discrepancies.
        /// </summary>
        Task<IEnumerable<MonthlyControlledBalance>> GetBalancesWithDiscrepanciesAsync(string tenantId);

        /// <summary>
        /// Gets overdue balances (should be closed by 5th of following month).
        /// </summary>
        Task<IEnumerable<MonthlyControlledBalance>> GetOverdueBalancesAsync(string tenantId);

        /// <summary>
        /// Gets balance history for a specific medication.
        /// </summary>
        Task<IEnumerable<MonthlyControlledBalance>> GetBalanceHistoryAsync(
            string medicationName,
            int year,
            string tenantId);

        /// <summary>
        /// Checks if a balance already exists for a medication in a specific month/year.
        /// </summary>
        Task<bool> BalanceExistsAsync(
            int year,
            int month,
            string medicationName,
            string tenantId);

        /// <summary>
        /// Gets the most recent closed balance for a medication (for initial balance calculation).
        /// </summary>
        Task<MonthlyControlledBalance?> GetMostRecentClosedBalanceAsync(
            string medicationName,
            string tenantId);
    }
}
