using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Financial summary showing revenue, expenses, and profit
    /// </summary>
    public class FinancialSummaryDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalPatients { get; set; }
        public decimal AverageAppointmentValue { get; set; }
        public List<RevenueByMethodDto> RevenueByPaymentMethod { get; set; } = new();
        public List<ExpenseByCategoryDto> ExpensesByCategory { get; set; } = new();
    }

    /// <summary>
    /// Revenue breakdown by payment method
    /// </summary>
    public class RevenueByMethodDto
    {
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Expenses breakdown by category
    /// </summary>
    public class ExpenseByCategoryDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Detailed revenue report with daily breakdown
    /// </summary>
    public class RevenueReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalTransactions { get; set; }
        public List<DailyRevenueDto> DailyBreakdown { get; set; } = new();
    }

    /// <summary>
    /// Daily revenue data
    /// </summary>
    public class DailyRevenueDto
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int Transactions { get; set; }
    }

    /// <summary>
    /// Appointments report with statistics
    /// </summary>
    public class AppointmentsReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal CompletionRate { get; set; }
        public decimal CancellationRate { get; set; }
        public List<AppointmentsByStatusDto> AppointmentsByStatus { get; set; } = new();
        public List<AppointmentsByTypeDto> AppointmentsByType { get; set; } = new();
    }

    /// <summary>
    /// Appointments breakdown by status
    /// </summary>
    public class AppointmentsByStatusDto
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Appointments breakdown by type
    /// </summary>
    public class AppointmentsByTypeDto
    {
        public string Type { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Patients growth report
    /// </summary>
    public class PatientsReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public int TotalPatients { get; set; }
        public int NewPatients { get; set; }
        public int ActivePatients { get; set; }
        public List<MonthlyPatientsDto> MonthlyBreakdown { get; set; } = new();
    }

    /// <summary>
    /// Monthly patients data
    /// </summary>
    public class MonthlyPatientsDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int NewPatients { get; set; }
        public int TotalPatients { get; set; }
    }

    /// <summary>
    /// Accounts receivable report (contas a receber)
    /// </summary>
    public class AccountsReceivableReportDto
    {
        public decimal TotalPending { get; set; }
        public decimal TotalOverdue { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
        public List<OverdueInvoiceDto> OverdueInvoices { get; set; } = new();
    }

    /// <summary>
    /// Overdue invoice summary
    /// </summary>
    public class OverdueInvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public string PatientName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Accounts payable report (contas a pagar)
    /// </summary>
    public class AccountsPayableReportDto
    {
        public decimal TotalPending { get; set; }
        public decimal TotalOverdue { get; set; }
        public int PendingCount { get; set; }
        public int OverdueCount { get; set; }
        public List<OverdueExpenseDto> OverdueExpenses { get; set; } = new();
    }

    /// <summary>
    /// Overdue expense summary
    /// </summary>
    public class OverdueExpenseDto
    {
        public Guid ExpenseId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public int DaysOverdue { get; set; }
        public string? SupplierName { get; set; }
    }
}
