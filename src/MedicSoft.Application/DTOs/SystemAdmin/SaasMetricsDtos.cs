using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SystemAdmin
{
    public class SaasDashboardDto
    {
        public decimal Mrr { get; set; }
        public decimal Arr { get; set; }
        public int ActiveCustomers { get; set; }
        public int NewCustomers { get; set; }
        public int ChurnedCustomers { get; set; }
        public decimal ChurnRate { get; set; }
        public decimal Arpu { get; set; }
        public decimal Ltv { get; set; }
        public decimal Cac { get; set; }
        public decimal LtvCacRatio { get; set; }
        public decimal MrrGrowthMoM { get; set; }
        public decimal GrowthRateYoY { get; set; }
        public decimal QuickRatio { get; set; }
        public string MrrTrend { get; set; } = "stable";
        public int TrialCustomers { get; set; }
        public int AtRiskCustomers { get; set; }
    }

    public class MrrBreakdownDto
    {
        public decimal TotalMrr { get; set; }
        public decimal NewMrr { get; set; }
        public decimal ExpansionMrr { get; set; }
        public decimal ContractionMrr { get; set; }
        public decimal ChurnedMrr { get; set; }
        public decimal NetNewMrr { get; set; }
    }

    public class ChurnAnalysisDto
    {
        public decimal RevenueChurnRate { get; set; }
        public decimal CustomerChurnRate { get; set; }
        public decimal MonthlyRevenueChurn { get; set; }
        public decimal MonthlyCustomerChurn { get; set; }
        public decimal AnnualRevenueChurn { get; set; }
        public decimal AnnualCustomerChurn { get; set; }
        public List<ChurnDataPoint> ChurnHistory { get; set; } = new();
    }

    public class ChurnDataPoint
    {
        public string Month { get; set; } = string.Empty;
        public decimal RevenueChurn { get; set; }
        public decimal CustomerChurn { get; set; }
        public int ChurnedCount { get; set; }
    }

    public class GrowthMetricsDto
    {
        public decimal MoMGrowthRate { get; set; }
        public decimal YoYGrowthRate { get; set; }
        public decimal QuickRatio { get; set; }
        public decimal TrialConversionRate { get; set; }
        public List<GrowthDataPoint> GrowthHistory { get; set; } = new();
    }

    public class GrowthDataPoint
    {
        public string Month { get; set; } = string.Empty;
        public decimal GrowthRate { get; set; }
        public decimal Mrr { get; set; }
    }

    public class RevenueTimelineDto
    {
        public string Month { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public decimal TotalMrr { get; set; }
        public decimal NewMrr { get; set; }
        public decimal ExpansionMrr { get; set; }
        public decimal ContractionMrr { get; set; }
        public decimal ChurnedMrr { get; set; }
        public int ActiveCustomers { get; set; }
    }

    public class CustomerBreakdownDto
    {
        public Dictionary<string, int> ByPlan { get; set; } = new();
        public Dictionary<string, int> ByStatus { get; set; } = new();
    }
}
