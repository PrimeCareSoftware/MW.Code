using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um alerta do sistema - notificação persistente que requer atenção ou ação
    /// Baseado em práticas de mercado (Doctolib, ZocDoc, 1Doc)
    /// </summary>
    public class Alert : BaseEntity
    {
        public AlertCategory Category { get; private set; }
        public AlertPriority Priority { get; private set; }
        public AlertStatus Status { get; private set; }
        public string Title { get; private set; }
        public string Message { get; private set; }
        public string? ActionUrl { get; private set; }
        public AlertAction SuggestedAction { get; private set; }
        public string? ActionLabel { get; private set; }
        
        // Destinatários
        public AlertRecipientType RecipientType { get; private set; }
        public Guid? UserId { get; private set; }
        public string? Role { get; private set; }
        public Guid? ClinicId { get; private set; }
        
        // Entidade relacionada (para rastreamento)
        public string? RelatedEntityType { get; private set; }
        public Guid? RelatedEntityId { get; private set; }
        
        // Gerenciamento
        public DateTime? AcknowledgedAt { get; private set; }
        public Guid? AcknowledgedBy { get; private set; }
        public DateTime? ResolvedAt { get; private set; }
        public Guid? ResolvedBy { get; private set; }
        public string? ResolutionNotes { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        
        // Canal(is) de entrega
        public List<AlertChannel> DeliveryChannels { get; private set; }
        
        // Metadados adicionais (JSON)
        public string? Metadata { get; private set; }
        
        // Navigation properties
        public User? User { get; private set; }
        
        private Alert()
        {
            // EF Constructor
            Title = null!;
            Message = null!;
            DeliveryChannels = new List<AlertChannel>();
        }
        
        public Alert(
            AlertCategory category,
            AlertPriority priority,
            string title,
            string message,
            AlertRecipientType recipientType,
            string tenantId,
            Guid? userId = null,
            string? role = null,
            Guid? clinicId = null,
            string? actionUrl = null,
            AlertAction suggestedAction = AlertAction.None,
            string? actionLabel = null,
            string? relatedEntityType = null,
            Guid? relatedEntityId = null,
            DateTime? expiresAt = null,
            List<AlertChannel>? deliveryChannels = null,
            string? metadata = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));
            
            // Validar destinatário
            if (recipientType == AlertRecipientType.User && userId == null)
                throw new ArgumentException("UserId is required for User recipient type");
            
            if (recipientType == AlertRecipientType.Role && string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required for Role recipient type");
            
            if (recipientType == AlertRecipientType.Clinic && clinicId == null)
                throw new ArgumentException("ClinicId is required for Clinic recipient type");
            
            if (expiresAt.HasValue && expiresAt.Value <= DateTime.UtcNow)
                throw new ArgumentException("ExpiresAt must be in the future", nameof(expiresAt));
            
            Category = category;
            Priority = priority;
            Status = AlertStatus.Active;
            Title = title.Trim();
            Message = message.Trim();
            ActionUrl = actionUrl?.Trim();
            SuggestedAction = suggestedAction;
            ActionLabel = actionLabel?.Trim();
            
            RecipientType = recipientType;
            UserId = userId;
            Role = role?.Trim();
            ClinicId = clinicId;
            
            RelatedEntityType = relatedEntityType?.Trim();
            RelatedEntityId = relatedEntityId;
            
            ExpiresAt = expiresAt;
            DeliveryChannels = deliveryChannels ?? new List<AlertChannel> { AlertChannel.InApp };
            Metadata = metadata;
        }
        
        /// <summary>
        /// Marcar alerta como reconhecido
        /// </summary>
        public void Acknowledge(Guid acknowledgedBy)
        {
            if (Status == AlertStatus.Resolved || Status == AlertStatus.Dismissed)
                throw new InvalidOperationException("Cannot acknowledge a resolved or dismissed alert");
            
            Status = AlertStatus.Acknowledged;
            AcknowledgedAt = DateTime.UtcNow;
            AcknowledgedBy = acknowledgedBy;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Marcar alerta como resolvido
        /// </summary>
        public void Resolve(Guid resolvedBy, string? notes = null)
        {
            Status = AlertStatus.Resolved;
            ResolvedAt = DateTime.UtcNow;
            ResolvedBy = resolvedBy;
            ResolutionNotes = notes?.Trim();
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Dispensar alerta (não é relevante)
        /// </summary>
        public void Dismiss(Guid dismissedBy)
        {
            Status = AlertStatus.Dismissed;
            ResolvedAt = DateTime.UtcNow;
            ResolvedBy = dismissedBy;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Verificar se o alerta expirou
        /// </summary>
        public bool IsExpired()
        {
            return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
        }
        
        /// <summary>
        /// Marcar como expirado
        /// </summary>
        public void MarkAsExpired()
        {
            if (Status != AlertStatus.Active && Status != AlertStatus.Acknowledged)
                return;
            
            Status = AlertStatus.Expired;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Atualizar URL de ação
        /// </summary>
        public void UpdateActionUrl(string actionUrl, string? actionLabel = null)
        {
            ActionUrl = actionUrl?.Trim();
            ActionLabel = actionLabel?.Trim();
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Adicionar canal de entrega
        /// </summary>
        public void AddDeliveryChannel(AlertChannel channel)
        {
            if (!DeliveryChannels.Contains(channel))
            {
                DeliveryChannels.Add(channel);
                UpdateTimestamp();
            }
        }
        
        /// <summary>
        /// Verificar se requer ação urgente
        /// </summary>
        public bool RequiresUrgentAction()
        {
            return Priority == AlertPriority.Critical && 
                   Status == AlertStatus.Active &&
                   !IsExpired();
        }
    }
}
