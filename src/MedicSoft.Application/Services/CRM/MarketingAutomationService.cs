using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services.CRM
{
    public class MarketingAutomationService : IMarketingAutomationService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<MarketingAutomationService> _logger;
        private readonly IAutomationEngine _automationEngine;

        public MarketingAutomationService(
            MedicSoftDbContext context,
            ILogger<MarketingAutomationService> logger,
            IAutomationEngine automationEngine)
        {
            _context = context;
            _logger = logger;
            _automationEngine = automationEngine;
        }

        public async Task<MarketingAutomationDto> CreateAsync(CreateMarketingAutomationDto dto, string tenantId)
        {
            var automation = new MarketingAutomation
            {
                Id = Guid.NewGuid(),
                TenantId = tenantId,
                Name = dto.Name,
                Description = dto.Description,
                IsActive = false, // Start inactive by default
                TriggerType = dto.TriggerType,
                TriggerStage = dto.TriggerStage,
                TriggerEvent = dto.TriggerEvent,
                DelayMinutes = dto.DelayMinutes,
                SegmentFilter = dto.SegmentFilter,
                Tags = dto.Tags ?? new List<string>(),
                Actions = dto.Actions.Select(a => new AutomationAction
                {
                    Id = Guid.NewGuid(),
                    Order = a.Order,
                    Type = a.Type,
                    EmailTemplateId = a.EmailTemplateId,
                    MessageTemplate = a.MessageTemplate,
                    Channel = a.Channel,
                    TagToAdd = a.TagToAdd,
                    TagToRemove = a.TagToRemove,
                    ScoreChange = a.ScoreChange,
                    Condition = a.Condition
                }).ToList(),
                TimesExecuted = 0,
                SuccessRate = 1.0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created marketing automation {AutomationId} for tenant {TenantId}", 
                automation.Id, tenantId);

            return MapToDto(automation);
        }

        public async Task<MarketingAutomationDto> UpdateAsync(Guid id, UpdateMarketingAutomationDto dto, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .Include(a => a.Actions)
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

            if (automation == null)
                throw new KeyNotFoundException($"Marketing automation {id} not found");

            // Update basic properties
            if (dto.Name != null) automation.Name = dto.Name;
            if (dto.Description != null) automation.Description = dto.Description;
            if (dto.TriggerType.HasValue) automation.TriggerType = dto.TriggerType.Value;
            if (dto.TriggerStage.HasValue) automation.TriggerStage = dto.TriggerStage;
            if (dto.TriggerEvent != null) automation.TriggerEvent = dto.TriggerEvent;
            if (dto.DelayMinutes.HasValue) automation.DelayMinutes = dto.DelayMinutes;
            if (dto.SegmentFilter != null) automation.SegmentFilter = dto.SegmentFilter;
            if (dto.Tags != null) automation.Tags = dto.Tags;

            // Update actions if provided
            if (dto.Actions != null)
            {
                // Remove old actions
                _context.AutomationActions.RemoveRange(automation.Actions);
                
                // Add new actions
                automation.Actions = dto.Actions.Select(a => new AutomationAction
                {
                    Id = Guid.NewGuid(),
                    MarketingAutomationId = automation.Id,
                    Order = a.Order,
                    Type = a.Type,
                    EmailTemplateId = a.EmailTemplateId,
                    MessageTemplate = a.MessageTemplate,
                    Channel = a.Channel,
                    TagToAdd = a.TagToAdd,
                    TagToRemove = a.TagToRemove,
                    ScoreChange = a.ScoreChange,
                    Condition = a.Condition
                }).ToList();
            }

            automation.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated marketing automation {AutomationId}", id);

            return MapToDto(automation);
        }

        public async Task<bool> DeleteAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

            if (automation == null)
                return false;

            // Soft delete
            automation.IsDeleted = true;
            automation.DeletedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<MarketingAutomationDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId && !a.IsDeleted);

            return automation == null ? null : MapToDto(automation);
        }

        public async Task<IEnumerable<MarketingAutomationDto>> GetAllAsync(string tenantId)
        {
            var automations = await _context.MarketingAutomations
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return automations.Select(MapToDto);
        }

        public async Task<IEnumerable<MarketingAutomationDto>> GetActiveAsync(string tenantId)
        {
            var automations = await _context.MarketingAutomations
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId && a.IsActive && !a.IsDeleted)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return automations.Select(MapToDto);
        }

        public async Task<bool> ActivateAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId && !a.IsDeleted);

            if (automation == null)
                return false;

            automation.IsActive = true;
            automation.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Activated marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId && !a.IsDeleted);

            if (automation == null)
                return false;

            automation.IsActive = false;
            automation.UpdatedAt = DateTime.UtcNow;
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<MarketingAutomationMetricsDto?> GetMetricsAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId && !a.IsDeleted);

            if (automation == null)
                return null;

            // Calculate metrics (placeholder - would need execution tracking table)
            var successfulExecutions = (int)(automation.TimesExecuted * automation.SuccessRate);
            var failedExecutions = automation.TimesExecuted - successfulExecutions;

            return new MarketingAutomationMetricsDto
            {
                AutomationId = automation.Id,
                Name = automation.Name,
                TimesExecuted = automation.TimesExecuted,
                SuccessfulExecutions = successfulExecutions,
                FailedExecutions = failedExecutions,
                SuccessRate = automation.SuccessRate,
                LastExecutedAt = automation.LastExecutedAt,
                FirstExecutedAt = automation.CreatedAt,
                TotalPatientsReached = automation.TimesExecuted // Simplified
            };
        }

        public async Task<IEnumerable<MarketingAutomationMetricsDto>> GetAllMetricsAsync(string tenantId)
        {
            var automations = await _context.MarketingAutomations
                .Where(a => a.TenantId == tenantId && !a.IsDeleted)
                .ToListAsync();

            return automations.Select(a =>
            {
                var successfulExecutions = (int)(a.TimesExecuted * a.SuccessRate);
                var failedExecutions = a.TimesExecuted - successfulExecutions;

                return new MarketingAutomationMetricsDto
                {
                    AutomationId = a.Id,
                    Name = a.Name,
                    TimesExecuted = a.TimesExecuted,
                    SuccessfulExecutions = successfulExecutions,
                    FailedExecutions = failedExecutions,
                    SuccessRate = a.SuccessRate,
                    LastExecutedAt = a.LastExecutedAt,
                    FirstExecutedAt = a.CreatedAt,
                    TotalPatientsReached = a.TimesExecuted
                };
            });
        }

        public async Task TriggerAutomationAsync(Guid automationId, Guid patientId, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .FirstOrDefaultAsync(a => a.Id == automationId && a.TenantId == tenantId && a.IsActive && !a.IsDeleted);

            if (automation == null)
            {
                _logger.LogWarning("Automation {AutomationId} not found or not active", automationId);
                return;
            }

            await _automationEngine.ExecuteAutomationAsync(automation, patientId, tenantId);
        }

        private MarketingAutomationDto MapToDto(MarketingAutomation automation)
        {
            return new MarketingAutomationDto
            {
                Id = automation.Id,
                Name = automation.Name,
                Description = automation.Description,
                IsActive = automation.IsActive,
                TriggerType = automation.TriggerType,
                TriggerStage = automation.TriggerStage,
                TriggerEvent = automation.TriggerEvent,
                DelayMinutes = automation.DelayMinutes,
                SegmentFilter = automation.SegmentFilter,
                Tags = automation.Tags ?? new List<string>(),
                Actions = automation.Actions?.Select(a => new AutomationActionDto
                {
                    Id = a.Id,
                    Order = a.Order,
                    Type = a.Type,
                    EmailTemplateId = a.EmailTemplateId,
                    EmailTemplateName = a.EmailTemplate?.Name,
                    MessageTemplate = a.MessageTemplate,
                    Channel = a.Channel,
                    TagToAdd = a.TagToAdd,
                    TagToRemove = a.TagToRemove,
                    ScoreChange = a.ScoreChange,
                    Condition = a.Condition
                }).ToList() ?? new List<AutomationActionDto>(),
                TimesExecuted = automation.TimesExecuted,
                LastExecutedAt = automation.LastExecutedAt,
                SuccessRate = automation.SuccessRate,
                CreatedAt = automation.CreatedAt,
                UpdatedAt = automation.UpdatedAt
            };
        }
    }
}
