using System;

namespace MedicSoft.Application.DTOs
{
    public class CashFlowEntryDto
    {
        public Guid Id { get; set; }
        public string Type { get; set; } = string.Empty; // Income or Expense
        public string Category { get; set; } = string.Empty;
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public string? Notes { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReceivableId { get; set; }
        public Guid? PayableId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? BankAccount { get; set; }
        public string? PaymentMethod { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCashFlowEntryDto
    {
        public int Type { get; set; } // CashFlowType enum value
        public int Category { get; set; } // CashFlowCategory enum value
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Reference { get; set; }
        public Guid? PaymentId { get; set; }
        public Guid? ReceivableId { get; set; }
        public Guid? PayableId { get; set; }
        public Guid? AppointmentId { get; set; }
        public string? BankAccount { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class UpdateCashFlowEntryDto
    {
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? BankAccount { get; set; }
        public string? PaymentMethod { get; set; }
    }

    public class CashFlowSummaryDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal NetCashFlow { get; set; }
        public decimal OpeningBalance { get; set; }
        public decimal ClosingBalance { get; set; }
    }
}
