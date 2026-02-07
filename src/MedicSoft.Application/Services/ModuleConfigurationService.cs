using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public interface IModuleConfigurationService
    {
        // Configuration per Clinic
        Task<ModuleConfigDto> GetModuleConfigAsync(Guid clinicId, string moduleName);
        Task<IEnumerable<ModuleConfigDto>> GetAllModuleConfigsAsync(Guid clinicId);
        Task EnableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null);
        Task DisableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null);
        Task UpdateModuleConfigAsync(Guid clinicId, string moduleName, string configuration, string userId);

        // Global Configuration (System Admin)
        Task<IEnumerable<ModuleUsageDto>> GetGlobalModuleUsageAsync();
        Task EnableModuleGloballyAsync(string moduleName, string userId);
        Task DisableModuleGloballyAsync(string moduleName, string userId);
        Task<IEnumerable<ModuleConfigHistoryDto>> GetModuleHistoryAsync(Guid clinicId, string moduleName);

        // Validations
        Task<bool> CanEnableModuleAsync(Guid clinicId, string moduleName);
        Task<bool> HasRequiredModulesAsync(Guid clinicId, string moduleName);
        Task<ModuleValidationResult> ValidateModuleConfigAsync(Guid clinicId, string moduleName);
    }

    public class ModuleConfigurationService : IModuleConfigurationService
    {
        private readonly MedicSoftDbContext _context;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly ILogger<ModuleConfigurationService> _logger;

        public ModuleConfigurationService(
            MedicSoftDbContext context,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            ILogger<ModuleConfigurationService> logger)
        {
            _context = context;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _logger = logger;
        }

        public async Task<ModuleConfigDto> GetModuleConfigAsync(Guid clinicId, string moduleName)
        {
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);

            // Get tenant ID from clinic
            var clinic = await _context.Clinics.FindAsync(clinicId);
            var tenantId = clinic?.TenantId ?? string.Empty;

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            var isAvailableInPlan = false;

            if (subscription != null)
            {
                // Subscription plans are system-wide entities with tenantId="system"
                // Use GetByIdWithoutTenantAsync to retrieve them
                var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
                isAvailableInPlan = plan != null && SystemModules.IsModuleAvailableInPlan(moduleName, plan);
            }

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
                UpdatedAt = config?.UpdatedAt
            };
        }

        public async Task<IEnumerable<ModuleConfigDto>> GetAllModuleConfigsAsync(Guid clinicId)
        {
            var modules = SystemModules.GetAllModules();
            
            // Get clinic and subscription info
            var clinic = await _context.Clinics.FindAsync(clinicId);
            var tenantId = clinic?.TenantId ?? string.Empty;
            
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            SubscriptionPlan? plan = null;
            if (subscription != null)
            {
                // Subscription plans are system-wide entities with tenantId="system"
                // Use GetByIdWithoutTenantAsync to retrieve them
                plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
            }

            // Fetch all module configurations for this clinic in one query
            var configurations = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinicId)
                .ToListAsync();

            // Build result combining module info with configurations
            var result = new List<ModuleConfigDto>();
            foreach (var moduleName in modules)
            {
                var moduleInfo = SystemModules.GetModuleInfo(moduleName);
                var config = configurations.FirstOrDefault(c => c.ModuleName == moduleName);
                var isAvailableInPlan = plan != null && SystemModules.IsModuleAvailableInPlan(moduleName, plan);

                result.Add(new ModuleConfigDto
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
                    UpdatedAt = config?.UpdatedAt
                });
            }

            return result;
        }

        public async Task EnableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null)
        {
            // 1. Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            // 2. Validate module is available in plan
            var validation = await ValidateModuleConfigAsync(clinicId, moduleName);
            if (!validation.IsValid)
                throw new InvalidOperationException(validation.ErrorMessage);

            // 3. Check required modules
            if (!await HasRequiredModulesAsync(clinicId, moduleName))
                throw new InvalidOperationException("Required modules are not enabled");

            // 4. Get clinic to determine tenant ID
            var clinic = await _context.Clinics.FindAsync(clinicId);
            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            // 5. Enable module
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);

            if (config == null)
            {
                config = new ModuleConfiguration(clinicId, moduleName, clinic.TenantId, true);
                await _context.ModuleConfigurations.AddAsync(config);
            }
            else
            {
                config.Enable();
            }

            await _context.SaveChangesAsync();

            // 6. Register history
            var history = new ModuleConfigurationHistory(
                config.Id,
                clinicId,
                moduleName,
                "Enabled",
                userId,
                clinic.TenantId,
                previousConfig: null,
                newConfig: null,
                reason: reason
            );
            await _context.ModuleConfigurationHistories.AddAsync(history);
            
            // Save both changes in a single transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Module {moduleName} enabled for clinic {clinicId} by user {userId}");
        }

        public async Task DisableModuleAsync(Guid clinicId, string moduleName, string userId, string? reason = null)
        {
            // 1. Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            // 2. Check if module is core (cannot be disabled)
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);
            if (moduleInfo.IsCore)
                throw new InvalidOperationException("Core modules cannot be disabled");

            // 3. Get clinic to determine tenant ID
            var clinic = await _context.Clinics.FindAsync(clinicId);
            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            // 4. Disable module
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);

            if (config == null)
                throw new InvalidOperationException("Module configuration not found");

            var previousConfig = config.Configuration;
            config.Disable();

            // 5. Register history
            var history = new ModuleConfigurationHistory(
                config.Id,
                clinicId,
                moduleName,
                "Disabled",
                userId,
                clinic.TenantId,
                previousConfig: previousConfig,
                newConfig: null,
                reason: reason
            );
            await _context.ModuleConfigurationHistories.AddAsync(history);
            
            // Save both changes in a single transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Module {moduleName} disabled for clinic {clinicId} by user {userId}");
        }

        public async Task UpdateModuleConfigAsync(Guid clinicId, string moduleName, string configuration, string userId)
        {
            // 1. Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            // 2. Get clinic to determine tenant ID
            var clinic = await _context.Clinics.FindAsync(clinicId);
            if (clinic == null)
                throw new InvalidOperationException("Clinic not found");

            // 3. Update configuration
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);

            string? previousConfig = null;

            if (config == null)
            {
                config = new ModuleConfiguration(clinicId, moduleName, clinic.TenantId, true, configuration);
                await _context.ModuleConfigurations.AddAsync(config);
            }
            else
            {
                previousConfig = config.Configuration;
                config.UpdateConfiguration(configuration);
            }

            // 4. Register history
            var history = new ModuleConfigurationHistory(
                config.Id,
                clinicId,
                moduleName,
                "ConfigUpdated",
                userId,
                clinic.TenantId,
                previousConfig: previousConfig,
                newConfig: configuration,
                reason: null
            );
            await _context.ModuleConfigurationHistories.AddAsync(history);
            
            // Save both changes in a single transaction
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Module {moduleName} configuration updated for clinic {clinicId} by user {userId}");
        }

        public async Task<IEnumerable<ModuleUsageDto>> GetGlobalModuleUsageAsync()
        {
            var modules = SystemModules.GetAllModules();
            var totalClinics = await _context.Clinics.CountAsync();

            // Fetch all module usage in a single query
            var moduleUsage = await _context.ModuleConfigurations
                .Where(mc => mc.IsEnabled)
                .GroupBy(mc => mc.ModuleName)
                .Select(g => new { ModuleName = g.Key, Count = g.Select(x => x.ClinicId).Distinct().Count() })
                .ToListAsync();

            var usageMap = moduleUsage.ToDictionary(x => x.ModuleName, x => x.Count);

            var result = new List<ModuleUsageDto>();
            foreach (var moduleName in modules)
            {
                var moduleInfo = SystemModules.GetModuleInfo(moduleName);
                var clinicsWithModule = usageMap.GetValueOrDefault(moduleName, 0);

                result.Add(new ModuleUsageDto
                {
                    ModuleName = moduleName,
                    DisplayName = moduleInfo.DisplayName,
                    TotalClinics = totalClinics,
                    ClinicsWithModuleEnabled = clinicsWithModule,
                    AdoptionRate = totalClinics > 0 ? (decimal)clinicsWithModule / totalClinics * 100 : 0,
                    Category = moduleInfo.Category
                });
            }

            return result.OrderByDescending(m => m.AdoptionRate);
        }

        public async Task EnableModuleGloballyAsync(string moduleName, string userId)
        {
            // Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            var clinics = await _context.Clinics.ToListAsync();

            foreach (var clinic in clinics)
            {
                try
                {
                    await EnableModuleAsync(clinic.Id, moduleName, userId, "Enabled globally by system admin");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to enable module {moduleName} for clinic {clinic.Id}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Module {moduleName} enabled globally by user {userId}");
        }

        public async Task DisableModuleGloballyAsync(string moduleName, string userId)
        {
            // Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                throw new ArgumentException($"Module {moduleName} not found");

            // Check if module is core
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);
            if (moduleInfo.IsCore)
                throw new InvalidOperationException("Core modules cannot be disabled");

            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ModuleName == moduleName && mc.IsEnabled)
                .ToListAsync();

            foreach (var config in configs)
            {
                try
                {
                    await DisableModuleAsync(config.ClinicId, moduleName, userId, "Disabled globally by system admin");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning($"Failed to disable module {moduleName} for clinic {config.ClinicId}: {ex.Message}");
                }
            }

            _logger.LogInformation($"Module {moduleName} disabled globally by user {userId}");
        }

        public async Task<IEnumerable<ModuleConfigHistoryDto>> GetModuleHistoryAsync(Guid clinicId, string moduleName)
        {
            var history = await _context.ModuleConfigurationHistories
                .Where(h => h.ClinicId == clinicId && h.ModuleName == moduleName)
                .OrderByDescending(h => h.ChangedAt)
                .ToListAsync();

            return history.Select(h => new ModuleConfigHistoryDto
            {
                Id = h.Id,
                ModuleName = h.ModuleName,
                Action = h.Action,
                ChangedBy = h.ChangedBy,
                ChangedAt = h.ChangedAt,
                Reason = h.Reason,
                PreviousConfiguration = h.PreviousConfiguration,
                NewConfiguration = h.NewConfiguration
            });
        }

        public async Task<bool> CanEnableModuleAsync(Guid clinicId, string moduleName)
        {
            var validation = await ValidateModuleConfigAsync(clinicId, moduleName);
            return validation.IsValid;
        }

        public async Task<bool> HasRequiredModulesAsync(Guid clinicId, string moduleName)
        {
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);

            if (moduleInfo.RequiredModules.Length == 0)
                return true;

            foreach (var requiredModule in moduleInfo.RequiredModules)
            {
                var config = await _context.ModuleConfigurations
                    .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == requiredModule);

                if (config == null || !config.IsEnabled)
                    return false;
            }

            return true;
        }

        public async Task<ModuleValidationResult> ValidateModuleConfigAsync(Guid clinicId, string moduleName)
        {
            // 1. Validate module exists
            if (!SystemModules.GetAllModules().Contains(moduleName))
                return new ModuleValidationResult(false, "Module not found");

            // 2. Get module information
            var moduleInfo = SystemModules.GetModuleInfo(moduleName);

            // 3. Get clinic subscription
            var clinic = await _context.Clinics.FindAsync(clinicId);
            var tenantId = clinic?.TenantId ?? string.Empty;

            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return new ModuleValidationResult(false, "Clinic has no active subscription");

            // Subscription plans are system-wide entities with tenantId="system"
            // Use GetByIdWithoutTenantAsync to retrieve them
            var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
            if (plan == null)
                return new ModuleValidationResult(false, "Invalid subscription plan");

            // 4. Check if module is available in plan
            if (!plan.HasModule(moduleName))
                return new ModuleValidationResult(false, $"Module {moduleName} not available in current plan. Please upgrade.");

            // 5. Check minimum plan requirement
            if (plan.Type < moduleInfo.MinimumPlan)
                return new ModuleValidationResult(false, $"Module requires at least {moduleInfo.MinimumPlan} plan");

            return new ModuleValidationResult(true);
        }
    }

    public class ModuleValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        public ModuleValidationResult(bool isValid, string errorMessage = "")
        {
            IsValid = isValid;
            ErrorMessage = errorMessage;
        }
    }
}
