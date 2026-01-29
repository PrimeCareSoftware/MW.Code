using System;
using System.Linq;
using System.Threading.Tasks;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Jobs.SystemAdmin
{
    /// <summary>
    /// Background job for automatic tag application based on clinic characteristics
    /// </summary>
    public class AutoTaggingJob
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<AutoTaggingJob> _logger;

        public AutoTaggingJob(
            MedicSoftDbContext context,
            ILogger<AutoTaggingJob> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Apply automatic tags to clinics based on their characteristics (runs daily)
        /// </summary>
        [AutomaticRetry(Attempts = 3)]
        public async Task ApplyAutomaticTagsAsync()
        {
            try
            {
                _logger.LogInformation("Starting automatic tag application...");

                var tagsApplied = 0;

                // Get all automatic tags
                var automaticTags = await _context.Tags
                    .IgnoreQueryFilters()
                    .Where(t => t.IsAutomatic)
                    .ToListAsync();

                if (!automaticTags.Any())
                {
                    _logger.LogInformation("No automatic tags found");
                    return;
                }

                // Apply "At-Risk" tag - Clinics inactive for more than 30 days
                var atRiskTag = automaticTags.FirstOrDefault(t => t.Name == "At-Risk");
                if (atRiskTag != null)
                {
                    tagsApplied += await ApplyAtRiskTag(atRiskTag.Id);
                }

                // Apply "High-Value" tag - Clinics with high subscription value
                var highValueTag = automaticTags.FirstOrDefault(t => t.Name == "High-Value");
                if (highValueTag != null)
                {
                    tagsApplied += await ApplyHighValueTag(highValueTag.Id);
                }

                // Apply "New" tag - Clinics created in last 30 days
                var newTag = automaticTags.FirstOrDefault(t => t.Name == "New");
                if (newTag != null)
                {
                    tagsApplied += await ApplyNewTag(newTag.Id);
                }

                // Apply "Active-User" tag - Clinics with high user engagement
                var activeUserTag = automaticTags.FirstOrDefault(t => t.Name == "Active-User");
                if (activeUserTag != null)
                {
                    tagsApplied += await ApplyActiveUserTag(activeUserTag.Id);
                }

                // Apply "Support-Heavy" tag - Clinics with many support tickets
                var supportHeavyTag = automaticTags.FirstOrDefault(t => t.Name == "Support-Heavy");
                if (supportHeavyTag != null)
                {
                    tagsApplied += await ApplySupportHeavyTag(supportHeavyTag.Id);
                }

                // Apply "Trial" tag - Clinics in trial period
                var trialTag = automaticTags.FirstOrDefault(t => t.Name == "Trial");
                if (trialTag != null)
                {
                    tagsApplied += await ApplyTrialTag(trialTag.Id);
                }

                _logger.LogInformation($"Automatic tagging complete. Applied {tagsApplied} tags.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying automatic tags");
                throw;
            }
        }

        private async Task<int> ApplyAtRiskTag(Guid tagId)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var count = 0;

            var inactiveClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.IsActive && c.UpdatedAt < thirtyDaysAgo)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var clinicId in inactiveClinics)
            {
                if (await AddTagIfNotExists(clinicId, tagId))
                    count++;
            }

            // Remove tag from active clinics
            var activeClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.IsActive && c.UpdatedAt >= thirtyDaysAgo)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var clinicId in activeClinics)
            {
                await RemoveTagIfExists(clinicId, tagId);
            }

            return count;
        }

        private async Task<int> ApplyHighValueTag(Guid tagId)
        {
            var count = 0;
            var highValueThreshold = 1000m; // R$ 1000 or more

            var highValueClinics = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == Domain.Entities.SubscriptionStatus.Active && s.CurrentPrice >= highValueThreshold)
                .Select(s => s.ClinicId)
                .Distinct()
                .ToListAsync();

            foreach (var clinicId in highValueClinics)
            {
                if (await AddTagIfNotExists(clinicId, tagId))
                    count++;
            }

            // Remove tag from low-value clinics
            var lowValueClinics = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == Domain.Entities.SubscriptionStatus.Active && s.CurrentPrice < highValueThreshold)
                .Select(s => s.ClinicId)
                .Distinct()
                .ToListAsync();

            foreach (var clinicId in lowValueClinics)
            {
                await RemoveTagIfExists(clinicId, tagId);
            }

            return count;
        }

        private async Task<int> ApplyNewTag(Guid tagId)
        {
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var count = 0;

            var newClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.CreatedAt >= thirtyDaysAgo)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var clinicId in newClinics)
            {
                if (await AddTagIfNotExists(clinicId, tagId))
                    count++;
            }

            // Remove tag from old clinics
            var oldClinics = await _context.Clinics
                .IgnoreQueryFilters()
                .Where(c => c.CreatedAt < thirtyDaysAgo)
                .Select(c => c.Id)
                .ToListAsync();

            foreach (var clinicId in oldClinics)
            {
                await RemoveTagIfExists(clinicId, tagId);
            }

            return count;
        }

        private async Task<int> ApplyActiveUserTag(Guid tagId)
        {
            var count = 0;
            var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);

            // Find clinics with recent user activity
            var activeClinics = await _context.Set<Domain.Entities.UserSession>()
                .IgnoreQueryFilters()
                .Where(s => s.CreatedAt >= sevenDaysAgo)
                .Select(s => s.TenantId)
                .Distinct()
                .ToListAsync();

            foreach (var tenantId in activeClinics)
            {
                if (Guid.TryParse(tenantId, out var clinicId))
                {
                    if (await AddTagIfNotExists(clinicId, tagId))
                        count++;
                }
            }

            return count;
        }

        private async Task<int> ApplySupportHeavyTag(Guid tagId)
        {
            var count = 0;
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);
            var ticketThreshold = 5; // 5 or more tickets in last 30 days

            var supportHeavyClinics = await _context.Tickets
                .IgnoreQueryFilters()
                .Where(t => t.CreatedAt >= thirtyDaysAgo)
                .GroupBy(t => t.ClinicId)
                .Where(g => g.Count() >= ticketThreshold)
                .Select(g => g.Key)
                .ToListAsync();

            foreach (var clinicId in supportHeavyClinics)
            {
                if (await AddTagIfNotExists(clinicId, tagId))
                    count++;
            }

            // Remove tag from clinics with few tickets
            var lowSupportClinics = await _context.Tickets
                .IgnoreQueryFilters()
                .Where(t => t.CreatedAt >= thirtyDaysAgo)
                .GroupBy(t => t.ClinicId)
                .Where(g => g.Count() < ticketThreshold)
                .Select(g => g.Key)
                .ToListAsync();

            foreach (var clinicId in lowSupportClinics)
            {
                await RemoveTagIfExists(clinicId, tagId);
            }

            return count;
        }

        private async Task<int> ApplyTrialTag(Guid tagId)
        {
            var count = 0;

            var trialClinics = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status == Domain.Entities.SubscriptionStatus.Trial)
                .Select(s => s.ClinicId)
                .Distinct()
                .ToListAsync();

            foreach (var clinicId in trialClinics)
            {
                if (await AddTagIfNotExists(clinicId, tagId))
                    count++;
            }

            // Remove tag from non-trial clinics
            var nonTrialClinics = await _context.ClinicSubscriptions
                .IgnoreQueryFilters()
                .Where(s => s.Status != Domain.Entities.SubscriptionStatus.Trial)
                .Select(s => s.ClinicId)
                .Distinct()
                .ToListAsync();

            foreach (var clinicId in nonTrialClinics)
            {
                await RemoveTagIfExists(clinicId, tagId);
            }

            return count;
        }

        private async Task<bool> AddTagIfNotExists(Guid clinicId, Guid tagId)
        {
            var exists = await _context.Set<Domain.Entities.ClinicTag>()
                .IgnoreQueryFilters()
                .AnyAsync(ct => ct.ClinicId == clinicId && ct.TagId == tagId);

            if (!exists)
            {
                // Get the clinic to obtain its tenantId
                var clinic = await _context.Clinics
                    .IgnoreQueryFilters()
                    .FirstOrDefaultAsync(c => c.Id == clinicId);

                if (clinic == null)
                {
                    _logger.LogWarning($"Clinic {clinicId} not found when trying to add tag {tagId}");
                    return false;
                }

                var clinicTag = new Domain.Entities.ClinicTag(
                    clinicId,
                    tagId,
                    clinic.TenantId,
                    assignedBy: "auto-tagging-job",
                    isAutoAssigned: true
                );

                _context.Set<Domain.Entities.ClinicTag>().Add(clinicTag);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        private async Task RemoveTagIfExists(Guid clinicId, Guid tagId)
        {
            var clinicTag = await _context.Set<Domain.Entities.ClinicTag>()
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(ct => ct.ClinicId == clinicId && ct.TagId == tagId);

            if (clinicTag != null)
            {
                _context.Set<Domain.Entities.ClinicTag>().Remove(clinicTag);
                await _context.SaveChangesAsync();
            }
        }
    }
}
