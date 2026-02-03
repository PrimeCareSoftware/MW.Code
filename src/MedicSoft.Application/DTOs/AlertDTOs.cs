using System;
using System.Collections.Generic;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO para exibir alerta
    /// </summary>
    public class AlertDto
    {
        public Guid Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string SuggestedAction { get; set; } = string.Empty;
        public string? ActionLabel { get; set; }
        
        public string RecipientType { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
        public string? Role { get; set; }
        public Guid? ClinicId { get; set; }
        
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        
        public DateTime? AcknowledgedAt { get; set; }
        public Guid? AcknowledgedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedBy { get; set; }
        public string? ResolutionNotes { get; set; }
        public DateTime? ExpiresAt { get; set; }
        
        public List<string> DeliveryChannels { get; set; } = new();
        public string? Metadata { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        
        public bool IsExpired { get; set; }
        public bool RequiresUrgentAction { get; set; }
    }
    
    /// <summary>
    /// DTO para criar alerta
    /// </summary>
    public class CreateAlertDto
    {
        public AlertCategory Category { get; set; }
        public AlertPriority Priority { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public AlertAction SuggestedAction { get; set; }
        public string? ActionLabel { get; set; }
        
        public AlertRecipientType RecipientType { get; set; }
        public Guid? UserId { get; set; }
        public string? Role { get; set; }
        public Guid? ClinicId { get; set; }
        
        public string? RelatedEntityType { get; set; }
        public Guid? RelatedEntityId { get; set; }
        
        public DateTime? ExpiresAt { get; set; }
        public List<AlertChannel>? DeliveryChannels { get; set; }
        public string? Metadata { get; set; }
    }
    
    /// <summary>
    /// DTO para resposta de alerta reconhecido
    /// </summary>
    public class AcknowledgeAlertDto
    {
        public Guid AlertId { get; set; }
    }
    
    /// <summary>
    /// DTO para resolver alerta
    /// </summary>
    public class ResolveAlertDto
    {
        public Guid AlertId { get; set; }
        public string? Notes { get; set; }
    }
    
    /// <summary>
    /// DTO para estatísticas de alertas
    /// </summary>
    public class AlertStatisticsDto
    {
        public int TotalActive { get; set; }
        public int TotalAcknowledged { get; set; }
        public int TotalResolved { get; set; }
        public int TotalDismissed { get; set; }
        public int TotalExpired { get; set; }
        
        public int CriticalCount { get; set; }
        public int HighCount { get; set; }
        public int NormalCount { get; set; }
        public int LowCount { get; set; }
        
        public Dictionary<string, int> CategoryCounts { get; set; } = new();
    }
    
    /// <summary>
    /// DTO para listagem de alertas com filtros
    /// </summary>
    public class AlertFilterDto
    {
        public AlertStatus? Status { get; set; }
        public AlertPriority? Priority { get; set; }
        public AlertCategory? Category { get; set; }
        public Guid? UserId { get; set; }
        public Guid? ClinicId { get; set; }
        public bool IncludeExpired { get; set; } = false;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
    
    /// <summary>
    /// DTO para configuração de alerta
    /// </summary>
    public class AlertConfigurationDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsEnabled { get; set; }
        
        public string TriggerType { get; set; } = string.Empty;
        public string TriggerConditions { get; set; } = string.Empty;
        
        public string TitleTemplate { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string? ActionUrlTemplate { get; set; }
        public string SuggestedAction { get; set; } = string.Empty;
        public string? ActionLabelTemplate { get; set; }
        
        public string RecipientType { get; set; } = string.Empty;
        public string? RecipientFilter { get; set; }
        
        public string DeliveryChannels { get; set; } = string.Empty;
        public int? ExpirationHours { get; set; }
        public bool IsGlobal { get; set; }
        
        public int AlertsGeneratedCount { get; set; }
        public DateTime? LastTriggeredAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO para criar/atualizar configuração de alerta
    /// </summary>
    public class CreateAlertConfigurationDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public AlertCategory Category { get; set; }
        public AlertPriority Priority { get; set; }
        public string TriggerType { get; set; } = string.Empty;
        public string TriggerConditions { get; set; } = string.Empty;
        public string TitleTemplate { get; set; } = string.Empty;
        public string MessageTemplate { get; set; } = string.Empty;
        public string? ActionUrlTemplate { get; set; }
        public AlertAction SuggestedAction { get; set; }
        public string? ActionLabelTemplate { get; set; }
        public AlertRecipientType RecipientType { get; set; }
        public string? RecipientFilter { get; set; }
        public string? DeliveryChannels { get; set; }
        public int? ExpirationHours { get; set; }
        public bool IsGlobal { get; set; }
    }
}
