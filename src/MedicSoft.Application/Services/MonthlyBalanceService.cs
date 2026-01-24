using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing monthly balance reconciliation of controlled medications.
    /// Implements ANVISA RDC 27/2007 requirements for monthly inventory control.
    /// </summary>
    public class MonthlyBalanceService : IMonthlyBalanceService
    {
        private readonly IMonthlyControlledBalanceRepository _balanceRepository;
        private readonly IControlledMedicationRegistryRepository _registryRepository;
        private readonly ILogger<MonthlyBalanceService> _logger;

        public MonthlyBalanceService(
            IMonthlyControlledBalanceRepository balanceRepository,
            IControlledMedicationRegistryRepository registryRepository,
            ILogger<MonthlyBalanceService> logger)
        {
            _balanceRepository = balanceRepository ?? throw new ArgumentNullException(nameof(balanceRepository));
            _registryRepository = registryRepository ?? throw new ArgumentNullException(nameof(registryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> CalculateMonthlyBalancesAsync(
            int year, 
            int month, 
            string tenantId)
        {
            if (year < 2000 || year > DateTime.UtcNow.Year + 1)
                throw new ArgumentException($"Year must be between 2000 and {DateTime.UtcNow.Year + 1}", nameof(year));

            if (month < 1 || month > 12)
                throw new ArgumentException("Month must be between 1 and 12", nameof(month));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation(
                "Calculating monthly balances for {Year}-{Month} for tenant {TenantId}",
                year, month, tenantId);

            // Get the date range for the month
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            // Get all controlled medications that had activity in this month
            var medications = await _registryRepository.GetControlledMedicationsAsync(tenantId);
            var balances = new List<MonthlyControlledBalance>();

            foreach (var medicationName in medications)
            {
                // Check if balance already exists for this medication/month
                var existingBalance = await _balanceRepository.GetByMonthYearMedicationAsync(
                    year, month, medicationName, tenantId);

                if (existingBalance != null)
                {
                    _logger.LogInformation(
                        "Balance already exists for {MedicationName} in {Year}-{Month}",
                        medicationName, year, month);
                    balances.Add(existingBalance);
                    continue;
                }

                // Get all registry entries for this medication in the month
                var registryEntries = (await _registryRepository.GetByMedicationAsync(medicationName, tenantId))
                    .Where(r => r.Date >= startDate && r.Date <= endDate)
                    .OrderBy(r => r.Date)
                    .ToList();

                // Calculate initial balance (from previous month's closing balance)
                decimal initialBalance = 0;
                var previousMonthBalance = await GetPreviousMonthBalance(year, month, medicationName, tenantId);
                if (previousMonthBalance != null)
                {
                    initialBalance = previousMonthBalance.PhysicalBalance ?? previousMonthBalance.CalculatedFinalBalance;
                }
                else
                {
                    // If no previous month, get balance from registry before this month
                    var entriesBeforeMonth = (await _registryRepository.GetByMedicationAsync(medicationName, tenantId))
                        .Where(r => r.Date < startDate)
                        .OrderByDescending(r => r.Date)
                        .FirstOrDefault();
                    
                    if (entriesBeforeMonth != null)
                    {
                        initialBalance = entriesBeforeMonth.Balance;
                    }
                }

                // Calculate total in and total out
                var totalIn = registryEntries
                    .Where(r => r.RegistryType == RegistryType.Inbound)
                    .Sum(r => r.QuantityIn);

                var totalOut = registryEntries
                    .Where(r => r.RegistryType == RegistryType.Outbound)
                    .Sum(r => r.QuantityOut);

                // Get medication details from first registry entry
                var firstEntry = registryEntries.FirstOrDefault();
                if (firstEntry == null)
                {
                    // No activity this month, but medication exists
                    // Get details from most recent entry
                    firstEntry = (await _registryRepository.GetByMedicationAsync(medicationName, tenantId))
                        .OrderByDescending(r => r.Date)
                        .FirstOrDefault();
                }

                if (firstEntry == null)
                {
                    _logger.LogWarning(
                        "No registry entries found for medication {MedicationName}",
                        medicationName);
                    continue;
                }

                // Create the monthly balance
                var balance = new MonthlyControlledBalance(
                    tenantId: tenantId,
                    year: year,
                    month: month,
                    medicationName: medicationName,
                    activeIngredient: firstEntry.ActiveIngredient,
                    anvisaList: firstEntry.AnvisaList,
                    initialBalance: initialBalance,
                    totalIn: totalIn,
                    totalOut: totalOut
                );

                await _balanceRepository.AddAsync(balance);
                balances.Add(balance);

                _logger.LogInformation(
                    "Created monthly balance for {MedicationName} in {Year}-{Month}: Initial={Initial}, In={In}, Out={Out}, Final={Final}",
                    medicationName, year, month, initialBalance, totalIn, totalOut, balance.CalculatedFinalBalance);
            }

            return balances;
        }

        public async Task<MonthlyControlledBalance> RecordPhysicalInventoryAsync(
            Guid balanceId, 
            decimal physicalCount, 
            string? discrepancyReason, 
            string tenantId, 
            Guid userId)
        {
            if (balanceId == Guid.Empty)
                throw new ArgumentException("Balance ID cannot be empty", nameof(balanceId));

            if (physicalCount < 0)
                throw new ArgumentException("Physical count cannot be negative", nameof(physicalCount));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            _logger.LogInformation(
                "Recording physical inventory for balance {BalanceId}: {PhysicalCount}",
                balanceId, physicalCount);

            var balance = await _balanceRepository.GetByIdAsync(balanceId, tenantId);
            if (balance == null)
            {
                _logger.LogError("Balance {BalanceId} not found", balanceId);
                throw new InvalidOperationException($"Balance {balanceId} not found");
            }

            if (balance.Status == BalanceStatus.Closed)
            {
                _logger.LogWarning("Attempted to update closed balance {BalanceId}", balanceId);
                throw new InvalidOperationException("Cannot update a closed balance");
            }

            balance.RecordPhysicalInventory(physicalCount, discrepancyReason, userId);
            await _balanceRepository.UpdateAsync(balance);

            _logger.LogInformation(
                "Physical inventory recorded for {MedicationName}: Physical={Physical}, Calculated={Calculated}, Discrepancy={Discrepancy}",
                balance.MedicationName, balance.PhysicalBalance, balance.CalculatedFinalBalance, balance.Discrepancy);

            return balance;
        }

        public async Task<MonthlyControlledBalance> CloseBalanceAsync(
            Guid balanceId, 
            string tenantId, 
            Guid userId)
        {
            if (balanceId == Guid.Empty)
                throw new ArgumentException("Balance ID cannot be empty", nameof(balanceId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            _logger.LogInformation(
                "Closing balance {BalanceId} by user {UserId}",
                balanceId, userId);

            var balance = await _balanceRepository.GetByIdAsync(balanceId, tenantId);
            if (balance == null)
            {
                _logger.LogError("Balance {BalanceId} not found", balanceId);
                throw new InvalidOperationException($"Balance {balanceId} not found");
            }

            if (balance.Status == BalanceStatus.Closed)
            {
                _logger.LogWarning("Balance {BalanceId} is already closed", balanceId);
                throw new InvalidOperationException("Balance is already closed");
            }

            balance.Close(userId);
            await _balanceRepository.UpdateAsync(balance);

            _logger.LogInformation(
                "Balance {BalanceId} closed successfully for {MedicationName}",
                balanceId, balance.MedicationName);

            return balance;
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetOverdueBalancesAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Getting overdue balances for tenant {TenantId}", tenantId);

            return await _balanceRepository.GetOverdueBalancesAsync(tenantId);
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetBalancesWithDiscrepanciesAsync(string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            _logger.LogInformation("Getting balances with discrepancies for tenant {TenantId}", tenantId);

            return await _balanceRepository.GetBalancesWithDiscrepanciesAsync(tenantId);
        }

        private async Task<MonthlyControlledBalance?> GetPreviousMonthBalance(
            int year, 
            int month, 
            string medicationName, 
            string tenantId)
        {
            var previousMonth = month - 1;
            var previousYear = year;

            if (previousMonth < 1)
            {
                previousMonth = 12;
                previousYear--;
            }

            return await _balanceRepository.GetByMonthYearMedicationAsync(
                previousYear, previousMonth, medicationName, tenantId);
        }
    }
}
