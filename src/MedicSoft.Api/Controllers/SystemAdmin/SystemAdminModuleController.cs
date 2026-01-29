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
    /// System Admin endpoints for global module configuration
    /// </summary>
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
        /// Get global module usage statistics
        /// </summary>
        [HttpGet("usage")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleUsageDto>>> GetGlobalModuleUsage()
        {
            var usage = await _moduleConfigService.GetGlobalModuleUsageAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Get module adoption rates across all clinics
        /// </summary>
        [HttpGet("adoption")]
        [ProducesResponseType(typeof(IEnumerable<ModuleAdoptionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleAdoptionDto>>> GetModuleAdoption()
        {
            var adoption = await _analyticsService.GetModuleAdoptionRatesAsync();
            return Ok(adoption);
        }

        /// <summary>
        /// Get module usage grouped by subscription plan
        /// </summary>
        [HttpGet("usage-by-plan")]
        [ProducesResponseType(typeof(IEnumerable<ModuleUsageByPlanDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleUsageByPlanDto>>> GetUsageByPlan()
        {
            var usage = await _analyticsService.GetUsageByPlanAsync();
            return Ok(usage);
        }

        /// <summary>
        /// Get module usage counts
        /// </summary>
        [HttpGet("counts")]
        [ProducesResponseType(typeof(Dictionary<string, int>), StatusCodes.Status200OK)]
        public async Task<ActionResult<Dictionary<string, int>>> GetModuleCounts()
        {
            var counts = await _analyticsService.GetModuleCountsAsync();
            return Ok(counts);
        }

        /// <summary>
        /// Enable module globally (for all clinics with appropriate plan)
        /// </summary>
        [HttpPost("{moduleName}/enable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EnableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _moduleConfigService.EnableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} enabled globally" });
        }

        /// <summary>
        /// Disable module globally (for all clinics)
        /// </summary>
        [HttpPost("{moduleName}/disable-globally")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> DisableModuleGlobally(string moduleName)
        {
            var userId = User.FindFirst("sub")?.Value ?? "System";
            await _moduleConfigService.DisableModuleGloballyAsync(moduleName, userId);
            return Ok(new { message = $"Module {moduleName} disabled globally" });
        }

        /// <summary>
        /// Get all clinics with a specific module enabled
        /// </summary>
        [HttpGet("{moduleName}/clinics")]
        [ProducesResponseType(typeof(IEnumerable<ClinicModuleDto>), StatusCodes.Status200OK)]
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
        /// Get usage statistics for a specific module
        /// </summary>
        [HttpGet("{moduleName}/stats")]
        [ProducesResponseType(typeof(ModuleUsageStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ModuleUsageStatsDto>> GetModuleStats(string moduleName)
        {
            var stats = await _analyticsService.GetModuleUsageStatsAsync(moduleName);
            return Ok(stats);
        }
    }
}
