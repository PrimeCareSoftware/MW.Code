using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Configuração de regras de alertas automáticos
    /// Define quando e como alertas devem ser gerados automaticamente
    /// </summary>
    public class AlertConfiguration : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public AlertCategory Category { get; private set; }
        public AlertPriority Priority { get; private set; }
        public bool IsEnabled { get; private set; }
        
        // Gatilho
        public string TriggerType { get; private set; } // Tipo de evento que dispara o alerta
        public string TriggerConditions { get; private set; } // JSON com condições
        
        // Template da mensagem
        public string TitleTemplate { get; private set; }
        public string MessageTemplate { get; private set; }
        public string? ActionUrlTemplate { get; private set; }
        public AlertAction SuggestedAction { get; private set; }
        public string? ActionLabelTemplate { get; private set; }
        
        // Destinatários
        public AlertRecipientType RecipientType { get; private set; }
        public string? RecipientFilter { get; private set; } // JSON com filtros de destinatários
        
        // Configurações de entrega
        public string DeliveryChannels { get; private set; } // JSON array de canais
        
        // Configurações de expiração
        public int? ExpirationHours { get; private set; }
        
        // Escopo
        public bool IsGlobal { get; private set; } // Se true, aplica-se a todos os tenants
        
        // Estatísticas
        public int AlertsGeneratedCount { get; private set; }
        public DateTime? LastTriggeredAt { get; private set; }
        
        private AlertConfiguration()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
            TriggerType = null!;
            TriggerConditions = null!;
            TitleTemplate = null!;
            MessageTemplate = null!;
            DeliveryChannels = null!;
        }
        
        public AlertConfiguration(
            string name,
            string description,
            AlertCategory category,
            AlertPriority priority,
            string triggerType,
            string triggerConditions,
            string titleTemplate,
            string messageTemplate,
            AlertRecipientType recipientType,
            string tenantId,
            string? actionUrlTemplate = null,
            AlertAction suggestedAction = AlertAction.None,
            string? actionLabelTemplate = null,
            string? recipientFilter = null,
            string? deliveryChannels = null,
            int? expirationHours = null,
            bool isGlobal = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(triggerType))
                throw new ArgumentException("TriggerType cannot be empty", nameof(triggerType));
            
            if (string.IsNullOrWhiteSpace(titleTemplate))
                throw new ArgumentException("TitleTemplate cannot be empty", nameof(titleTemplate));
            
            if (string.IsNullOrWhiteSpace(messageTemplate))
                throw new ArgumentException("MessageTemplate cannot be empty", nameof(messageTemplate));
            
            if (expirationHours.HasValue && expirationHours.Value <= 0)
                throw new ArgumentException("ExpirationHours must be positive", nameof(expirationHours));
            
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Category = category;
            Priority = priority;
            IsEnabled = true;
            
            TriggerType = triggerType.Trim();
            TriggerConditions = triggerConditions?.Trim() ?? "{}";
            
            TitleTemplate = titleTemplate.Trim();
            MessageTemplate = messageTemplate.Trim();
            ActionUrlTemplate = actionUrlTemplate?.Trim();
            SuggestedAction = suggestedAction;
            ActionLabelTemplate = actionLabelTemplate?.Trim();
            
            RecipientType = recipientType;
            RecipientFilter = recipientFilter?.Trim();
            
            DeliveryChannels = deliveryChannels ?? "[\"InApp\"]";
            ExpirationHours = expirationHours;
            IsGlobal = isGlobal;
            
            AlertsGeneratedCount = 0;
        }
        
        /// <summary>
        /// Atualizar configuração
        /// </summary>
        public void Update(
            string name,
            string description,
            AlertPriority priority,
            string titleTemplate,
            string messageTemplate,
            string? actionUrlTemplate = null,
            AlertAction suggestedAction = AlertAction.None,
            string? actionLabelTemplate = null,
            string? recipientFilter = null,
            string? deliveryChannels = null,
            int? expirationHours = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(titleTemplate))
                throw new ArgumentException("TitleTemplate cannot be empty", nameof(titleTemplate));
            
            if (string.IsNullOrWhiteSpace(messageTemplate))
                throw new ArgumentException("MessageTemplate cannot be empty", nameof(messageTemplate));
            
            if (expirationHours.HasValue && expirationHours.Value <= 0)
                throw new ArgumentException("ExpirationHours must be positive", nameof(expirationHours));
            
            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Priority = priority;
            
            TitleTemplate = titleTemplate.Trim();
            MessageTemplate = messageTemplate.Trim();
            ActionUrlTemplate = actionUrlTemplate?.Trim();
            SuggestedAction = suggestedAction;
            ActionLabelTemplate = actionLabelTemplate?.Trim();
            
            RecipientFilter = recipientFilter?.Trim();
            DeliveryChannels = deliveryChannels ?? "[\"InApp\"]";
            ExpirationHours = expirationHours;
            
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Ativar configuração
        /// </summary>
        public void Enable()
        {
            IsEnabled = true;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Desativar configuração
        /// </summary>
        public void Disable()
        {
            IsEnabled = false;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Registrar geração de alerta
        /// </summary>
        public void RecordAlertGeneration()
        {
            AlertsGeneratedCount++;
            LastTriggeredAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}
