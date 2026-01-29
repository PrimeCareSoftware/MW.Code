using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for module configuration and access management
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleConfigController : BaseController
    {
        private readonly MedicSoftDbContext _context;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IModuleConfigurationService _moduleConfigService;

        public ModuleConfigController(
            ITenantContext tenantContext,
            MedicSoftDbContext context,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IModuleConfigurationService moduleConfigService) : base(tenantContext)
        {
            _context = context;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _moduleConfigService = moduleConfigService;
        }

        /// <summary>
        /// Get all available modules and their status for the clinic
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ModuleDto>>> GetModules()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Get subscription and plan
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return BadRequest(new { message = "No subscription found" });

            var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);
            if (plan == null)
                return BadRequest(new { message = "Invalid subscription plan" });

            // Get configured modules
            var configurations = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinicId && mc.TenantId == tenantId)
                .ToListAsync();

            var modules = SystemModules.GetAllModules();
            var result = modules.Select(moduleName =>
            {
                var config = configurations.FirstOrDefault(c => c.ModuleName == moduleName);
                var isAvailableInPlan = SystemModules.IsModuleAvailableInPlan(moduleName, plan);

                return new ModuleDto
                {
                    ModuleName = moduleName,
                    IsEnabled = config?.IsEnabled ?? false,
                    IsAvailableInPlan = isAvailableInPlan,
                    Configuration = config?.Configuration
                };
            });

            return Ok(result);
        }

        /// <summary>
        /// Enable a module for the clinic
        /// </summary>
        [HttpPost("{moduleName}/enable")]
        public async Task<ActionResult> EnableModule(string moduleName)
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                return BadRequest(new { message = "Invalid module name" });

            // Check if module is available in plan
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return BadRequest(new { message = "No subscription found" });

            var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);
            if (plan == null)
                return BadRequest(new { message = "Invalid subscription plan" });

            if (!SystemModules.IsModuleAvailableInPlan(moduleName, plan))
                return BadRequest(new { message = $"Module {moduleName} is not available in your current plan. Please upgrade." });

            // Get or create module configuration
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName && mc.TenantId == tenantId);

            if (config == null)
            {
                config = new ModuleConfiguration(clinicId, moduleName, tenantId, true);
                await _context.ModuleConfigurations.AddAsync(config);
            }
            else
            {
                config.Enable();
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = $"Module {moduleName} enabled successfully" });
        }

        /// <summary>
        /// Disable a module for the clinic
        /// </summary>
        [HttpPost("{moduleName}/disable")]
        public async Task<ActionResult> DisableModule(string moduleName)
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                return BadRequest(new { message = "Invalid module name" });

            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName && mc.TenantId == tenantId);

            if (config == null)
                return NotFound(new { message = "Module configuration not found" });

            config.Disable();
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Module {moduleName} disabled successfully" });
        }

        /// <summary>
        /// Update module configuration
        /// </summary>
        [HttpPut("{moduleName}/config")]
        public async Task<ActionResult> UpdateModuleConfig(string moduleName, [FromBody] UpdateConfigRequest request)
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                return BadRequest(new { message = "Invalid module name" });

            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName && mc.TenantId == tenantId);

            if (config == null)
            {
                config = new ModuleConfiguration(clinicId, moduleName, tenantId, true, request.Configuration);
                await _context.ModuleConfigurations.AddAsync(config);
            }
            else
            {
                config.UpdateConfiguration(request.Configuration);
            }

            await _context.SaveChangesAsync();

            return Ok(new { message = "Module configuration updated successfully" });
        }

        /// <summary>
        /// Get all available modules (system-wide)
        /// </summary>
        [HttpGet("available")]
        public ActionResult<IEnumerable<string>> GetAvailableModules()
        {
            return Ok(SystemModules.GetAllModules());
        }

        /// <summary>
        /// Get detailed information about all modules
        /// </summary>
        [HttpGet("info")]
        [ProducesResponseType(typeof(IEnumerable<ModuleInfoDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<ModuleInfoDto>> GetModulesInfo()
        {
            var modules = SystemModules.GetModulesInfo();
            var result = modules.Values.Select(m => new ModuleInfoDto
            {
                Name = m.Name,
                DisplayName = m.DisplayName,
                Description = m.Description,
                Category = m.Category,
                Icon = m.Icon,
                IsCore = m.IsCore,
                RequiredModules = m.RequiredModules,
                MinimumPlan = m.MinimumPlan.ToString()
            });

            return Ok(result);
        }

        /// <summary>
        /// Validate if a module can be enabled for a clinic
        /// </summary>
        [HttpPost("validate")]
        [ProducesResponseType(typeof(ValidationResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ValidationResponseDto>> ValidateModuleConfig(
            [FromBody] ValidateModuleRequest request)
        {
            var clinicId = GetClinicIdFromToken();
            var validation = await _moduleConfigService.ValidateModuleConfigAsync(clinicId, request.ModuleName);

            return Ok(new ValidationResponseDto
            {
                IsValid = validation.IsValid,
                ErrorMessage = validation.ErrorMessage
            });
        }

        /// <summary>
        /// Get module configuration history
        /// </summary>
        [HttpGet("{moduleName}/history")]
        [ProducesResponseType(typeof(IEnumerable<ModuleConfigHistoryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ModuleConfigHistoryDto>>> GetModuleHistory(string moduleName)
        {
            var clinicId = GetClinicIdFromToken();
            var history = await _moduleConfigService.GetModuleHistoryAsync(clinicId, moduleName);
            return Ok(history);
        }

        /// <summary>
        /// Enable module with reason (for audit)
        /// </summary>
        [HttpPost("{moduleName}/enable-with-reason")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> EnableModuleWithReason(
            string moduleName,
            [FromBody] EnableModuleRequest request)
        {
            var clinicId = GetClinicIdFromToken();
            var userId = User.FindFirst("sub")?.Value ?? "Unknown";

            await _moduleConfigService.EnableModuleAsync(clinicId, moduleName, userId, request.Reason);
            return Ok(new { message = $"Module {moduleName} enabled successfully" });
        }

        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
        }
    }

    public class ModuleDto
    {
        public string ModuleName { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        public bool IsAvailableInPlan { get; set; }
        public string? Configuration { get; set; }
    }

    public class UpdateConfigRequest
    {
        public string? Configuration { get; set; }
    }

    public class ValidateModuleRequest
    {
        public string ModuleName { get; set; } = string.Empty;
    }

    public class EnableModuleRequest
    {
        public string? Reason { get; set; }
    }

    public class ValidationResponseDto
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
