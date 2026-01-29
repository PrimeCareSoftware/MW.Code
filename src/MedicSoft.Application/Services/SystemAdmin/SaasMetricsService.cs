using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface ISaasMetricsService
    {
        Task<SaasDashboardDto> GetSaasDashboardMetricsAsync();
        Task<MrrBreakdownDto> GetMrrBreakdownAsync();
        Task<ChurnAnalysisDto> GetChurnAnalysisAsync();
        Task<GrowthMetricsDto> GetGrowthMetricsAsync();
        Task<List<RevenueTimelineDto>> GetRevenueTimelineAsync(int months = 12);
        Task<CustomerBreakdownDto> GetCustomerBreakdownAsync();
    }

    public class SaasMetricsService : ISaasMetricsService
    {
        private readonly MedicSoftDbContext _context;

        public SaasMetricsService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<SaasDashboardDto> GetSaasDashboardMetricsAsync()
        {
            var now = DateTime.UtcNow;
            var lastMonth = now.AddMonths(-1);

            // Get active subscriptions
            var activeSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > now)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var lastMonthSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > lastMonth && s.EndDate <= now.AddMonths(1))
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            // Calculate MRR
            var mrr = activeSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);
            var lastMonthMrr = lastMonthSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);
            
            var arr = mrr * 12;

            // Active customers
            var activeCustomers = activeSubscriptions.Count;
            
            // New customers this month
            var newCustomers = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .CountAsync(s => s.CreatedAt >= now.AddMonths(-1) && s.Status == SubscriptionStatus.Active);

            // Churned customers this month
            var churnedCustomers = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .CountAsync(s => s.Status == SubscriptionStatus.Cancelled && s.EndDate >= now.AddMonths(-1) && s.EndDate < now);

            // Churn rate
            var churnRate = lastMonthSubscriptions.Count > 0 
                ? (decimal)churnedCustomers / lastMonthSubscriptions.Count * 100 
                : 0;

            // ARPU
            var arpu = activeCustomers > 0 ? mrr / activeCustomers : 0;

            // LTV (simplified: ARPU / monthly churn rate)
            var monthlyChurnRate = churnRate / 100;
            var ltv = monthlyChurnRate > 0 ? arpu / monthlyChurnRate : arpu * 12;

            // TODO: CAC requires marketing cost tracking - implement when marketing data is available
            var cac = 500m; // Placeholder - needs marketing cost data

            // LTV/CAC Ratio
            var ltvCacRatio = cac > 0 ? ltv / cac : 0;

            // MRR Growth MoM
            var mrrGrowthMoM = lastMonthMrr > 0 ? (mrr - lastMonthMrr) / lastMonthMrr * 100 : 0;

            // TODO: Growth Rate YoY requires historical data tracking - improve accuracy
            var lastYearMrr = mrr * 0.8m; // Placeholder - needs historical MRR data
            var growthRateYoY = lastYearMrr > 0 ? (mrr - lastYearMrr) / lastYearMrr * 100 : 0;

            // Quick Ratio
            var mrrBreakdown = await GetMrrBreakdownAsync();
            var quickRatio = (mrrBreakdown.ContractionMrr + mrrBreakdown.ChurnedMrr) > 0
                ? (mrrBreakdown.NewMrr + mrrBreakdown.ExpansionMrr) / (mrrBreakdown.ContractionMrr + mrrBreakdown.ChurnedMrr)
                : 0;

            // Trial customers
            var trialCustomers = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .CountAsync(s => s.Status == SubscriptionStatus.Trial && s.TrialEndDate > now);

            // At-risk customers (no activity in 30 days - simplified)
            var atRiskCustomers = 0; // Would need usage/login data

            return new SaasDashboardDto
            {
                Mrr = mrr,
                Arr = arr,
                ActiveCustomers = activeCustomers,
                NewCustomers = newCustomers,
                ChurnedCustomers = churnedCustomers,
                ChurnRate = churnRate,
                Arpu = arpu,
                Ltv = ltv,
                Cac = cac,
                LtvCacRatio = ltvCacRatio,
                MrrGrowthMoM = mrrGrowthMoM,
                GrowthRateYoY = growthRateYoY,
                QuickRatio = quickRatio,
                MrrTrend = mrrGrowthMoM > 0 ? "up" : mrrGrowthMoM < 0 ? "down" : "stable",
                TrialCustomers = trialCustomers,
                AtRiskCustomers = atRiskCustomers
            };
        }

        public async Task<MrrBreakdownDto> GetMrrBreakdownAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Current MRR
            var activeSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > now)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var totalMrr = activeSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

            // New MRR (new subscriptions this month)
            var newSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.StartDate >= startOfMonth)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var newMrr = newSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

            // Expansion MRR (upgrades - simplified, would need plan change tracking)
            var expansionMrr = 0m;

            // Contraction MRR (downgrades - simplified)
            var contractionMrr = 0m;

            // Churned MRR (cancelled subscriptions this month)
            var churnedSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Cancelled && s.EndDate >= startOfMonth && s.EndDate < now)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var churnedMrr = churnedSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

            var netNewMrr = newMrr + expansionMrr - contractionMrr - churnedMrr;

            return new MrrBreakdownDto
            {
                TotalMrr = totalMrr,
                NewMrr = newMrr,
                ExpansionMrr = expansionMrr,
                ContractionMrr = contractionMrr,
                ChurnedMrr = churnedMrr,
                NetNewMrr = netNewMrr
            };
        }

        public async Task<ChurnAnalysisDto> GetChurnAnalysisAsync()
        {
            var now = DateTime.UtcNow;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);

            // Get churned subscriptions this month
            var churnedSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Cancelled && s.EndDate >= startOfMonth && s.EndDate < now)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var monthlyRevenueChurn = churnedSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);
            var monthlyCustomerChurn = churnedSubscriptions.Count;

            // Get active subscriptions from last month
            var lastMonthStart = startOfMonth.AddMonths(-1);
            var lastMonthSubscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > lastMonthStart && s.EndDate <= startOfMonth.AddMonths(1))
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            var lastMonthRevenue = lastMonthSubscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);
            var lastMonthCustomers = lastMonthSubscriptions.Count;

            var revenueChurnRate = lastMonthRevenue > 0 ? monthlyRevenueChurn / lastMonthRevenue * 100 : 0;
            var customerChurnRate = lastMonthCustomers > 0 ? (decimal)monthlyCustomerChurn / lastMonthCustomers * 100 : 0;

            // Annual churn (simplified)
            var annualRevenueChurn = revenueChurnRate * 12;
            var annualCustomerChurn = customerChurnRate * 12;

            // Get churn history for last 6 months
            var churnHistory = new List<ChurnDataPoint>();
            for (int i = 5; i >= 0; i--)
            {
                var monthStart = now.AddMonths(-i).Date;
                var monthEnd = monthStart.AddMonths(1);
                var monthName = monthStart.ToString("MMM yyyy");

                var monthChurned = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == SubscriptionStatus.Cancelled && s.EndDate >= monthStart && s.EndDate < monthEnd)
                    .Include(s => s.SubscriptionPlan)
                    .ToListAsync();

                churnHistory.Add(new ChurnDataPoint
                {
                    Month = monthName,
                    RevenueChurn = monthChurned.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0),
                    CustomerChurn = monthChurned.Count,
                    ChurnedCount = monthChurned.Count
                });
            }

            return new ChurnAnalysisDto
            {
                RevenueChurnRate = revenueChurnRate,
                CustomerChurnRate = customerChurnRate,
                MonthlyRevenueChurn = monthlyRevenueChurn,
                MonthlyCustomerChurn = monthlyCustomerChurn,
                AnnualRevenueChurn = annualRevenueChurn,
                AnnualCustomerChurn = annualCustomerChurn,
                ChurnHistory = churnHistory
            };
        }

        public async Task<GrowthMetricsDto> GetGrowthMetricsAsync()
        {
            var now = DateTime.UtcNow;
            
            // Get MRR for current and last month
            var currentMonthStart = new DateTime(now.Year, now.Month, 1);
            var lastMonthStart = currentMonthStart.AddMonths(-1);
            var lastYearStart = currentMonthStart.AddYears(-1);

            var currentMrr = await CalculateMrrForDate(currentMonthStart);
            var lastMonthMrr = await CalculateMrrForDate(lastMonthStart);
            var lastYearMrr = await CalculateMrrForDate(lastYearStart);

            var momGrowthRate = lastMonthMrr > 0 ? (currentMrr - lastMonthMrr) / lastMonthMrr * 100 : 0;
            var yoyGrowthRate = lastYearMrr > 0 ? (currentMrr - lastYearMrr) / lastYearMrr * 100 : 0;

            // Quick Ratio
            var mrrBreakdown = await GetMrrBreakdownAsync();
            var quickRatio = (mrrBreakdown.ContractionMrr + mrrBreakdown.ChurnedMrr) > 0
                ? (mrrBreakdown.NewMrr + mrrBreakdown.ExpansionMrr) / (mrrBreakdown.ContractionMrr + mrrBreakdown.ChurnedMrr)
                : 0;

            // Trial conversion rate
            var trialConversions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .CountAsync(s => s.Status == SubscriptionStatus.Active && s.TrialEndDate != null && s.TrialEndDate < now && s.StartDate >= now.AddMonths(-3));

            var totalTrials = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .CountAsync(s => s.TrialEndDate != null && s.TrialEndDate >= now.AddMonths(-3) && s.TrialEndDate < now);

            var trialConversionRate = totalTrials > 0 ? (decimal)trialConversions / totalTrials * 100 : 0;

            // Growth history
            var growthHistory = new List<GrowthDataPoint>();
            for (int i = 11; i >= 0; i--)
            {
                var monthStart = now.AddMonths(-i);
                var monthMrr = await CalculateMrrForDate(monthStart);
                var prevMonthMrr = await CalculateMrrForDate(monthStart.AddMonths(-1));
                var growth = prevMonthMrr > 0 ? (monthMrr - prevMonthMrr) / prevMonthMrr * 100 : 0;

                growthHistory.Add(new GrowthDataPoint
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    GrowthRate = growth,
                    Mrr = monthMrr
                });
            }

            return new GrowthMetricsDto
            {
                MoMGrowthRate = momGrowthRate,
                YoYGrowthRate = yoyGrowthRate,
                QuickRatio = quickRatio,
                TrialConversionRate = trialConversionRate,
                GrowthHistory = growthHistory
            };
        }

        public async Task<List<RevenueTimelineDto>> GetRevenueTimelineAsync(int months = 12)
        {
            var now = DateTime.UtcNow;
            var timeline = new List<RevenueTimelineDto>();

            for (int i = months - 1; i >= 0; i--)
            {
                var monthDate = now.AddMonths(-i);
                var monthStart = new DateTime(monthDate.Year, monthDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1);

                // Active subscriptions for this month
                var subscriptions = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == SubscriptionStatus.Active && s.StartDate < monthEnd && s.EndDate > monthStart)
                    .Include(s => s.SubscriptionPlan)
                    .ToListAsync();

                var totalMrr = subscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

                // New subscriptions
                var newSubs = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == SubscriptionStatus.Active && s.StartDate >= monthStart && s.StartDate < monthEnd)
                    .Include(s => s.SubscriptionPlan)
                    .ToListAsync();

                var newMrr = newSubs.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

                // Churned subscriptions
                var churnedSubs = await _context.ClinicSubscriptions
                    .IgnoreQueryFilters()
                    .Where(s => s.Status == SubscriptionStatus.Cancelled && s.EndDate >= monthStart && s.EndDate < monthEnd)
                    .Include(s => s.SubscriptionPlan)
                    .ToListAsync();

                var churnedMrr = churnedSubs.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);

                timeline.Add(new RevenueTimelineDto
                {
                    Month = monthStart.ToString("MMM yyyy"),
                    Date = monthStart,
                    TotalMrr = totalMrr,
                    NewMrr = newMrr,
                    ExpansionMrr = 0, // Placeholder
                    ContractionMrr = 0, // Placeholder
                    ChurnedMrr = churnedMrr,
                    ActiveCustomers = subscriptions.Count
                });
            }

            return timeline;
        }

        public async Task<CustomerBreakdownDto> GetCustomerBreakdownAsync()
        {
            var now = DateTime.UtcNow;

            // Breakdown by plan
            var byPlan = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.EndDate > now)
                .Include(s => s.SubscriptionPlan)
                .GroupBy(s => s.SubscriptionPlan!.Name)
                .Select(g => new { PlanName = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PlanName, x => x.Count);

            // Breakdown by status
            var byStatus = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.EndDate > now)
                .GroupBy(s => s.Status.ToString())
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count);

            return new CustomerBreakdownDto
            {
                ByPlan = byPlan,
                ByStatus = byStatus
            };
        }

        private async Task<decimal> CalculateMrrForDate(DateTime date)
        {
            var monthEnd = new DateTime(date.Year, date.Month, 1).AddMonths(1);
            
            var subscriptions = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == SubscriptionStatus.Active && s.StartDate < monthEnd && s.EndDate > date)
                .Include(s => s.SubscriptionPlan)
                .ToListAsync();

            return subscriptions.Sum(s => s.SubscriptionPlan?.MonthlyPrice ?? 0);
        }
    }
}
