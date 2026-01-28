using System.Threading.Tasks;
using MedicSoft.Application.DTOs.Cohorts;

namespace MedicSoft.Application.Services.Cohorts
{
    /// <summary>
    /// Interface for cohort analysis service
    /// Phase 3: Analytics and BI
    /// </summary>
    public interface ICohortAnalysisService
    {
        /// <summary>
        /// Get retention cohort analysis
        /// Shows customer retention over time for each cohort
        /// </summary>
        Task<RetentionAnalysisDto> GetRetentionAnalysisAsync(int monthsBack = 12);

        /// <summary>
        /// Get revenue cohort analysis
        /// Shows MRR and LTV metrics for each cohort
        /// </summary>
        Task<RevenueCohortAnalysisDto> GetRevenueCohortAnalysisAsync(int monthsBack = 12);

        /// <summary>
        /// Get comprehensive churn analysis
        /// Includes churn rates, trends, and top reasons
        /// </summary>
        Task<ComprehensiveChurnAnalysisDto> GetChurnAnalysisAsync(int monthsBack = 12);

        /// <summary>
        /// Compare two cohorts
        /// Useful for analyzing impact of changes or campaigns
        /// </summary>
        Task<CohortComparisonDto> CompareCohort(string cohort1Month, string cohort2Month);
    }
}
