using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    public class MarketingAutomationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Triggers
        public AutomationTriggerType TriggerType { get; set; }
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayMinutes { get; set; }
        
        // Segmentação
        public string? SegmentFilter { get; set; }
        public List<string> Tags { get; set; } = new();
        
        // Ações
        public List<AutomationActionDto> Actions { get; set; } = new();
        
        // Métricas
        public int TimesExecuted { get; set; }
        public DateTime? LastExecutedAt { get; set; }
        public double SuccessRate { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class AutomationActionDto
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public ActionType Type { get; set; }
        
        // Email
        public Guid? EmailTemplateId { get; set; }
        public string? EmailTemplateName { get; set; }
        
        // SMS/WhatsApp
        public string? MessageTemplate { get; set; }
        public string? Channel { get; set; }
        
        // Tags/Score
        public string? TagToAdd { get; set; }
        public string? TagToRemove { get; set; }
        public int? ScoreChange { get; set; }
        
        // Condicional
        public string? Condition { get; set; }
    }

    public class CreateMarketingAutomationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Triggers
        public AutomationTriggerType TriggerType { get; set; }
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayMinutes { get; set; }
        
        // Segmentação
        public string? SegmentFilter { get; set; }
        public List<string> Tags { get; set; } = new();
        
        // Ações
        public List<CreateAutomationActionDto> Actions { get; set; } = new();
    }

    public class CreateAutomationActionDto
    {
        public int Order { get; set; }
        public ActionType Type { get; set; }
        
        // Email
        public Guid? EmailTemplateId { get; set; }
        
        // SMS/WhatsApp
        public string? MessageTemplate { get; set; }
        public string? Channel { get; set; }
        
        // Tags/Score
        public string? TagToAdd { get; set; }
        public string? TagToRemove { get; set; }
        public int? ScoreChange { get; set; }
        
        // Condicional
        public string? Condition { get; set; }
    }

    public class UpdateMarketingAutomationDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        
        // Triggers
        public AutomationTriggerType? TriggerType { get; set; }
        public JourneyStageEnum? TriggerStage { get; set; }
        public string? TriggerEvent { get; set; }
        public int? DelayMinutes { get; set; }
        
        // Segmentação
        public string? SegmentFilter { get; set; }
        public List<string>? Tags { get; set; }
        
        // Ações
        public List<CreateAutomationActionDto>? Actions { get; set; }
    }

    public class MarketingAutomationMetricsDto
    {
        public Guid AutomationId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TimesExecuted { get; set; }
        public int SuccessfulExecutions { get; set; }
        public int FailedExecutions { get; set; }
        public double SuccessRate { get; set; }
        public DateTime? LastExecutedAt { get; set; }
        public DateTime? FirstExecutedAt { get; set; }
        public int TotalPatientsReached { get; set; }
    }
}
