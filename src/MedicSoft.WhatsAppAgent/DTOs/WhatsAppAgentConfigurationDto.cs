using System;

namespace MedicSoft.WhatsAppAgent.DTOs
{
    public class WhatsAppAgentConfigurationDto
    {
        public Guid Id { get; set; }
        public string? TenantId { get; set; }
        public string? ClinicName { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? AiModel { get; set; }
        public string? SystemPrompt { get; set; }
        public int MaxMessagesPerHour { get; set; }
        public bool IsActive { get; set; }
        public string? BusinessHoursStart { get; set; }
        public string? BusinessHoursEnd { get; set; }
        public string? ActiveDays { get; set; }
        public string? FallbackMessage { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateWhatsAppAgentConfigurationDto
    {
        public string? TenantId { get; set; }
        public string? ClinicName { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? WhatsAppApiKey { get; set; }
        public string? AiApiKey { get; set; }
        public string? AiModel { get; set; }
        public string? SystemPrompt { get; set; }
        public int MaxMessagesPerHour { get; set; } = 20;
        public string BusinessHoursStart { get; set; } = "08:00";
        public string BusinessHoursEnd { get; set; } = "18:00";
        public string ActiveDays { get; set; } = "Mon,Tue,Wed,Thu,Fri";
    }

    public class UpdateWhatsAppAgentConfigurationDto
    {
        public string? ClinicName { get; set; }
        public string? WhatsAppNumber { get; set; }
        public string? SystemPrompt { get; set; }
        public int MaxMessagesPerHour { get; set; }
        public string? BusinessHoursStart { get; set; }
        public string? BusinessHoursEnd { get; set; }
        public string? ActiveDays { get; set; }
        public string? FallbackMessage { get; set; }
    }

    public class UpdateApiKeysDto
    {
        public string? WhatsAppApiKey { get; set; }
        public string? AiApiKey { get; set; }
    }
}
