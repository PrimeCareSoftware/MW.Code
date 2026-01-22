using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for generating reports and analytics
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReportsController : BaseController
    {
        private readonly MedicSoftDbContext _context;

        public ReportsController(MedicSoftDbContext context, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _context = context;
        }

        /// <summary>
        /// Get financial summary for a period (requires reports.financial permission)
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Financial summary</returns>
        [HttpGet("financial-summary")]
        [RequirePermissionKey(PermissionKeys.ReportsFinancial)]
        [ProducesResponseType(typeof(FinancialSummaryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FinancialSummaryDto>> GetFinancialSummary(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            // Get payments (revenue)
            var payments = await _context.Payments
                .Where(p => p.Appointment != null && 
                           p.Appointment.ClinicId == clinicId &&
                           p.Status == PaymentStatus.Paid &&
                           p.ProcessedDate >= startDate && 
                           p.ProcessedDate <= endDate)
                .ToListAsync();

            var totalRevenue = payments.Sum(p => p.Amount);

            // Get expenses
            var expenses = await _context.Expenses
                .Where(e => e.ClinicId == clinicId &&
                           e.Status == ExpenseStatus.Paid &&
                           e.PaidDate >= startDate && 
                           e.PaidDate <= endDate)
                .ToListAsync();

            var totalExpenses = expenses.Sum(e => e.Amount);

            // Get appointments
            var appointments = await _context.Appointments
                .Where(a => a.ClinicId == clinicId &&
                           a.ScheduledDate >= startDate.Date &&
                           a.ScheduledDate <= endDate.Date)
                .ToListAsync();

            var totalAppointments = appointments.Count;

            // Get unique patients
            var totalPatients = await _context.Appointments
                .Where(a => a.ClinicId == clinicId &&
                           a.ScheduledDate >= startDate.Date &&
                           a.ScheduledDate <= endDate.Date)
                .Select(a => a.PatientId)
                .Distinct()
                .CountAsync();

            // Revenue by payment method
            var revenueByMethod = payments
                .GroupBy(p => p.Method)
                .Select(g => new RevenueByMethodDto
                {
                    PaymentMethod = g.Key.ToString(),
                    Amount = g.Sum(p => p.Amount),
                    Count = g.Count(),
                    Percentage = totalRevenue > 0 ? (g.Sum(p => p.Amount) / totalRevenue * 100) : 0
                })
                .OrderByDescending(r => r.Amount)
                .ToList();

            // Expenses by category
            var expensesByCategory = expenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseByCategoryDto
                {
                    Category = g.Key.ToString(),
                    Amount = g.Sum(e => e.Amount),
                    Count = g.Count(),
                    Percentage = totalExpenses > 0 ? (g.Sum(e => e.Amount) / totalExpenses * 100) : 0
                })
                .OrderByDescending(e => e.Amount)
                .ToList();

            return Ok(new FinancialSummaryDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                NetProfit = totalRevenue - totalExpenses,
                TotalAppointments = totalAppointments,
                TotalPatients = totalPatients,
                AverageAppointmentValue = totalAppointments > 0 ? totalRevenue / totalAppointments : 0,
                RevenueByPaymentMethod = revenueByMethod,
                ExpensesByCategory = expensesByCategory
            });
        }

        /// <summary>
        /// Get revenue report with daily breakdown
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Revenue report</returns>
        [HttpGet("revenue")]
        [ProducesResponseType(typeof(RevenueReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RevenueReportDto>> GetRevenueReport(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            var payments = await _context.Payments
                .Where(p => p.Appointment != null &&
                           p.Appointment.ClinicId == clinicId &&
                           p.Status == PaymentStatus.Paid &&
                           p.ProcessedDate >= startDate &&
                           p.ProcessedDate <= endDate)
                .ToListAsync();

            var dailyBreakdown = payments
                .GroupBy(p => p.ProcessedDate!.Value.Date)
                .Select(g => new DailyRevenueDto
                {
                    Date = g.Key,
                    Revenue = g.Sum(p => p.Amount),
                    Transactions = g.Count()
                })
                .OrderBy(d => d.Date)
                .ToList();

            return Ok(new RevenueReportDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalRevenue = payments.Sum(p => p.Amount),
                TotalTransactions = payments.Count,
                DailyBreakdown = dailyBreakdown
            });
        }

        /// <summary>
        /// Get appointments report with statistics
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Appointments report</returns>
        [HttpGet("appointments")]
        [ProducesResponseType(typeof(AppointmentsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AppointmentsReportDto>> GetAppointmentsReport(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            var appointments = await _context.Appointments
                .Where(a => a.ClinicId == clinicId &&
                           a.ScheduledDate >= startDate.Date &&
                           a.ScheduledDate <= endDate.Date)
                .ToListAsync();

            var totalAppointments = appointments.Count;
            var completedAppointments = appointments.Count(a => a.Status == AppointmentStatus.Completed);
            var cancelledAppointments = appointments.Count(a => a.Status == AppointmentStatus.Cancelled);
            var noShowAppointments = appointments.Count(a => a.Status == AppointmentStatus.NoShow);

            var appointmentsByStatus = appointments
                .GroupBy(a => a.Status)
                .Select(g => new AppointmentsByStatusDto
                {
                    Status = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = totalAppointments > 0 ? ((decimal)g.Count() / totalAppointments * 100) : 0
                })
                .OrderByDescending(a => a.Count)
                .ToList();

            var appointmentsByType = appointments
                .GroupBy(a => a.Type)
                .Select(g => new AppointmentsByTypeDto
                {
                    Type = g.Key.ToString(),
                    Count = g.Count(),
                    Percentage = totalAppointments > 0 ? ((decimal)g.Count() / totalAppointments * 100) : 0
                })
                .OrderByDescending(a => a.Count)
                .ToList();

            return Ok(new AppointmentsReportDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalAppointments = totalAppointments,
                CompletedAppointments = completedAppointments,
                CancelledAppointments = cancelledAppointments,
                NoShowAppointments = noShowAppointments,
                CompletionRate = totalAppointments > 0 ? ((decimal)completedAppointments / totalAppointments * 100) : 0,
                CancellationRate = totalAppointments > 0 ? ((decimal)cancelledAppointments / totalAppointments * 100) : 0,
                AppointmentsByStatus = appointmentsByStatus,
                AppointmentsByType = appointmentsByType
            });
        }

        /// <summary>
        /// Get patients growth report
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Patients report</returns>
        [HttpGet("patients")]
        [ProducesResponseType(typeof(PatientsReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientsReportDto>> GetPatientsReport(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            // Get all patient-clinic links for this clinic
            var patientLinks = await _context.PatientClinicLinks
                .Where(l => l.ClinicId == clinicId)
                .Include(l => l.Patient)
                .ToListAsync();

            var totalPatients = patientLinks.Count;

            // New patients in period
            var newPatients = patientLinks
                .Count(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate);

            // Active patients (with appointments in period)
            var activePatients = await _context.Appointments
                .Where(a => a.ClinicId == clinicId &&
                           a.ScheduledDate >= startDate.Date &&
                           a.ScheduledDate <= endDate.Date)
                .Select(a => a.PatientId)
                .Distinct()
                .CountAsync();

            // Monthly breakdown
            var monthlyBreakdown = patientLinks
                .Where(l => l.CreatedAt >= startDate && l.CreatedAt <= endDate)
                .GroupBy(l => new { l.CreatedAt.Year, l.CreatedAt.Month })
                .Select(g => new MonthlyPatientsDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NewPatients = g.Count(),
                    TotalPatients = patientLinks.Count(l => l.CreatedAt <= new DateTime(g.Key.Year, g.Key.Month, 1).AddMonths(1).AddDays(-1))
                })
                .OrderBy(m => m.Year)
                .ThenBy(m => m.Month)
                .ToList();

            return Ok(new PatientsReportDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalPatients = totalPatients,
                NewPatients = newPatients,
                ActivePatients = activePatients,
                MonthlyBreakdown = monthlyBreakdown
            });
        }

        /// <summary>
        /// Get accounts receivable report
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <returns>Accounts receivable report</returns>
        [HttpGet("accounts-receivable")]
        [ProducesResponseType(typeof(AccountsReceivableReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountsReceivableReportDto>> GetAccountsReceivable(
            [FromQuery] Guid clinicId)
        {
            var pendingInvoices = await _context.Invoices
                .Include(i => i.Payment)
                .ThenInclude(p => p.Appointment!)
                .ThenInclude(a => a.Patient)
                .Where(i => i.TenantId == GetTenantId() &&
                           (i.Status == InvoiceStatus.Draft || i.Status == InvoiceStatus.Issued))
                .ToListAsync();

            var overdueInvoices = pendingInvoices
                .Where(i => i.DueDate < DateTime.UtcNow.Date)
                .Select(i => new OverdueInvoiceDto
                {
                    InvoiceId = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    Amount = i.TotalAmount,
                    DueDate = i.DueDate,
                    DaysOverdue = (DateTime.UtcNow.Date - i.DueDate).Days,
                    PatientName = i.Payment?.Appointment?.Patient?.Name ?? i.CustomerName
                })
                .OrderByDescending(i => i.DaysOverdue)
                .ToList();

            return Ok(new AccountsReceivableReportDto
            {
                TotalPending = pendingInvoices.Sum(i => i.TotalAmount),
                TotalOverdue = overdueInvoices.Sum(i => i.Amount),
                PendingCount = pendingInvoices.Count,
                OverdueCount = overdueInvoices.Count,
                OverdueInvoices = overdueInvoices
            });
        }

        /// <summary>
        /// Get accounts payable report
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <returns>Accounts payable report</returns>
        [HttpGet("accounts-payable")]
        [ProducesResponseType(typeof(AccountsPayableReportDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<AccountsPayableReportDto>> GetAccountsPayable(
            [FromQuery] Guid clinicId)
        {
            var pendingExpenses = await _context.Expenses
                .Where(e => e.ClinicId == clinicId &&
                           (e.Status == ExpenseStatus.Pending || e.Status == ExpenseStatus.Overdue))
                .ToListAsync();

            var overdueExpenses = pendingExpenses
                .Where(e => e.IsOverdue())
                .Select(e => new OverdueExpenseDto
                {
                    ExpenseId = e.Id,
                    Description = e.Description,
                    Category = e.Category.ToString(),
                    Amount = e.Amount,
                    DueDate = e.DueDate,
                    DaysOverdue = e.DaysOverdue(),
                    SupplierName = e.SupplierName
                })
                .OrderByDescending(e => e.DaysOverdue)
                .ToList();

            return Ok(new AccountsPayableReportDto
            {
                TotalPending = pendingExpenses.Sum(e => e.Amount),
                TotalOverdue = overdueExpenses.Sum(e => e.Amount),
                PendingCount = pendingExpenses.Count,
                OverdueCount = overdueExpenses.Count,
                OverdueExpenses = overdueExpenses
            });
        }

        /// <summary>
        /// Get DRE - Demonstrativo de Resultados do Exercício (Income Statement)
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>DRE report</returns>
        [HttpGet("dre")]
        [RequirePermissionKey(PermissionKeys.ReportsFinancial)]
        [ProducesResponseType(typeof(DREReportDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DREReportDto>> GetDREReport(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            // Receita Bruta (Gross Revenue) - Todos os pagamentos
            var payments = await _context.Payments
                .Where(p => p.Appointment != null &&
                           p.Appointment.ClinicId == clinicId &&
                           p.Status == PaymentStatus.Paid &&
                           p.ProcessedDate >= startDate &&
                           p.ProcessedDate <= endDate)
                .ToListAsync();

            var grossRevenue = payments.Sum(p => p.Amount);

            // Deduções (cancelamentos, estornos, descontos)
            var refunds = await _context.Payments
                .Where(p => p.Appointment != null &&
                           p.Appointment.ClinicId == clinicId &&
                           p.Status == PaymentStatus.Refunded &&
                           p.ProcessedDate >= startDate &&
                           p.ProcessedDate <= endDate)
                .SumAsync(p => p.Amount);

            var deductions = refunds;
            var netRevenue = grossRevenue - deductions;

            // Despesas por categoria
            var expenses = await _context.Expenses
                .Where(e => e.ClinicId == clinicId &&
                           e.Status == ExpenseStatus.Paid &&
                           e.PaidDate >= startDate &&
                           e.PaidDate <= endDate)
                .ToListAsync();

            // Classificar despesas
            var operationalCosts = expenses
                .Where(e => e.Category == ExpenseCategory.Supplies)
                .Sum(e => e.Amount);

            var administrativeExpenses = expenses
                .Where(e => e.Category == ExpenseCategory.Salary ||
                           e.Category == ExpenseCategory.Rent ||
                           e.Category == ExpenseCategory.Utilities ||
                           e.Category == ExpenseCategory.Maintenance ||
                           e.Category == ExpenseCategory.ProfessionalServices ||
                           e.Category == ExpenseCategory.Software)
                .Sum(e => e.Amount);

            var salesExpenses = expenses
                .Where(e => e.Category == ExpenseCategory.Marketing)
                .Sum(e => e.Amount);

            var financialExpenses = expenses
                .Where(e => e.Category == ExpenseCategory.Taxes ||
                           e.Category == ExpenseCategory.Insurance)
                .Sum(e => e.Amount);

            var totalExpenses = expenses.Sum(e => e.Amount);
            var operationalProfit = netRevenue - totalExpenses;
            var netProfit = operationalProfit; // Simplificado, sem impostos sobre lucro
            var profitMargin = netRevenue > 0 ? (netProfit / netRevenue * 100) : 0;

            // Detalhamento de receitas por método de pagamento
            var revenueDetails = payments
                .GroupBy(p => p.Method)
                .Select(g => new RevenueDetailDto
                {
                    Category = g.Key.ToString(),
                    Amount = g.Sum(p => p.Amount),
                    Percentage = grossRevenue > 0 ? (g.Sum(p => p.Amount) / grossRevenue * 100) : 0
                })
                .OrderByDescending(r => r.Amount)
                .ToList();

            // Detalhamento de despesas por categoria
            var expenseDetails = expenses
                .GroupBy(e => e.Category)
                .Select(g => new ExpenseDetailDto
                {
                    Category = g.Key.ToString(),
                    Amount = g.Sum(e => e.Amount),
                    Percentage = totalExpenses > 0 ? (g.Sum(e => e.Amount) / totalExpenses * 100) : 0
                })
                .OrderByDescending(e => e.Amount)
                .ToList();

            return Ok(new DREReportDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                GrossRevenue = grossRevenue,
                Deductions = deductions,
                NetRevenue = netRevenue,
                OperationalCosts = operationalCosts,
                AdministrativeExpenses = administrativeExpenses,
                SalesExpenses = salesExpenses,
                FinancialExpenses = financialExpenses,
                TotalExpenses = totalExpenses,
                OperationalProfit = operationalProfit,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,
                RevenueDetails = revenueDetails,
                ExpenseDetails = expenseDetails
            });
        }

        /// <summary>
        /// Get Cash Flow Forecast - Projeção de Fluxo de Caixa
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="months">Number of months to forecast (default: 3)</param>
        /// <returns>Cash flow forecast</returns>
        [HttpGet("cash-flow-forecast")]
        [RequirePermissionKey(PermissionKeys.ReportsFinancial)]
        [ProducesResponseType(typeof(CashFlowForecastDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CashFlowForecastDto>> GetCashFlowForecast(
            [FromQuery] Guid clinicId,
            [FromQuery] int months = 3)
        {
            if (months < 1 || months > 12)
                return BadRequest("Months must be between 1 and 12");

            var today = DateTime.UtcNow.Date;
            var endDate = today.AddMonths(months);

            // Saldo atual (aproximação baseada em contas a receber e pagar)
            var paidReceivables = await _context.AccountsReceivable
                .Include(r => r.Appointment)
                .Where(r => r.Appointment != null && 
                           r.Appointment.ClinicId == clinicId && 
                           r.Status == ReceivableStatus.Paid)
                .SumAsync(r => r.PaidAmount);

            var paidPayables = await _context.AccountsPayable
                .Where(p => p.TenantId == GetTenantId() && p.Status == PayableStatus.Paid)
                .SumAsync(p => p.PaidAmount);

            var currentBalance = paidReceivables - paidPayables;

            // Contas a receber pendentes
            var pendingReceivables = await _context.AccountsReceivable
                .Include(r => r.Patient)
                .Include(r => r.Appointment)
                .Where(r => r.Appointment != null && 
                           r.Appointment.ClinicId == clinicId &&
                           (r.Status == ReceivableStatus.Pending ||
                            r.Status == ReceivableStatus.PartiallyPaid ||
                            r.Status == ReceivableStatus.Overdue) &&
                           r.DueDate <= endDate)
                .Select(r => new ReceivableForecastDto
                {
                    Id = r.Id,
                    DocumentNumber = r.DocumentNumber,
                    DueDate = r.DueDate,
                    Amount = r.OutstandingAmount,
                    Status = r.Status.ToString(),
                    PatientName = r.Patient != null ? r.Patient.Name : "N/A"
                })
                .OrderBy(r => r.DueDate)
                .ToListAsync();

            // Contas a pagar pendentes
            var pendingPayables = await _context.AccountsPayable
                .Include(p => p.Supplier)
                .Where(p => p.TenantId == GetTenantId() &&
                           (p.Status == PayableStatus.Pending ||
                            p.Status == PayableStatus.PartiallyPaid ||
                            p.Status == PayableStatus.Overdue) &&
                           p.DueDate <= endDate)
                .Select(p => new PayableForecastDto
                {
                    Id = p.Id,
                    DocumentNumber = p.DocumentNumber,
                    DueDate = p.DueDate,
                    Amount = p.OutstandingAmount,
                    Category = p.Category.ToString(),
                    SupplierName = p.Supplier != null ? p.Supplier.Name : null
                })
                .OrderBy(p => p.DueDate)
                .ToListAsync();

            var projectedIncome = pendingReceivables.Sum(r => r.Amount);
            var projectedExpenses = pendingPayables.Sum(p => p.Amount);
            var projectedBalance = currentBalance + projectedIncome - projectedExpenses;

            // Previsão mensal
            var monthlyForecast = new List<MonthlyForecastDto>();
            var cumulativeBalance = currentBalance;

            for (int i = 0; i < months; i++)
            {
                var monthStart = today.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var monthIncome = pendingReceivables
                    .Where(r => r.DueDate >= monthStart && r.DueDate <= monthEnd)
                    .Sum(r => r.Amount);

                var monthExpenses = pendingPayables
                    .Where(p => p.DueDate >= monthStart && p.DueDate <= monthEnd)
                    .Sum(p => p.Amount);

                cumulativeBalance += monthIncome - monthExpenses;

                monthlyForecast.Add(new MonthlyForecastDto
                {
                    Year = monthStart.Year,
                    Month = monthStart.Month,
                    ExpectedIncome = monthIncome,
                    ExpectedExpenses = monthExpenses,
                    ExpectedBalance = monthIncome - monthExpenses,
                    CumulativeBalance = cumulativeBalance
                });
            }

            return Ok(new CashFlowForecastDto
            {
                StartDate = today,
                EndDate = endDate,
                CurrentBalance = currentBalance,
                ProjectedIncome = projectedIncome,
                ProjectedExpenses = projectedExpenses,
                ProjectedBalance = projectedBalance,
                MonthlyForecast = monthlyForecast,
                PendingReceivables = pendingReceivables,
                PendingPayables = pendingPayables
            });
        }

        /// <summary>
        /// Get Profitability Analysis - Análise de Rentabilidade
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <param name="startDate">Period start date</param>
        /// <param name="endDate">Period end date</param>
        /// <returns>Profitability analysis</returns>
        [HttpGet("profitability")]
        [RequirePermissionKey(PermissionKeys.ReportsFinancial)]
        [ProducesResponseType(typeof(ProfitabilityAnalysisDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProfitabilityAnalysisDto>> GetProfitabilityAnalysis(
            [FromQuery] Guid clinicId,
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate)
        {
            if (startDate > endDate)
                return BadRequest("Start date must be before end date");

            // Total de receitas
            var payments = await _context.Payments
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.Professional)
                .Include(p => p.Appointment)
                    .ThenInclude(a => a!.HealthInsurancePlan)
                        .ThenInclude(hip => hip!.Operator)
                .Where(p => p.Appointment != null &&
                           p.Appointment.ClinicId == clinicId &&
                           p.Status == PaymentStatus.Paid &&
                           p.ProcessedDate >= startDate &&
                           p.ProcessedDate <= endDate)
                .ToListAsync();

            var totalRevenue = payments.Sum(p => p.Amount);

            // Total de custos
            var expenses = await _context.Expenses
                .Where(e => e.ClinicId == clinicId &&
                           e.Status == ExpenseStatus.Paid &&
                           e.PaidDate >= startDate &&
                           e.PaidDate <= endDate)
                .SumAsync(e => e.Amount);

            var totalProfit = totalRevenue - expenses;
            var profitMargin = totalRevenue > 0 ? (totalProfit / totalRevenue * 100) : 0;

            // Rentabilidade por tipo de consulta (usando Type do Appointment)
            var byProcedure = payments
                .GroupBy(p => p.Appointment!.Type)
                .Select(g => new ProfitabilityByProcedureDto
                {
                    ProcedureName = g.Key.ToString(),
                    Count = g.Count(),
                    Revenue = g.Sum(p => p.Amount),
                    AverageValue = g.Average(p => p.Amount),
                    Percentage = totalRevenue > 0 ? (g.Sum(p => p.Amount) / totalRevenue * 100) : 0
                })
                .OrderByDescending(p => p.Revenue)
                .ToList();

            // Rentabilidade por médico
            var byDoctor = payments
                .Where(p => p.Appointment!.Professional != null)
                .GroupBy(p => new { p.Appointment!.ProfessionalId, p.Appointment.Professional!.FullName })
                .Select(g => new ProfitabilityByDoctorDto
                {
                    DoctorId = g.Key.ProfessionalId!.Value,
                    DoctorName = g.Key.FullName,
                    AppointmentsCount = g.Count(),
                    Revenue = g.Sum(p => p.Amount),
                    AverageAppointmentValue = g.Average(p => p.Amount),
                    Percentage = totalRevenue > 0 ? (g.Sum(p => p.Amount) / totalRevenue * 100) : 0
                })
                .OrderByDescending(d => d.Revenue)
                .ToList();

            // Rentabilidade por convênio (particular vs convênios)
            var byInsurance = payments
                .GroupBy(p => new
                {
                    InsuranceId = p.Appointment!.HealthInsurancePlan != null
                        ? p.Appointment.HealthInsurancePlan.OperatorId
                        : (Guid?)null,
                    InsuranceName = p.Appointment.HealthInsurancePlan != null && p.Appointment.HealthInsurancePlan.Operator != null
                        ? p.Appointment.HealthInsurancePlan.Operator.TradeName
                        : "Particular"
                })
                .Select(g => new ProfitabilityByInsuranceDto
                {
                    InsuranceId = g.Key.InsuranceId,
                    InsuranceName = g.Key.InsuranceName,
                    AppointmentsCount = g.Count(),
                    Revenue = g.Sum(p => p.Amount),
                    AverageValue = g.Average(p => p.Amount),
                    Percentage = totalRevenue > 0 ? (g.Sum(p => p.Amount) / totalRevenue * 100) : 0
                })
                .OrderByDescending(i => i.Revenue)
                .ToList();

            return Ok(new ProfitabilityAnalysisDto
            {
                PeriodStart = startDate,
                PeriodEnd = endDate,
                TotalRevenue = totalRevenue,
                TotalCosts = expenses,
                TotalProfit = totalProfit,
                ProfitMargin = profitMargin,
                ByProcedure = byProcedure,
                ByDoctor = byDoctor,
                ByInsurance = byInsurance
            });
        }
    }
}
