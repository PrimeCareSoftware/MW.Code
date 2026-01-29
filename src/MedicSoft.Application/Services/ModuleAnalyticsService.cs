using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public interface IModuleAnalyticsService
    {
        Task<ModuleUsageStatsDto> GetModuleUsageStatsAsync(string moduleName);
        Task<IEnumerable<ModuleAdoptionDto>> GetModuleAdoptionRatesAsync();
        Task<IEnumerable<ModuleUsageByPlanDto>> GetUsageByPlanAsync();
        Task<Dictionary<string, int>> GetModuleCountsAsync();
    }

    public class ModuleAnalyticsService : IModuleAnalyticsService
    {
        private readonly MedicSoftDbContext _context;

        public ModuleAnalyticsService(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<ModuleUsageStatsDto> GetModuleUsageStatsAsync(string moduleName)
        {
            var totalClinics = await _context.Clinics.CountAsync();
            var clinicsWithModule = await _context.ModuleConfigurations
                .Where(mc => mc.ModuleName == moduleName && mc.IsEnabled)
                .Select(mc => mc.ClinicId)
                .Distinct()
                .CountAsync();

            return new ModuleUsageStatsDto
            {
                ModuleName = moduleName,
                TotalClinics = totalClinics,
                ClinicsWithModuleEnabled = clinicsWithModule,
                AdoptionRate = totalClinics > 0 ? (decimal)clinicsWithModule / totalClinics * 100 : 0
            };
        }

        public async Task<IEnumerable<ModuleAdoptionDto>> GetModuleAdoptionRatesAsync()
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

            var result = new List<ModuleAdoptionDto>();
            foreach (var module in modules)
            {
                var moduleInfo = SystemModules.GetModuleInfo(module);
                var enabledCount = usageMap.GetValueOrDefault(module, 0);

                result.Add(new ModuleAdoptionDto
                {
                    ModuleName = module,
                    DisplayName = moduleInfo.DisplayName,
                    AdoptionRate = totalClinics > 0 ? (decimal)enabledCount / totalClinics * 100 : 0,
                    EnabledCount = enabledCount
                });
            }

            return result.OrderByDescending(r => r.AdoptionRate);
        }

        public async Task<IEnumerable<ModuleUsageByPlanDto>> GetUsageByPlanAsync()
        {
            var result = new List<ModuleUsageByPlanDto>();
            var modules = SystemModules.GetAllModules();

            // Get all subscriptions with plans
            var subscriptions = await _context.ClinicSubscriptions
                .Include(s => s.SubscriptionPlan)
                .Where(s => s.IsActive())
                .ToListAsync();

            var subscriptionsByPlan = subscriptions.GroupBy(s => s.SubscriptionPlan?.Name ?? "Unknown");

            foreach (var planGroup in subscriptionsByPlan)
            {
                var planName = planGroup.Key;
                var clinicsInPlan = planGroup.Select(s => s.ClinicId).ToList();
                var totalClinicsInPlan = clinicsInPlan.Count;

                foreach (var module in modules)
                {
                    var clinicsWithModule = await _context.ModuleConfigurations
                        .Where(mc => mc.ModuleName == module && mc.IsEnabled && clinicsInPlan.Contains(mc.ClinicId))
                        .CountAsync();

                    result.Add(new ModuleUsageByPlanDto
                    {
                        PlanName = planName,
                        ModuleName = module,
                        ClinicsCount = clinicsWithModule,
                        UsagePercentage = totalClinicsInPlan > 0 
                            ? (decimal)clinicsWithModule / totalClinicsInPlan * 100 
                            : 0
                    });
                }
            }

            return result.OrderBy(r => r.PlanName).ThenByDescending(r => r.UsagePercentage);
        }

        public async Task<Dictionary<string, int>> GetModuleCountsAsync()
        {
            var modules = SystemModules.GetAllModules();

            // Fetch all module counts in a single query
            var moduleCounts = await _context.ModuleConfigurations
                .Where(mc => mc.IsEnabled)
                .GroupBy(mc => mc.ModuleName)
                .Select(g => new { ModuleName = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new Dictionary<string, int>();
            foreach (var module in modules)
            {
                var count = moduleCounts.FirstOrDefault(mc => mc.ModuleName == module)?.Count ?? 0;
                result[module] = count;
            }

            return result;
        }
    }
}
