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
            var result = new List<ModuleAdoptionDto>();

            foreach (var module in modules)
            {
                var stats = await GetModuleUsageStatsAsync(module);
                var moduleInfo = SystemModules.GetModuleInfo(module);

                result.Add(new ModuleAdoptionDto
                {
                    ModuleName = module,
                    DisplayName = moduleInfo.DisplayName,
                    AdoptionRate = stats.AdoptionRate,
                    EnabledCount = stats.ClinicsWithModuleEnabled
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
                .Where(s => s.IsActive)
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
            var result = new Dictionary<string, int>();

            foreach (var module in modules)
            {
                var count = await _context.ModuleConfigurations
                    .Where(mc => mc.ModuleName == module && mc.IsEnabled)
                    .CountAsync();

                result[module] = count;
            }

            return result;
        }
    }
}
