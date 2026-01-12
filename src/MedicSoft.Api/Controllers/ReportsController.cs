using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
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
    }
}
