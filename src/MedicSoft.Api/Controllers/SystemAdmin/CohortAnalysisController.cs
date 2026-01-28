using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Cohorts;
using MedicSoft.Application.Services.Cohorts;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for cohort analysis
    /// Phase 3: Analytics and BI
    /// </summary>
    [ApiController]
    [Route("api/system-admin/cohorts")]
    [Authorize(Roles = "SystemAdmin")]
    public class CohortAnalysisController : BaseController
    {
        private readonly ICohortAnalysisService _cohortAnalysisService;

        public CohortAnalysisController(
            ITenantContext tenantContext,
            ICohortAnalysisService cohortAnalysisService) : base(tenantContext)
        {
            _cohortAnalysisService = cohortAnalysisService;
        }

        /// <summary>
        /// Get retention cohort analysis
        /// Shows customer retention over time for each cohort
        /// </summary>
        /// <param name="monthsBack">Number of months to analyze (default: 12)</param>
        [HttpGet("retention")]
        public async Task<ActionResult<RetentionAnalysisDto>> GetRetentionAnalysis([FromQuery] int monthsBack = 12)
        {
            var analysis = await _cohortAnalysisService.GetRetentionAnalysisAsync(monthsBack);
            return Ok(analysis);
        }

        /// <summary>
        /// Get revenue cohort analysis
        /// Shows MRR and LTV metrics for each cohort
        /// </summary>
        /// <param name="monthsBack">Number of months to analyze (default: 12)</param>
        [HttpGet("revenue")]
        public async Task<ActionResult<RevenueCohortAnalysisDto>> GetRevenueCohortAnalysis([FromQuery] int monthsBack = 12)
        {
            var analysis = await _cohortAnalysisService.GetRevenueCohortAnalysisAsync(monthsBack);
            return Ok(analysis);
        }

        /// <summary>
        /// Get comprehensive churn analysis
        /// Includes churn rates, trends, and top reasons
        /// </summary>
        /// <param name="monthsBack">Number of months to analyze (default: 12)</param>
        [HttpGet("churn")]
        public async Task<ActionResult<ComprehensiveChurnAnalysisDto>> GetChurnAnalysis([FromQuery] int monthsBack = 12)
        {
            var analysis = await _cohortAnalysisService.GetChurnAnalysisAsync(monthsBack);
            return Ok(analysis);
        }

        /// <summary>
        /// Compare two cohorts
        /// Useful for analyzing impact of changes or campaigns
        /// </summary>
        /// <param name="cohort1">First cohort month (format: yyyy-MM)</param>
        /// <param name="cohort2">Second cohort month (format: yyyy-MM)</param>
        [HttpGet("compare")]
        public async Task<ActionResult<CohortComparisonDto>> CompareCohorts(
            [FromQuery] string cohort1,
            [FromQuery] string cohort2)
        {
            if (string.IsNullOrEmpty(cohort1) || string.IsNullOrEmpty(cohort2))
            {
                return BadRequest("Both cohort1 and cohort2 parameters are required");
            }

            var comparison = await _cohortAnalysisService.CompareCohort(cohort1, cohort2);
            return Ok(comparison);
        }
    }
}
