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
    [Route("api/module-config")]
    public class ModuleConfigController : BaseController
    {
        private readonly MedicSoftDbContext _context;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IModuleConfigurationService _moduleConfigService;
        private readonly IModuleConfigurationValidator _configValidator;

        public ModuleConfigController(
            ITenantContext tenantContext,
            MedicSoftDbContext context,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IModuleConfigurationService moduleConfigService,
            IModuleConfigurationValidator configValidator) : base(tenantContext)
        {
            _context = context;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _moduleConfigService = moduleConfigService;
            _configValidator = configValidator;
        }

        /// <summary>
        /// Get all available modules and their status for the authenticated clinic
        /// </summary>
        /// <returns>List of modules with their enabled status and availability in current plan</returns>
        /// <response code="200">Modules retrieved successfully</response>
        /// <response code="400">Invalid subscription or plan</response>
        /// <response code="401">Unauthorized - Invalid or missing JWT token</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ModuleConfigDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ModuleConfigDto>>> GetModules()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Get subscription and plan
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return BadRequest(new { message = "No subscription found" });

            // Subscription plans are system-wide entities with tenantId="system"
            // Use GetByIdWithoutTenantAsync to retrieve them
            var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
            if (plan == null)
                return BadRequest(new { message = "Invalid subscription plan" });

            // Get configured modules
            var configurations = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinicId && mc.TenantId == tenantId)
                .ToListAsync();

            // Get all modules with their metadata
            var modulesInfo = SystemModules.GetModulesInfo();
            var result = modulesInfo.Values.Select(moduleInfo =>
            {
                var config = configurations.FirstOrDefault(c => c.ModuleName == moduleInfo.Name);
                var isAvailableInPlan = SystemModules.IsModuleAvailableInPlan(moduleInfo.Name, plan);

                return new ModuleConfigDto
                {
                    ModuleName = moduleInfo.Name,
                    DisplayName = moduleInfo.DisplayName,
                    Description = moduleInfo.Description,
                    Category = moduleInfo.Category,
                    Icon = moduleInfo.Icon,
                    IsEnabled = config?.IsEnabled ?? false,
                    IsAvailableInPlan = isAvailableInPlan,
                    IsCore = moduleInfo.IsCore,
                    RequiredModules = moduleInfo.RequiredModules,
                    Configuration = config?.Configuration,
                    UpdatedAt = config?.UpdatedAt,
                    RequiresConfiguration = moduleInfo.RequiresConfiguration,
                    ConfigurationType = moduleInfo.ConfigurationType,
                    ConfigurationExample = moduleInfo.ConfigurationExample,
                    ConfigurationHelp = moduleInfo.ConfigurationHelp
                };
            });

            return Ok(result);
        }

        /// <summary>
        /// Enable a specific module for the authenticated clinic
        /// </summary>
        /// <param name="moduleName">Name of the module to enable (e.g., "PatientManagement", "WhatsAppIntegration")</param>
        /// <returns>Success message if module was enabled</returns>
        /// <response code="200">Module enabled successfully</response>
        /// <response code="400">Module not available in plan or invalid module name</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/ModuleConfig/WhatsAppIntegration/enable
        ///     
        /// The module must be available in the clinic's subscription plan.
        /// Core modules are always available and cannot be disabled.
        /// </remarks>
        [HttpPost("{moduleName}/enable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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

            // Subscription plans are system-wide entities with tenantId="system"
            // Use GetByIdWithoutTenantAsync to retrieve them
            var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
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
        /// Disable a specific module for the authenticated clinic
        /// </summary>
        /// <param name="moduleName">Name of the module to disable</param>
        /// <returns>Success message if module was disabled</returns>
        /// <response code="200">Module disabled successfully</response>
        /// <response code="400">Invalid module name</response>
        /// <response code="404">Module configuration not found</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// Core modules cannot be disabled as they are essential for system operation.
        /// Disabling a module only affects visibility and access, data is preserved.
        /// </remarks>
        [HttpPost("{moduleName}/disable")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// Update module-specific configuration settings
        /// </summary>
        /// <param name="moduleName">Name of the module to configure</param>
        /// <param name="request">Configuration data in JSON format</param>
        /// <returns>Success message if configuration was updated</returns>
        /// <response code="200">Configuration updated successfully</response>
        /// <response code="400">Invalid module name or configuration format</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// Sample request:
        /// 
        ///     PUT /api/ModuleConfig/WhatsAppIntegration/config
        ///     {
        ///       "configuration": "{\"sendReminders\": true, \"hoursBeforeAppointment\": 24}"
        ///     }
        ///     
        /// Configuration format varies by module. See module documentation for specific parameters.
        /// </remarks>
        [HttpPut("{moduleName}/config")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// Get list of all module names available in the system
        /// </summary>
        /// <returns>Array of module names</returns>
        /// <response code="200">Module list retrieved successfully</response>
        /// <remarks>
        /// Returns simple list of module identifiers. Use /api/ModuleConfig/info for detailed information.
        /// </remarks>
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<string>> GetAvailableModules()
        {
            return Ok(SystemModules.GetAllModules());
        }

        /// <summary>
        /// Get detailed information about all available modules
        /// </summary>
        /// <returns>List of modules with complete metadata</returns>
        /// <response code="200">Module information retrieved successfully</response>
        /// <remarks>
        /// Returns comprehensive information including:
        /// - Display name and description
        /// - Category (Core, Advanced, Premium, Analytics)
        /// - Dependencies and requirements
        /// - Minimum subscription plan required
        /// - Icon for UI display
        /// </remarks>
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
                MinimumPlan = m.MinimumPlan.ToString(),
                RequiresConfiguration = m.RequiresConfiguration,
                ConfigurationType = m.ConfigurationType,
                ConfigurationExample = m.ConfigurationExample,
                ConfigurationHelp = m.ConfigurationHelp
            });

            return Ok(result);
        }

        /// <summary>
        /// Validate if a module can be enabled for the authenticated clinic
        /// </summary>
        /// <param name="request">Validation request containing module name</param>
        /// <returns>Validation result with error message if invalid</returns>
        /// <response code="200">Validation completed (check IsValid property)</response>
        /// <remarks>
        /// Performs validation checks including:
        /// - Module exists in system
        /// - Module available in clinic's subscription plan
        /// - All required dependencies are satisfied
        /// - Subscription plan limits not exceeded
        /// 
        /// Sample request:
        /// 
        ///     POST /api/ModuleConfig/validate
        ///     {
        ///       "moduleName": "WhatsAppIntegration"
        ///     }
        /// </remarks>
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
        /// Get change history for a specific module
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>List of historical changes with timestamps and users</returns>
        /// <response code="200">History retrieved successfully</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// Returns audit trail including:
        /// - When module was enabled/disabled
        /// - Configuration changes
        /// - User who made the change
        /// - Previous and new values
        /// 
        /// Useful for compliance and troubleshooting.
        /// </remarks>
        [HttpGet("{moduleName}/history")]
        [ProducesResponseType(typeof(IEnumerable<ModuleConfigHistoryDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ModuleConfigHistoryDto>>> GetModuleHistory(string moduleName)
        {
            var clinicId = GetClinicIdFromToken();
            var history = await _moduleConfigService.GetModuleHistoryAsync(clinicId, moduleName);
            return Ok(history);
        }

        /// <summary>
        /// Enable a module with audit reason
        /// </summary>
        /// <param name="moduleName">Name of the module to enable</param>
        /// <param name="request">Request containing optional reason for audit</param>
        /// <returns>Success message</returns>
        /// <response code="200">Module enabled successfully</response>
        /// <response code="400">Module not available or invalid</response>
        /// <response code="401">Unauthorized</response>
        /// <remarks>
        /// Similar to /enable but includes audit reason for compliance.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/ModuleConfig/WhatsAppIntegration/enable-with-reason
        ///     {
        ///       "reason": "Cliente solicitou envio de lembretes via WhatsApp"
        ///     }
        /// 
        /// Use this endpoint when you need detailed audit trail.
        /// </remarks>
        [HttpPost("{moduleName}/enable-with-reason")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult> EnableModuleWithReason(
            string moduleName,
            [FromBody] EnableModuleRequest request)
        {
            var clinicId = GetClinicIdFromToken();
            var userId = User.FindFirst("sub")?.Value ?? "Unknown";

            await _moduleConfigService.EnableModuleAsync(clinicId, moduleName, userId, request.Reason);
            return Ok(new { message = $"Module {moduleName} enabled successfully" });
        }

        /// <summary>
        /// Validate module configuration JSON
        /// </summary>
        /// <param name="request">Request containing module name and configuration JSON</param>
        /// <returns>Validation result with errors if any</returns>
        /// <response code="200">Validation completed</response>
        /// <remarks>
        /// Validates configuration JSON against module-specific schema.
        /// Returns detailed validation errors for each field.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/ModuleConfig/validate-configuration
        ///     {
        ///       "moduleName": "WhatsAppIntegration",
        ///       "configurationJson": "{\"apiKey\": \"test\", \"phoneNumber\": \"+5511999999999\"}"
        ///     }
        /// </remarks>
        [HttpPost("validate-configuration")]
        [ProducesResponseType(typeof(ConfigurationValidationResult), StatusCodes.Status200OK)]
        public ActionResult<ConfigurationValidationResult> ValidateConfiguration(
            [FromBody] ValidateConfigurationRequest request)
        {
            var result = _configValidator.ValidateConfiguration(request.ModuleName, request.ConfigurationJson);
            return Ok(result);
        }

        /// <summary>
        /// Get default configuration for a module
        /// </summary>
        /// <param name="moduleName">Name of the module</param>
        /// <returns>Default configuration JSON</returns>
        /// <response code="200">Default configuration returned</response>
        /// <response code="404">Module not found or doesn't require configuration</response>
        /// <remarks>
        /// Returns a default configuration template with recommended values.
        /// Use this as a starting point when configuring a module.
        /// </remarks>
        [HttpGet("{moduleName}/default-configuration")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<string> GetDefaultConfiguration(string moduleName)
        {
            var defaultConfig = _configValidator.GetDefaultConfiguration(moduleName);
            
            if (defaultConfig == null)
            {
                return NotFound(new { message = $"Module {moduleName} does not require configuration or was not found" });
            }

            return Ok(new { moduleName, configuration = defaultConfig });
        }

        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
        }
    }

    /// <summary>
    /// Data transfer object for module information
    /// </summary>
    public class ModuleDto
    {
        /// <summary>
        /// Technical name of the module
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;
        
        /// <summary>
        /// Whether the module is currently enabled for this clinic
        /// </summary>
        public bool IsEnabled { get; set; }
        
        /// <summary>
        /// Whether the module is available in the clinic's subscription plan
        /// </summary>
        public bool IsAvailableInPlan { get; set; }
        
        /// <summary>
        /// JSON configuration string for module-specific settings
        /// </summary>
        public string? Configuration { get; set; }
    }

    /// <summary>
    /// Request object for updating module configuration
    /// </summary>
    public class UpdateConfigRequest
    {
        /// <summary>
        /// JSON string containing module configuration parameters
        /// </summary>
        public string? Configuration { get; set; }
    }

    /// <summary>
    /// Request object for validating module availability
    /// </summary>
    public class ValidateModuleRequest
    {
        /// <summary>
        /// Name of the module to validate
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request object for enabling module with audit reason
    /// </summary>
    public class EnableModuleRequest
    {
        /// <summary>
        /// Optional reason for enabling the module (for audit trail)
        /// </summary>
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Response object for module validation
    /// </summary>
    public class ValidationResponseDto
    {
        /// <summary>
        /// Whether the module can be enabled
        /// </summary>
        public bool IsValid { get; set; }
        
        /// <summary>
        /// Error message if validation failed
        /// </summary>
        public string ErrorMessage { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request object for validating module configuration
    /// </summary>
    public class ValidateConfigurationRequest
    {
        /// <summary>
        /// Name of the module
        /// </summary>
        public string ModuleName { get; set; } = string.Empty;

        /// <summary>
        /// Configuration JSON to validate
        /// </summary>
        public string? ConfigurationJson { get; set; }
    }
}
