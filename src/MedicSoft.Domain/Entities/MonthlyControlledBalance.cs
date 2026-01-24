using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the monthly balance reconciliation for controlled medications.
    /// Required by ANVISA RDC 27/2007 for monthly reporting and stock control.
    /// </summary>
    public class MonthlyControlledBalance : BaseEntity
    {
        public int Year { get; private set; }
        public int Month { get; private set; }
        
        // Medication Identification
        public string MedicationName { get; private set; }
        public string ActiveIngredient { get; private set; }
        public string AnvisaList { get; private set; } // A1, A2, A3, B1, B2, C1, C2, C3, C4, C5
        
        // Balance Calculation
        public decimal InitialBalance { get; private set; }
        public decimal TotalIn { get; private set; }
        public decimal TotalOut { get; private set; }
        public decimal CalculatedFinalBalance { get; private set; } // Initial + In - Out
        
        // Physical Inventory
        public decimal? PhysicalBalance { get; private set; } // Actual counted quantity
        public decimal? Discrepancy { get; private set; } // Physical - Calculated
        public string? DiscrepancyReason { get; private set; }
        
        // Status and Audit
        public BalanceStatus Status { get; private set; }
        public DateTime? ClosedAt { get; private set; }
        public Guid? ClosedByUserId { get; private set; }
        
        // Navigation Properties
        public User? ClosedBy { get; private set; }

        private MonthlyControlledBalance()
        {
            // EF Constructor
            MedicationName = string.Empty;
            ActiveIngredient = string.Empty;
            AnvisaList = string.Empty;
        }

        public MonthlyControlledBalance(
            string tenantId,
            int year,
            int month,
            string medicationName,
            string activeIngredient,
            string anvisaList,
            decimal initialBalance,
            decimal totalIn,
            decimal totalOut) : base(tenantId)
        {
            if (year < 2000 || year > DateTime.UtcNow.Year)
                throw new ArgumentException("Invalid year", nameof(year));
            
            if (month < 1 || month > 12)
                throw new ArgumentException("Month must be between 1 and 12", nameof(month));
            
            if (string.IsNullOrWhiteSpace(medicationName))
                throw new ArgumentException("Medication name cannot be empty", nameof(medicationName));
            
            if (string.IsNullOrWhiteSpace(activeIngredient))
                throw new ArgumentException("Active ingredient cannot be empty", nameof(activeIngredient));
            
            if (string.IsNullOrWhiteSpace(anvisaList))
                throw new ArgumentException("ANVISA list cannot be empty", nameof(anvisaList));
            
            if (initialBalance < 0)
                throw new ArgumentException("Initial balance cannot be negative", nameof(initialBalance));
            
            if (totalIn < 0)
                throw new ArgumentException("Total in cannot be negative", nameof(totalIn));
            
            if (totalOut < 0)
                throw new ArgumentException("Total out cannot be negative", nameof(totalOut));

            Year = year;
            Month = month;
            MedicationName = medicationName.Trim();
            ActiveIngredient = activeIngredient.Trim();
            AnvisaList = anvisaList.Trim().ToUpperInvariant();
            InitialBalance = initialBalance;
            TotalIn = totalIn;
            TotalOut = totalOut;
            CalculatedFinalBalance = initialBalance + totalIn - totalOut;
            Status = BalanceStatus.Open;
        }

        public void RecordPhysicalInventory(
            decimal physicalBalance,
            string? discrepancyReason,
            Guid closedByUserId)
        {
            if (physicalBalance < 0)
                throw new ArgumentException("Physical balance cannot be negative", nameof(physicalBalance));
            
            if (Status == BalanceStatus.Closed)
                throw new InvalidOperationException("Cannot update a closed balance");

            PhysicalBalance = physicalBalance;
            Discrepancy = physicalBalance - CalculatedFinalBalance;
            
            // If there's a discrepancy, reason is required
            if (Math.Abs(Discrepancy.Value) > 0.001m)
            {
                if (string.IsNullOrWhiteSpace(discrepancyReason))
                    throw new ArgumentException("Discrepancy reason is required when physical and calculated balances differ", nameof(discrepancyReason));
                
                DiscrepancyReason = discrepancyReason.Trim();
            }
            else
            {
                DiscrepancyReason = null;
            }
            
            UpdateTimestamp();
        }

        public void Close(Guid closedByUserId)
        {
            if (Status == BalanceStatus.Closed)
                throw new InvalidOperationException("Balance is already closed");
            
            // Physical inventory is optional but recommended
            if (!PhysicalBalance.HasValue)
            {
                // If no physical count, assume calculated balance is correct
                PhysicalBalance = CalculatedFinalBalance;
                Discrepancy = 0;
            }
            
            Status = BalanceStatus.Closed;
            ClosedAt = DateTime.UtcNow;
            ClosedByUserId = closedByUserId;
            UpdateTimestamp();
        }

        public void Reopen()
        {
            if (Status != BalanceStatus.Closed)
                throw new InvalidOperationException("Can only reopen a closed balance");
            
            Status = BalanceStatus.Open;
            ClosedAt = null;
            ClosedByUserId = null;
            UpdateTimestamp();
        }

        public bool HasDiscrepancy()
        {
            return Discrepancy.HasValue && Math.Abs(Discrepancy.Value) > 0.001m;
        }

        public bool IsOverdue()
        {
            // Balance should be closed by the 5th day of the following month
            var deadline = new DateTime(Month == 12 ? Year + 1 : Year, Month == 12 ? 1 : Month + 1, 5);
            return DateTime.UtcNow > deadline && Status == BalanceStatus.Open;
        }
    }

    /// <summary>
    /// Status of monthly balance
    /// </summary>
    public enum BalanceStatus
    {
        /// <summary>
        /// Balance is being calculated and can be modified
        /// </summary>
        Open = 1,

        /// <summary>
        /// Balance has been closed and locked
        /// </summary>
        Closed = 2
    }
}
