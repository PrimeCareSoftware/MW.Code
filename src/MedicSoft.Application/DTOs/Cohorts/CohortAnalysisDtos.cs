using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.Cohorts
{
    /// <summary>
    /// DTO for retention cohort analysis
    /// </summary>
    public class RetentionCohortDto
    {
        public string CohortMonth { get; set; }
        public int CohortSize { get; set; }
        public List<decimal> RetentionRates { get; set; } = new();
        public List<int> RetainedCounts { get; set; } = new();
    }

    /// <summary>
    /// DTO for retention analysis response
    /// </summary>
    public class RetentionAnalysisDto
    {
        public List<RetentionCohortDto> Cohorts { get; set; } = new();
        public List<string> Months { get; set; } = new();
        public decimal AverageRetentionMonth1 { get; set; }
        public decimal AverageRetentionMonth3 { get; set; }
        public decimal AverageRetentionMonth6 { get; set; }
        public decimal AverageRetentionMonth12 { get; set; }
    }

    /// <summary>
    /// DTO for revenue cohort analysis
    /// </summary>
    public class RevenueCohortDto
    {
        public string CohortMonth { get; set; }
        public int CohortSize { get; set; }
        public List<decimal> MrrByMonth { get; set; } = new();
        public List<decimal> CumulativeRevenue { get; set; } = new();
        public decimal LifetimeValue { get; set; }
        public decimal AverageRevenuePerCustomer { get; set; }
    }

    /// <summary>
    /// DTO for revenue cohort analysis response
    /// </summary>
    public class RevenueCohortAnalysisDto
    {
        public List<RevenueCohortDto> Cohorts { get; set; } = new();
        public List<string> Months { get; set; } = new();
        public decimal AverageLTV { get; set; }
        public decimal TotalMRR { get; set; }
        public decimal MonthOverMonthGrowth { get; set; }
    }

    /// <summary>
    /// DTO for churn analysis
    /// </summary>
    public class ChurnAnalysisDto
    {
        public string Month { get; set; }
        public int StartingCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
        public int EndingCustomers { get; set; }
        public decimal ChurnRate { get; set; }
        public decimal GrowthRate { get; set; }
        public decimal NetRetentionRate { get; set; }
    }

    /// <summary>
    /// DTO for comprehensive churn analysis response
    /// </summary>
    public class ComprehensiveChurnAnalysisDto
    {
        public List<ChurnAnalysisDto> MonthlyChurn { get; set; } = new();
        public decimal AverageChurnRate { get; set; }
        public decimal CurrentChurnRate { get; set; }
        public string ChurnTrend { get; set; } // "improving", "stable", "worsening"
        public List<string> TopChurnReasons { get; set; } = new();
    }

    /// <summary>
    /// DTO for cohort comparison
    /// </summary>
    public class CohortComparisonDto
    {
        public string Cohort1 { get; set; }
        public string Cohort2 { get; set; }
        public decimal RetentionDifference { get; set; }
        public decimal RevenueDifference { get; set; }
        public decimal LtvDifference { get; set; }
        public string PerformanceSummary { get; set; }
    }

    /// <summary>
    /// DTO for cohort request parameters
    /// </summary>
    public class CohortAnalysisRequestDto
    {
        public int MonthsBack { get; set; } = 12;
        public string CohortType { get; set; } // "retention", "revenue", "churn"
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
