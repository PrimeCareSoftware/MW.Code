using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.WhatsAppAgent.Entities
{
    /// <summary>
    /// Configuration for WhatsApp AI Agent per clinic
    /// Enables automated appointment scheduling via WhatsApp
    /// </summary>
    public class WhatsAppAgentConfiguration
    {
        public Guid Id { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string TenantId { get; private set; }
        
        [Required]
        [MaxLength(200)]
        public string ClinicName { get; private set; }
        
        [Required]
        [MaxLength(20)]
        public string WhatsAppNumber { get; private set; }
        
        /// <summary>
        /// API key for WhatsApp Business API integration (encrypted)
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string WhatsAppApiKey { get; private set; }
        
        /// <summary>
        /// API key for AI service (OpenAI, Azure OpenAI, etc.) (encrypted)
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string AiApiKey { get; private set; }
        
        /// <summary>
        /// AI model to use (e.g., "gpt-4", "gpt-3.5-turbo")
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string AiModel { get; private set; }
        
        /// <summary>
        /// System prompt for the AI agent (with security constraints)
        /// </summary>
        [Required]
        [MaxLength(4000)]
        public string SystemPrompt { get; private set; }
        
        /// <summary>
        /// Maximum number of messages per user per hour (rate limiting)
        /// </summary>
        public int MaxMessagesPerHour { get; private set; }
        
        /// <summary>
        /// Whether the agent is active and can respond to messages
        /// </summary>
        public bool IsActive { get; private set; }
        
        /// <summary>
        /// Business hours start time (e.g., "08:00")
        /// </summary>
        [MaxLength(5)]
        public string BusinessHoursStart { get; private set; }
        
        /// <summary>
        /// Business hours end time (e.g., "18:00")
        /// </summary>
        [MaxLength(5)]
        public string BusinessHoursEnd { get; private set; }
        
        /// <summary>
        /// Days of the week the agent is active (comma-separated: "Mon,Tue,Wed,Thu,Fri")
        /// </summary>
        [MaxLength(100)]
        public string ActiveDays { get; private set; }
        
        /// <summary>
        /// Fallback message when agent cannot handle the request
        /// </summary>
        [MaxLength(500)]
        public string FallbackMessage { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        private WhatsAppAgentConfiguration() { }

        public WhatsAppAgentConfiguration(
            string tenantId,
            string clinicName,
            string whatsAppNumber,
            string whatsAppApiKey,
            string aiApiKey,
            string aiModel,
            string systemPrompt,
            int maxMessagesPerHour = 20,
            string businessHoursStart = "08:00",
            string businessHoursEnd = "18:00",
            string activeDays = "Mon,Tue,Wed,Thu,Fri")
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));
            
            if (string.IsNullOrWhiteSpace(clinicName))
                throw new ArgumentException("ClinicName is required", nameof(clinicName));
            
            if (string.IsNullOrWhiteSpace(whatsAppNumber))
                throw new ArgumentException("WhatsAppNumber is required", nameof(whatsAppNumber));
            
            if (string.IsNullOrWhiteSpace(whatsAppApiKey))
                throw new ArgumentException("WhatsAppApiKey is required", nameof(whatsAppApiKey));
            
            if (string.IsNullOrWhiteSpace(aiApiKey))
                throw new ArgumentException("AiApiKey is required", nameof(aiApiKey));
            
            if (string.IsNullOrWhiteSpace(aiModel))
                throw new ArgumentException("AiModel is required", nameof(aiModel));
            
            if (string.IsNullOrWhiteSpace(systemPrompt))
                throw new ArgumentException("SystemPrompt is required", nameof(systemPrompt));
            
            if (maxMessagesPerHour < 1 || maxMessagesPerHour > 100)
                throw new ArgumentException("MaxMessagesPerHour must be between 1 and 100", nameof(maxMessagesPerHour));

            Id = Guid.NewGuid();
            TenantId = tenantId;
            ClinicName = clinicName;
            WhatsAppNumber = whatsAppNumber;
            WhatsAppApiKey = whatsAppApiKey;
            AiApiKey = aiApiKey;
            AiModel = aiModel;
            SystemPrompt = systemPrompt;
            MaxMessagesPerHour = maxMessagesPerHour;
            IsActive = false;
            BusinessHoursStart = businessHoursStart;
            BusinessHoursEnd = businessHoursEnd;
            ActiveDays = activeDays;
            FallbackMessage = "Desculpe, não consegui processar sua solicitação. Por favor, entre em contato com nossa recepção.";
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateConfiguration(
            string clinicName,
            string whatsAppNumber,
            string systemPrompt,
            int maxMessagesPerHour,
            string businessHoursStart,
            string businessHoursEnd,
            string activeDays,
            string fallbackMessage)
        {
            if (!string.IsNullOrWhiteSpace(clinicName))
                ClinicName = clinicName;
            
            if (!string.IsNullOrWhiteSpace(whatsAppNumber))
                WhatsAppNumber = whatsAppNumber;
            
            if (!string.IsNullOrWhiteSpace(systemPrompt))
                SystemPrompt = systemPrompt;
            
            if (maxMessagesPerHour >= 1 && maxMessagesPerHour <= 100)
                MaxMessagesPerHour = maxMessagesPerHour;
            
            if (!string.IsNullOrWhiteSpace(businessHoursStart))
                BusinessHoursStart = businessHoursStart;
            
            if (!string.IsNullOrWhiteSpace(businessHoursEnd))
                BusinessHoursEnd = businessHoursEnd;
            
            if (!string.IsNullOrWhiteSpace(activeDays))
                ActiveDays = activeDays;
            
            if (!string.IsNullOrWhiteSpace(fallbackMessage))
                FallbackMessage = fallbackMessage;

            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateApiKeys(string whatsAppApiKey, string aiApiKey)
        {
            if (!string.IsNullOrWhiteSpace(whatsAppApiKey))
                WhatsAppApiKey = whatsAppApiKey;
            
            if (!string.IsNullOrWhiteSpace(aiApiKey))
                AiApiKey = aiApiKey;

            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsWithinBusinessHours(DateTime dateTime)
        {
            var timeStr = dateTime.ToString("HH:mm");
            var dayOfWeek = dateTime.DayOfWeek.ToString().Substring(0, 3);
            
            return IsWithinTimeRange(timeStr) && IsActiveDay(dayOfWeek);
        }

        private bool IsWithinTimeRange(string timeStr)
        {
            return string.Compare(timeStr, BusinessHoursStart) >= 0 && 
                   string.Compare(timeStr, BusinessHoursEnd) <= 0;
        }

        private bool IsActiveDay(string dayOfWeek)
        {
            return ActiveDays?.Contains(dayOfWeek, StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
