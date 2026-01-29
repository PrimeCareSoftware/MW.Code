using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// System Admin endpoints for global module configuration, analytics, and management
    /// </summary>
    /// <remarks>
    /// This controller provides system-wide module management capabilities:
    /// - View adoption rates across all clinics
    /// - Enable/disable modules globally
    /// - Analyze usage by subscription plan
    /// - Access detailed statistics and metrics
    /// 
    /// **Authorization:** Requires SystemAdmin role
    /// </remarks>
    [ApiController]
    [Route("api/system-admin/modules")]
    [Authorize(Roles = "SystemAdmin")]
    public class SystemAdminModuleController : ControllerBase
    {
        private readonly IModuleConfigurationService _moduleConfigService;
        private readonly IModuleAnalyticsService _analyticsService;
        private readonly MedicSoftDbContext _context;

        public SystemAdminModuleController(
            IModuleConfigurationService moduleConfigService,
            IModuleAnalyticsService analyticsService,
            MedicSoftDbContext context)
        {
            _moduleConfigService = moduleConfigService;
            _analyticsService = analyticsService;
            _context = context;
        }

        /// <summary>
        /// Get global module usage statistics across all clinics
        /// </summary>
        /// <returns>List of modules with usage counts and percentages</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="401">Unauthorized - Requires SystemAdmin role</response>
        /// <response code="403">Forbidden - Insufficient permissions</response>
        /// <remarks>
        /// Provides overview of how many clinics are using each module.
        /// Useful for:
        /// - Identifying most/least popular modules
        /// - Planning feature development
        /// - Understanding user adoption patterns
        /// - Generating executive reports
        /// </remarks>
        [HttpGet("usage")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ModuleUsageDto>>> GetGlobalModuleUsage()
        {
            var usage = await _moduleConfigService.GetGlobalModuleUsageAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Get module adoption rates across all clinics
        /// </summary>
        /// <returns>Adoption percentages for each module</returns>
        /// <response code="200">Adoption rates calculated successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// Calculates adoption rate as: (Clinics using module / Total clinics) × 100
        /// 
        /// **Example Response:**
        /// ```json
        /// [
        ///   {
        ///     "moduleName": "PatientManagement",
        ///     "adoptionRate": 98.5,
        ///     "totalClinics": 150,
        ///     "clinicsUsing": 148
        ///   }
        /// ]
        /// ```
        /// 
        /// Use this to:
        /// - Track module performance over time
        /// - Identify modules needing promotion
        /// - Measure impact of marketing campaigns
        /// </remarks>
        [HttpGet("adoption")]
        [ProducesResponseType(typeof(IEnumerable<ModuleAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ModuleAdoptionDto>>> GetModuleAdoption()
        {
            var adoption = await _analyticsService.GetModuleAdoptionRatesAsync();
            return Ok(adoption);
        }

        /// <summary>
        /// Get module usage statistics grouped by subscription plan
        /// </summary>
        /// <returns>Usage breakdown per plan type</returns>
        /// <response code="200">Usage data retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// Shows how modules are being used across different subscription tiers.
        /// 
        /// **Use Cases:**
        /// - Validate that plan features are properly aligned
        /// - Identify opportunities for upsell (e.g., Premium modules with low adoption)
        /// - Detect plan misconfiguration
        /// - Analyze value perception by plan
        /// 
        /// **Example:** If WhatsApp shows 90% adoption in Enterprise but only 30% in Premium,
        /// this may indicate pricing issues or lack of awareness in Premium tier.
        /// </remarks>
        [HttpGet("usage-by-plan")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageByPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ModuleUsageByPlanDto>>> GetUsageByPlan()
        {
            var usage = await _analyticsService.GetUsageByPlanAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Get simple count of how many clinics use each module
        /// </summary>
        /// <returns>Dictionary mapping module names to usage counts</returns>
        /// <response code="200">Counts retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// Returns lightweight summary suitable for dashboards or quick checks.
        /// 
        /// **Example Response:**
        /// ```json
        /// {
        ///   "PatientManagement": 148,
        ///   "WhatsAppIntegration": 45,
        ///   "Reports": 112
        /// }
        /// ```
        /// 
        /// For detailed statistics, use `/modules/usage` or `/modules/{moduleName}/stats`
        /// </remarks>
        [HttpGet("counts")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<Dictionary<string, int>>> GetModuleCounts()
        {
            var counts = await _analyticsService.GetModuleCountsAsync();
            return Ok(counts);
        }

        /// <summary>
        /// Enable a module globally for all clinics that have it in their subscription plan
        /// </summary>
        /// <param name="moduleName">Name of the module to enable</param>
        /// <returns>Success message with count of affected clinics</returns>
        /// <response code="200">Module enabled globally</response>
        /// <response code="400">Invalid module name</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// ⚠️ **CAUTION:** This is a powerful operation that affects multiple clinics.
        /// 
        /// **What it does:**
        /// - Enables the module for ALL clinics where it's available in their plan
        /// - Does NOT enable for clinics where module is not in plan
        /// - Creates audit trail with system admin ID
        /// 
        /// **Use Cases:**
        /// - Rolling out a new module
        /// - Enabling a feature after maintenance
        /// - Promotional campaigns (e.g., "Try Premium modules free for 30 days")
        /// 
        /// **Best Practices:**
        /// 1. Communicate with clinics before enabling
        /// 2. Provide training materials
        /// 3. Monitor support tickets after rollout
        /// 4. Be ready to disable globally if critical issues arise
        /// 
        /// **Example:**
        /// ```
        /// POST /api/system-admin/modules/WhatsAppIntegration/enable-globally
        /// ```
        /// Result: Enables WhatsApp for ~45 clinics (Premium + Enterprise plans)
        /// </remarks>
        [HttpPost("{moduleName}/enable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> EnableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _moduleConfigService.EnableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} enabled globally" });
        }

        /// <summary>
        /// Disable a module globally for ALL clinics
        /// </summary>
        /// <param name="moduleName">Name of the module to disable</param>
        /// <returns>Success message with count of affected clinics</returns>
        /// <response code="200">Module disabled globally</response>
        /// <response code="400">Cannot disable Core modules or invalid module name</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// 🚨 **CRITICAL OPERATION:** This disables the module for ALL clinics regardless of plan.
        /// 
        /// **What it does:**
        /// - Immediately disables module for every clinic
        /// - Prevents Core modules from being disabled (safety check)
        /// - Records action in audit trail
        /// - Does NOT delete data (can be re-enabled later)
        /// 
        /// **When to Use:**
        /// - Critical bug discovered in module
        /// - Security vulnerability requires immediate action
        /// - Scheduled maintenance
        /// - Module deprecation
        /// 
        /// **Process:**
        /// 1. ✅ Notify all affected clinics (48h advance notice if possible)
        /// 2. ✅ Explain reason and expected duration
        /// 3. ✅ Provide alternative workflows if available
        /// 4. ⚠️ Execute disable
        /// 5. ✅ Monitor support channels
        /// 6. ✅ Communicate when issue is resolved
        /// 
        /// **Example:**
        /// ```
        /// POST /api/system-admin/modules/WhatsAppIntegration/disable-globally
        /// ```
        /// Result: Disables WhatsApp for ALL 150 clinics
        /// 
        /// **Rollback:** Use `/enable-globally` to restore
        /// </remarks>
        [HttpPost("{moduleName}/disable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult> DisableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _moduleConfigService.DisableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} disabled globally" });
        }

        /// <summary>
        /// Get list of all clinics that have a specific module enabled
        /// </summary>
        /// <param name="moduleName">Name of the module to query</param>
        /// <returns>List of clinics with module details</returns>
        /// <response code="200">Clinic list retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// Useful for:
        /// - Identifying which clinics use specific features
        /// - Targeted communication (e.g., announce WhatsApp updates to users)
        /// - Support escalation (contact clinics affected by module issue)
        /// - Usage analysis and case studies
        /// 
        /// **Example Response:**
        /// ```json
        /// [
        ///   {
        ///     "clinicId": "guid",
        ///     "clinicName": "Clínica São Paulo",
        ///     "isEnabled": true,
        ///     "configuration": "{\"sendReminders\": true}",
        ///     "updatedAt": "2026-01-15T10:30:00Z"
        ///   }
        /// ]
        /// ```
        /// 
        /// **Tip:** Export this data for email campaigns or support outreach
        /// </remarks>
        [HttpGet("{moduleName}/clinics")]
        [ProducesResponseType(typeof(IEnumerable<ClinicModuleDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<IEnumerable<ClinicModuleDto>>> GetClinicsWithModule(string moduleName)
        {
            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ModuleName == moduleName && mc.IsEnabled)
                .Include(mc => mc.Clinic)
                .ToListAsync();

            var result = configs.Select(mc => new ClinicModuleDto
            {
                ClinicId = mc.ClinicId,
                ClinicName = mc.Clinic?.Name ?? "Unknown",
                IsEnabled = mc.IsEnabled,
                Configuration = mc.Configuration,
                UpdatedAt = mc.UpdatedAt
            });

            return Ok(result);
        }

        /// <summary>
        /// Get detailed usage statistics for a specific module
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>Comprehensive statistics including trends and demographics</returns>
        /// <response code="200">Statistics retrieved successfully</response>
        /// <response code="404">Module not found</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="403">Forbidden</response>
        /// <remarks>
        /// Provides deep-dive analytics for a single module:
        /// - Total clinics using
        /// - Adoption rate percentage
        /// - Usage by subscription plan
        /// - Growth trend (last 30/60/90 days)
        /// - Average configuration settings
        /// - Top users/clinics
        /// 
        /// **Use Cases:**
        /// - Module performance review
        /// - Feature prioritization decisions
        /// - ROI analysis
        /// - Product roadmap planning
        /// - Quarterly business reviews
        /// 
        /// **Example Request:**
        /// ```
        /// GET /api/system-admin/modules/WhatsAppIntegration/stats
        /// ```
        /// 
        /// **Example Response:**
        /// ```json
        /// {
        ///   "moduleName": "WhatsAppIntegration",
        ///   "totalClinics": 45,
        ///   "adoptionRate": 30.0,
        ///   "growthLast30Days": 12.5,
        ///   "usageByPlan": {
        ///     "Premium": 20,
        ///     "Enterprise": 25
        ///   }
        /// }
        /// ```
        /// </remarks>
        [HttpGet("{moduleName}/stats")]
        [ProducesResponseType(typeof(ModuleUsageStatsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<ModuleUsageStatsDto>> GetModuleStats(string moduleName)
        {
            var stats = await _analyticsService.GetModuleUsageStatsAsync(moduleName);
            return Ok(stats);
        }
    }
}
