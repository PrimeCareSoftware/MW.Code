using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.Cohorts;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.Cohorts
{
    public class CohortAnalysisService : ICohortAnalysisService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<CohortAnalysisService> _logger;

        public CohortAnalysisService(MedicSoftDbContext context, ILogger<CohortAnalysisService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<RetentionAnalysisDto> GetRetentionAnalysisAsync(int monthsBack = 12)
        {
            _logger.LogInformation("Calculating retention cohort analysis for {MonthsBack} months", monthsBack);

            var result = new RetentionAnalysisDto();
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack).Date;

            // Get all clinic subscriptions grouped by cohort month
            var subscriptions = await _context.ClinicSubscriptions
                .Where(cs => cs.StartDate >= startDate)
                .OrderBy(cs => cs.StartDate)
                .ToListAsync();

            if (!subscriptions.Any())
            {
                _logger.LogWarning("No subscription data found for retention analysis");
                return result;
            }

            // Group by cohort month
            var cohorts = subscriptions
                .GroupBy(cs => new DateTime(cs.StartDate.Year, cs.StartDate.Month, 1))
                .OrderBy(g => g.Key)
                .ToList();

            var monthsList = new List<string>();
            var maxMonthsTracked = 12;

            foreach (var cohortGroup in cohorts)
            {
                var cohortMonth = cohortGroup.Key;
                var cohortSize = cohortGroup.Count();
                var retentionRates = new List<decimal>();
                var retainedCounts = new List<int>();

                // Calculate retention for each month after cohort start
                for (int monthOffset = 0; monthOffset <= maxMonthsTracked; monthOffset++)
                {
                    var checkMonth = cohortMonth.AddMonths(monthOffset);
                    
                    if (checkMonth > DateTime.UtcNow)
                        break;

                    var retainedCount = cohortGroup.Count(cs =>
                    {
                        var endDate = cs.EndDate ?? DateTime.MaxValue;
                        return cs.StartDate <= checkMonth && endDate >= checkMonth;
                    });

                    retainedCounts.Add(retainedCount);
                    retentionRates.Add(cohortSize > 0 ? (decimal)retainedCount / cohortSize * 100 : 0);
                }

                result.Cohorts.Add(new RetentionCohortDto
                {
                    CohortMonth = cohortMonth.ToString("yyyy-MM"),
                    CohortSize = cohortSize,
                    RetentionRates = retentionRates,
                    RetainedCounts = retainedCounts
                });
            }

            // Build months list
            if (result.Cohorts.Any())
            {
                var maxLength = result.Cohorts.Max(c => c.RetentionRates.Count);
                for (int i = 0; i < maxLength; i++)
                {
                    monthsList.Add($"Month {i}");
                }
                result.Months = monthsList;
            }

            // Calculate averages
            if (result.Cohorts.Any())
            {
                var cohortsWithMonth1 = result.Cohorts.Where(c => c.RetentionRates.Count > 1).ToList();
                var cohortsWithMonth3 = result.Cohorts.Where(c => c.RetentionRates.Count > 3).ToList();
                var cohortsWithMonth6 = result.Cohorts.Where(c => c.RetentionRates.Count > 6).ToList();
                var cohortsWithMonth12 = result.Cohorts.Where(c => c.RetentionRates.Count > 12).ToList();

                result.AverageRetentionMonth1 = cohortsWithMonth1.Any() 
                    ? cohortsWithMonth1.Average(c => c.RetentionRates[1]) 
                    : 0;
                result.AverageRetentionMonth3 = cohortsWithMonth3.Any() 
                    ? cohortsWithMonth3.Average(c => c.RetentionRates[3]) 
                    : 0;
                result.AverageRetentionMonth6 = cohortsWithMonth6.Any() 
                    ? cohortsWithMonth6.Average(c => c.RetentionRates[6]) 
                    : 0;
                result.AverageRetentionMonth12 = cohortsWithMonth12.Any() 
                    ? cohortsWithMonth12.Average(c => c.RetentionRates[12]) 
                    : 0;
            }

            _logger.LogInformation("Retention analysis completed with {CohortCount} cohorts", result.Cohorts.Count);

            return result;
        }

        public async Task<RevenueCohortAnalysisDto> GetRevenueCohortAnalysisAsync(int monthsBack = 12)
        {
            _logger.LogInformation("Calculating revenue cohort analysis for {MonthsBack} months", monthsBack);

            var result = new RevenueCohortAnalysisDto();
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack).Date;

            // Get all clinic subscriptions with plan information
            var subscriptions = await _context.ClinicSubscriptions
                .Include(cs => cs.SubscriptionPlan)
                .Where(cs => cs.StartDate >= startDate)
                .OrderBy(cs => cs.StartDate)
                .ToListAsync();

            if (!subscriptions.Any())
            {
                _logger.LogWarning("No subscription data found for revenue analysis");
                return result;
            }

            // Group by cohort month
            var cohorts = subscriptions
                .GroupBy(cs => new DateTime(cs.StartDate.Year, cs.StartDate.Month, 1))
                .OrderBy(g => g.Key)
                .ToList();

            var monthsList = new List<string>();
            var maxMonthsTracked = 12;

            foreach (var cohortGroup in cohorts)
            {
                var cohortMonth = cohortGroup.Key;
                var cohortSize = cohortGroup.Count();
                var mrrByMonth = new List<decimal>();
                var cumulativeRevenue = new List<decimal>();
                decimal totalRevenue = 0;

                // Calculate MRR for each month after cohort start
                for (int monthOffset = 0; monthOffset <= maxMonthsTracked; monthOffset++)
                {
                    var checkMonth = cohortMonth.AddMonths(monthOffset);
                    
                    if (checkMonth > DateTime.UtcNow)
                        break;

                    var monthMrr = cohortGroup
                        .Where(cs =>
                        {
                            var endDate = cs.EndDate ?? DateTime.MaxValue;
                            return cs.StartDate <= checkMonth && endDate >= checkMonth;
                        })
                        .Sum(cs => cs.SubscriptionPlan?.MonthlyPrice ?? cs.CurrentPrice);

                    mrrByMonth.Add(monthMrr);
                    totalRevenue += monthMrr;
                    cumulativeRevenue.Add(totalRevenue);
                }

                var avgRevenuePerCustomer = cohortSize > 0 ? totalRevenue / cohortSize : 0;

                result.Cohorts.Add(new RevenueCohortDto
                {
                    CohortMonth = cohortMonth.ToString("yyyy-MM"),
                    CohortSize = cohortSize,
                    MrrByMonth = mrrByMonth,
                    CumulativeRevenue = cumulativeRevenue,
                    LifetimeValue = totalRevenue,
                    AverageRevenuePerCustomer = avgRevenuePerCustomer
                });
            }

            // Build months list
            if (result.Cohorts.Any())
            {
                var maxLength = result.Cohorts.Max(c => c.MrrByMonth.Count);
                for (int i = 0; i < maxLength; i++)
                {
                    monthsList.Add($"Month {i}");
                }
                result.Months = monthsList;
            }

            // Calculate averages and metrics
            if (result.Cohorts.Any())
            {
                result.AverageLTV = result.Cohorts.Average(c => c.AverageRevenuePerCustomer);
                result.TotalMRR = result.Cohorts
                    .Where(c => c.MrrByMonth.Any())
                    .Sum(c => c.MrrByMonth.Last());

                // Calculate MoM growth
                if (result.Cohorts.Count >= 2)
                {
                    var lastMonthMrr = result.Cohorts[result.Cohorts.Count - 1].MrrByMonth.FirstOrDefault();
                    var prevMonthMrr = result.Cohorts[result.Cohorts.Count - 2].MrrByMonth.FirstOrDefault();
                    
                    if (prevMonthMrr > 0)
                    {
                        result.MonthOverMonthGrowth = ((lastMonthMrr - prevMonthMrr) / prevMonthMrr) * 100;
                    }
                }
            }

            _logger.LogInformation("Revenue cohort analysis completed with {CohortCount} cohorts", result.Cohorts.Count);

            return result;
        }

        public async Task<ComprehensiveChurnAnalysisDto> GetChurnAnalysisAsync(int monthsBack = 12)
        {
            _logger.LogInformation("Calculating comprehensive churn analysis for {MonthsBack} months", monthsBack);

            var result = new ComprehensiveChurnAnalysisDto();
            var startDate = DateTime.UtcNow.AddMonths(-monthsBack).Date;

            // Get all subscriptions
            var allSubscriptions = await _context.ClinicSubscriptions
                .Where(cs => cs.StartDate >= startDate.AddMonths(-1) || 
                            (cs.EndDate.HasValue && cs.EndDate >= startDate))
                .OrderBy(cs => cs.StartDate)
                .ToListAsync();

            if (!allSubscriptions.Any())
            {
                _logger.LogWarning("No subscription data found for churn analysis");
                return result;
            }

            // Analyze each month
            for (int i = 0; i < monthsBack; i++)
            {
                var monthStart = startDate.AddMonths(i);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                var startingCustomers = allSubscriptions.Count(cs =>
                    cs.StartDate < monthStart &&
                    (!cs.EndDate.HasValue || cs.EndDate >= monthStart));

                var newCustomers = allSubscriptions.Count(cs =>
                    cs.StartDate >= monthStart && cs.StartDate <= monthEnd);

                var churnedCustomers = allSubscriptions.Count(cs =>
                    cs.EndDate.HasValue &&
                    cs.EndDate >= monthStart &&
                    cs.EndDate <= monthEnd);

                var endingCustomers = startingCustomers + newCustomers - churnedCustomers;

                var churnRate = startingCustomers > 0 
                    ? (decimal)churnedCustomers / startingCustomers * 100 
                    : 0;

                var growthRate = startingCustomers > 0 
                    ? (decimal)newCustomers / startingCustomers * 100 
                    : 0;

                var netRetentionRate = startingCustomers > 0 
                    ? (decimal)(startingCustomers + newCustomers - churnedCustomers) / startingCustomers * 100 
                    : 0;

                result.MonthlyChurn.Add(new ChurnAnalysisDto
                {
                    Month = monthStart.ToString("yyyy-MM"),
                    StartingCustomers = startingCustomers,
                    NewCustomers = newCustomers,
                    ChurnedCustomers = churnedCustomers,
                    EndingCustomers = endingCustomers,
                    ChurnRate = churnRate,
                    GrowthRate = growthRate,
                    NetRetentionRate = netRetentionRate
                });
            }

            // Calculate averages and trends
            if (result.MonthlyChurn.Any())
            {
                result.AverageChurnRate = result.MonthlyChurn.Average(m => m.ChurnRate);
                result.CurrentChurnRate = result.MonthlyChurn.Last().ChurnRate;

                // Determine trend
                if (result.MonthlyChurn.Count >= 3)
                {
                    var recentAvg = result.MonthlyChurn.TakeLast(3).Average(m => m.ChurnRate);
                    var previousAvg = result.MonthlyChurn.SkipLast(3).TakeLast(3).Average(m => m.ChurnRate);

                    if (recentAvg < previousAvg * 0.9m)
                        result.ChurnTrend = "improving";
                    else if (recentAvg > previousAvg * 1.1m)
                        result.ChurnTrend = "worsening";
                    else
                        result.ChurnTrend = "stable";
                }

                // TODO: Implement actual churn reason tracking
                result.TopChurnReasons = new List<string>
                {
                    "Price concerns",
                    "Feature limitations",
                    "Customer support issues",
                    "Competitor switch",
                    "Business closure"
                };
            }

            _logger.LogInformation("Churn analysis completed for {MonthCount} months", result.MonthlyChurn.Count);

            return result;
        }

        public async Task<CohortComparisonDto> CompareCohort(string cohort1Month, string cohort2Month)
        {
            _logger.LogInformation("Comparing cohorts: {Cohort1} vs {Cohort2}", cohort1Month, cohort2Month);

            // Get retention data for both cohorts
            var retentionAnalysis = await GetRetentionAnalysisAsync(24);
            var revenueAnalysis = await GetRevenueCohortAnalysisAsync(24);

            var cohort1Retention = retentionAnalysis.Cohorts.FirstOrDefault(c => c.CohortMonth == cohort1Month);
            var cohort2Retention = retentionAnalysis.Cohorts.FirstOrDefault(c => c.CohortMonth == cohort2Month);

            var cohort1Revenue = revenueAnalysis.Cohorts.FirstOrDefault(c => c.CohortMonth == cohort1Month);
            var cohort2Revenue = revenueAnalysis.Cohorts.FirstOrDefault(c => c.CohortMonth == cohort2Month);

            if (cohort1Retention == null || cohort2Retention == null)
            {
                throw new InvalidOperationException("One or both cohorts not found");
            }

            var retentionDiff = cohort1Retention.RetentionRates.LastOrDefault() - cohort2Retention.RetentionRates.LastOrDefault();
            var revenueDiff = (cohort1Revenue?.AverageRevenuePerCustomer ?? 0) - (cohort2Revenue?.AverageRevenuePerCustomer ?? 0);
            var ltvDiff = (cohort1Revenue?.LifetimeValue ?? 0) - (cohort2Revenue?.LifetimeValue ?? 0);

            string performanceSummary;
            if (retentionDiff > 5 && revenueDiff > 0)
                performanceSummary = $"Cohort {cohort1Month} significantly outperforms {cohort2Month}";
            else if (retentionDiff < -5 && revenueDiff < 0)
                performanceSummary = $"Cohort {cohort2Month} significantly outperforms {cohort1Month}";
            else
                performanceSummary = $"Cohorts {cohort1Month} and {cohort2Month} show similar performance";

            return new CohortComparisonDto
            {
                Cohort1 = cohort1Month,
                Cohort2 = cohort2Month,
                RetentionDifference = retentionDiff,
                RevenueDifference = revenueDiff,
                LtvDifference = ltvDiff,
                PerformanceSummary = performanceSummary
            };
        }
    }
}
