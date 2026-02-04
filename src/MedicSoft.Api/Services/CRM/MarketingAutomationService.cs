using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.DTOs.Common;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
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
            var automation = new MarketingAutomation(
                dto.Name,
                dto.Description ?? "",
                dto.TriggerType,
                tenantId);

            // Configure trigger
            automation.ConfigureTrigger(dto.TriggerStage, dto.TriggerEvent, dto.DelayMinutes);

            // Set segment filter if provided
            if (!string.IsNullOrEmpty(dto.SegmentFilter))
                automation.SetSegmentFilter(dto.SegmentFilter);

            // Add tags
            if (dto.Tags != null)
            {
                foreach (var tag in dto.Tags)
                    automation.AddTag(tag);
            }

            // Add actions
            foreach (var actionDto in dto.Actions)
            {
                var action = new AutomationAction(
                    automation.Id,
                    actionDto.Order,
                    actionDto.Type,
                    tenantId);

                // Configure action based on type
                ConfigureActionFromCreate(action, actionDto);

                automation.AddAction(action);
            }

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

            // Note: Name, Description, and TriggerType cannot be updated after creation
            // Only trigger configuration, segmentation and actions can be updated

            // Update trigger configuration
            if (dto.TriggerStage.HasValue || dto.TriggerEvent != null || dto.DelayMinutes.HasValue)
            {
                automation.ConfigureTrigger(
                    dto.TriggerStage ?? automation.TriggerStage,
                    dto.TriggerEvent ?? automation.TriggerEvent,
                    dto.DelayMinutes ?? automation.DelayMinutes);
            }

            // Update segment filter
            if (dto.SegmentFilter != null)
                automation.SetSegmentFilter(dto.SegmentFilter);

            // Update tags if provided
            if (dto.Tags != null)
            {
                // Remove all existing tags and add new ones
                var currentTags = automation.Tags.ToList();
                foreach (var tag in currentTags)
                    automation.RemoveTag(tag);
                
                foreach (var tag in dto.Tags)
                    automation.AddTag(tag);
            }

            // Update actions if provided
            if (dto.Actions != null)
            {
                // Remove old actions
                var actionsToRemove = automation.Actions.ToList();
                foreach (var action in actionsToRemove)
                {
                    automation.RemoveAction(action.Id);
                    _context.AutomationActions.Remove(action);
                }
                
                // Add new actions
                foreach (var actionDto in dto.Actions)
                {
                    var action = new AutomationAction(
                        automation.Id,
                        actionDto.Order,
                        actionDto.Type,
                        tenantId);

                    ConfigureActionFromCreate(action, actionDto);
                    automation.AddAction(action);
                }
            }

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

            // Hard delete for now since BaseEntity doesn't have IsDeleted
            _context.MarketingAutomations.Remove(automation);
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<MarketingAutomationDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .AsNoTracking()
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

            return automation == null ? null : MapToDto(automation);
        }

        public async Task<IEnumerable<MarketingAutomationDto>> GetAllAsync(string tenantId)
        {
            var automations = await _context.MarketingAutomations
                .AsNoTracking()
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return automations.Select(MapToDto);
        }

        public async Task<IEnumerable<MarketingAutomationDto>> GetActiveAsync(string tenantId)
        {
            var automations = await _context.MarketingAutomations
                .AsNoTracking()
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();

            return automations.Select(MapToDto);
        }

        public async Task<PagedResult<MarketingAutomationDto>> GetAllPagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25)
        {
            var query = _context.MarketingAutomations
                .AsNoTracking()
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId)
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var automations = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var automationDtos = automations.Select(MapToDto).ToList();

            return new PagedResult<MarketingAutomationDto>(automationDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<MarketingAutomationDto>> GetActivePagedAsync(string tenantId, int pageNumber = 1, int pageSize = 25)
        {
            var query = _context.MarketingAutomations
                .AsNoTracking()
                .Include(a => a.Actions)
                .ThenInclude(a => a.EmailTemplate)
                .Where(a => a.TenantId == tenantId && a.IsActive)
                .OrderByDescending(a => a.CreatedAt);

            var totalCount = await query.CountAsync();
            
            var automations = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var automationDtos = automations.Select(MapToDto).ToList();

            return new PagedResult<MarketingAutomationDto>(automationDtos, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> ActivateAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

            if (automation == null)
                return false;

            automation.Activate();
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Activated marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<bool> DeactivateAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

            if (automation == null)
                return false;

            automation.Deactivate();
            
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated marketing automation {AutomationId}", id);

            return true;
        }

        public async Task<MarketingAutomationMetricsDto?> GetMetricsAsync(Guid id, string tenantId)
        {
            var automation = await _context.MarketingAutomations
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

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
                .Where(a => a.TenantId == tenantId)
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
                .FirstOrDefaultAsync(a => a.Id == automationId && a.TenantId == tenantId && a.IsActive);

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
                Tags = automation.Tags.ToList(),
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
                    ScoreChange = a.ScoreChange,
                    Condition = a.Condition
                }).ToList() ?? new List<AutomationActionDto>(),
                TimesExecuted = automation.TimesExecuted,
                LastExecutedAt = automation.LastExecutedAt,
                SuccessRate = automation.SuccessRate,
                CreatedAt = automation.CreatedAt,
                UpdatedAt = automation.UpdatedAt ?? automation.CreatedAt
            };
        }

        private void ConfigureActionFromCreate(AutomationAction action, CreateAutomationActionDto dto)
        {
            switch (dto.Type)
            {
                case ActionType.SendEmail:
                    if (dto.EmailTemplateId.HasValue)
                        action.ConfigureEmailAction(dto.EmailTemplateId.Value);
                    break;

                case ActionType.SendSMS:
                case ActionType.SendWhatsApp:
                    if (!string.IsNullOrEmpty(dto.MessageTemplate) && !string.IsNullOrEmpty(dto.Channel))
                        action.ConfigureMessageAction(dto.MessageTemplate, dto.Channel);
                    break;

                case ActionType.AddTag:
                case ActionType.RemoveTag:
                    if (!string.IsNullOrEmpty(dto.TagToAdd))
                        action.ConfigureTagAction(dto.TagToAdd);
                    break;

                case ActionType.ChangeScore:
                    if (dto.ScoreChange.HasValue)
                        action.ConfigureScoreAction(dto.ScoreChange.Value);
                    break;
            }

            if (!string.IsNullOrEmpty(dto.Condition))
                action.SetCondition(dto.Condition);
        }
    }
}
