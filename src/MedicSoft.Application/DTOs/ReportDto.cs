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

    /// <summary>
    /// DRE - Demonstrativo de Resultados do Exercício (Income Statement)
    /// </summary>
    public class DREReportDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        
        // Receitas (Revenue)
        public decimal GrossRevenue { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetRevenue { get; set; }
        
        // Custos e Despesas (Costs and Expenses)
        public decimal OperationalCosts { get; set; }
        public decimal AdministrativeExpenses { get; set; }
        public decimal SalesExpenses { get; set; }
        public decimal FinancialExpenses { get; set; }
        public decimal TotalExpenses { get; set; }
        
        // Resultados (Results)
        public decimal OperationalProfit { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        
        // Detalhamentos (Details)
        public List<RevenueDetailDto> RevenueDetails { get; set; } = new();
        public List<ExpenseDetailDto> ExpenseDetails { get; set; } = new();
    }

    /// <summary>
    /// Revenue detail for DRE
    /// </summary>
    public class RevenueDetailDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Expense detail for DRE
    /// </summary>
    public class ExpenseDetailDto
    {
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Cash Flow Forecast - Projeção de Fluxo de Caixa
    /// </summary>
    public class CashFlowForecastDto
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal ProjectedIncome { get; set; }
        public decimal ProjectedExpenses { get; set; }
        public decimal ProjectedBalance { get; set; }
        public List<MonthlyForecastDto> MonthlyForecast { get; set; } = new();
        public List<ReceivableForecastDto> PendingReceivables { get; set; } = new();
        public List<PayableForecastDto> PendingPayables { get; set; } = new();
    }

    /// <summary>
    /// Monthly forecast data
    /// </summary>
    public class MonthlyForecastDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public decimal ExpectedIncome { get; set; }
        public decimal ExpectedExpenses { get; set; }
        public decimal ExpectedBalance { get; set; }
        public decimal CumulativeBalance { get; set; }
    }

    /// <summary>
    /// Receivable forecast item
    /// </summary>
    public class ReceivableForecastDto
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PatientName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Payable forecast item
    /// </summary>
    public class PayableForecastDto
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? SupplierName { get; set; }
    }

    /// <summary>
    /// Profitability Analysis - Análise de Rentabilidade
    /// </summary>
    public class ProfitabilityAnalysisDto
    {
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalCosts { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal ProfitMargin { get; set; }
        public List<ProfitabilityByProcedureDto> ByProcedure { get; set; } = new();
        public List<ProfitabilityByDoctorDto> ByDoctor { get; set; } = new();
        public List<ProfitabilityByInsuranceDto> ByInsurance { get; set; } = new();
    }

    /// <summary>
    /// Profitability by procedure
    /// </summary>
    public class ProfitabilityByProcedureDto
    {
        public string ProcedureName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageValue { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Profitability by doctor
    /// </summary>
    public class ProfitabilityByDoctorDto
    {
        public Guid DoctorId { get; set; }
        public string DoctorName { get; set; } = string.Empty;
        public int AppointmentsCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageAppointmentValue { get; set; }
        public decimal Percentage { get; set; }
    }

    /// <summary>
    /// Profitability by insurance operator
    /// </summary>
    public class ProfitabilityByInsuranceDto
    {
        public Guid? InsuranceId { get; set; }
        public string? InsuranceName { get; set; }
        public int AppointmentsCount { get; set; }
        public decimal Revenue { get; set; }
        public decimal AverageValue { get; set; }
        public decimal Percentage { get; set; }
    }
}
