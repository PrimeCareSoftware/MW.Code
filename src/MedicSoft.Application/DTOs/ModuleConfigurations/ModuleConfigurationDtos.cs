using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MedicSoft.Application.DTOs.ModuleConfigurations
{
    /// <summary>
    /// Base class for all module-specific configurations
    /// </summary>
    public abstract class ModuleConfigurationBase
    {
        /// <summary>
        /// Indicates if the configuration is enabled
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Additional notes or comments about this configuration
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Validates the configuration
        /// </summary>
        public abstract IEnumerable<ValidationResult> Validate(ValidationContext validationContext);
    }

    /// <summary>
    /// Configuration for WhatsApp Integration module
    /// </summary>
    public class WhatsAppIntegrationConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// WhatsApp Business API Key
        /// </summary>
        [Required(ErrorMessage = "API Key é obrigatória")]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// WhatsApp Business Phone Number (with country code)
        /// </summary>
        [Required(ErrorMessage = "Número de telefone é obrigatório")]
        [Phone(ErrorMessage = "Número de telefone inválido")]
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Webhook URL for receiving messages
        /// </summary>
        [Url(ErrorMessage = "URL do webhook inválida")]
        public string? WebhookUrl { get; set; }

        /// <summary>
        /// Webhook verification token
        /// </summary>
        public string? WebhookToken { get; set; }

        /// <summary>
        /// Enable automatic appointment reminders
        /// </summary>
        public bool EnableAppointmentReminders { get; set; } = true;

        /// <summary>
        /// Hours before appointment to send reminder
        /// </summary>
        [Range(1, 168, ErrorMessage = "Horas de antecedência devem ser entre 1 e 168 (7 dias)")]
        public int ReminderHoursBefore { get; set; } = 24;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            
            if (string.IsNullOrWhiteSpace(ApiKey))
                results.Add(new ValidationResult("API Key é obrigatória", new[] { nameof(ApiKey) }));
            
            if (string.IsNullOrWhiteSpace(PhoneNumber))
                results.Add(new ValidationResult("Número de telefone é obrigatório", new[] { nameof(PhoneNumber) }));

            if (ReminderHoursBefore < 1 || ReminderHoursBefore > 168)
                results.Add(new ValidationResult("Horas de antecedência devem ser entre 1 e 168", new[] { nameof(ReminderHoursBefore) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for SMS Notifications module
    /// </summary>
    public class SMSNotificationsConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// SMS Provider (e.g., "Twilio", "Nexmo", "AWS SNS")
        /// </summary>
        [Required(ErrorMessage = "Provedor de SMS é obrigatório")]
        public string Provider { get; set; } = string.Empty;

        /// <summary>
        /// Provider API Key or Account SID
        /// </summary>
        [Required(ErrorMessage = "API Key é obrigatória")]
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Provider Auth Token or API Secret
        /// </summary>
        [Required(ErrorMessage = "Auth Token é obrigatório")]
        public string AuthToken { get; set; } = string.Empty;

        /// <summary>
        /// Sender ID or Phone Number
        /// </summary>
        [Required(ErrorMessage = "Sender ID é obrigatório")]
        public string SenderId { get; set; } = string.Empty;

        /// <summary>
        /// Enable appointment reminders via SMS
        /// </summary>
        public bool EnableAppointmentReminders { get; set; } = true;

        /// <summary>
        /// Enable payment reminders via SMS
        /// </summary>
        public bool EnablePaymentReminders { get; set; } = false;

        /// <summary>
        /// Maximum SMS per day (cost control)
        /// </summary>
        [Range(1, 10000, ErrorMessage = "Limite diário deve ser entre 1 e 10.000")]
        public int DailyLimit { get; set; } = 500;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(Provider))
                results.Add(new ValidationResult("Provedor de SMS é obrigatório", new[] { nameof(Provider) }));

            if (string.IsNullOrWhiteSpace(ApiKey))
                results.Add(new ValidationResult("API Key é obrigatória", new[] { nameof(ApiKey) }));

            if (string.IsNullOrWhiteSpace(AuthToken))
                results.Add(new ValidationResult("Auth Token é obrigatório", new[] { nameof(AuthToken) }));

            if (string.IsNullOrWhiteSpace(SenderId))
                results.Add(new ValidationResult("Sender ID é obrigatório", new[] { nameof(SenderId) }));

            if (DailyLimit < 1 || DailyLimit > 10000)
                results.Add(new ValidationResult("Limite diário deve ser entre 1 e 10.000", new[] { nameof(DailyLimit) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for TISS Export module
    /// </summary>
    public class TissExportConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// ANS Code (Registro na ANS)
        /// </summary>
        [Required(ErrorMessage = "Código ANS é obrigatório")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Código ANS deve ter 6 dígitos")]
        public string AnsCode { get; set; } = string.Empty;

        /// <summary>
        /// TISS version to use (e.g., "3.05.00", "4.00.00")
        /// </summary>
        [Required(ErrorMessage = "Versão TISS é obrigatória")]
        public string TissVersion { get; set; } = "3.05.00";

        /// <summary>
        /// Export directory path
        /// </summary>
        [Required(ErrorMessage = "Caminho de exportação é obrigatório")]
        public string ExportPath { get; set; } = string.Empty;

        /// <summary>
        /// Generate XML files
        /// </summary>
        public bool GenerateXml { get; set; } = true;

        /// <summary>
        /// Generate PDF files
        /// </summary>
        public bool GeneratePdf { get; set; } = true;

        /// <summary>
        /// Automatically sign XML files
        /// </summary>
        public bool AutoSignXml { get; set; } = false;

        /// <summary>
        /// Certificate thumbprint for signing (if AutoSignXml is true)
        /// </summary>
        public string? CertificateThumbprint { get; set; }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (string.IsNullOrWhiteSpace(AnsCode) || AnsCode.Length != 6)
                results.Add(new ValidationResult("Código ANS deve ter 6 dígitos", new[] { nameof(AnsCode) }));

            if (string.IsNullOrWhiteSpace(TissVersion))
                results.Add(new ValidationResult("Versão TISS é obrigatória", new[] { nameof(TissVersion) }));

            if (string.IsNullOrWhiteSpace(ExportPath))
                results.Add(new ValidationResult("Caminho de exportação é obrigatório", new[] { nameof(ExportPath) }));

            if (AutoSignXml && string.IsNullOrWhiteSpace(CertificateThumbprint))
                results.Add(new ValidationResult("Certificado digital é obrigatório quando assinatura automática está ativada", new[] { nameof(CertificateThumbprint) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for Inventory Management module
    /// </summary>
    public class InventoryManagementConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// Low stock threshold percentage
        /// </summary>
        [Range(1, 100, ErrorMessage = "Limite de estoque baixo deve ser entre 1% e 100%")]
        public int LowStockThresholdPercent { get; set; } = 20;

        /// <summary>
        /// Enable automatic low stock alerts
        /// </summary>
        public bool EnableLowStockAlerts { get; set; } = true;

        /// <summary>
        /// Email addresses to send alerts to (comma-separated)
        /// </summary>
        public string? AlertEmails { get; set; }

        /// <summary>
        /// Enable barcode scanning
        /// </summary>
        public bool EnableBarcodeScanning { get; set; } = false;

        /// <summary>
        /// Track expiration dates
        /// </summary>
        public bool TrackExpirationDates { get; set; } = true;

        /// <summary>
        /// Days before expiration to alert
        /// </summary>
        [Range(1, 365, ErrorMessage = "Dias antes do vencimento devem ser entre 1 e 365")]
        public int ExpirationAlertDays { get; set; } = 30;

        /// <summary>
        /// Enable batch/lot tracking
        /// </summary>
        public bool EnableBatchTracking { get; set; } = false;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (LowStockThresholdPercent < 1 || LowStockThresholdPercent > 100)
                results.Add(new ValidationResult("Limite de estoque baixo deve ser entre 1% e 100%", new[] { nameof(LowStockThresholdPercent) }));

            if (TrackExpirationDates && (ExpirationAlertDays < 1 || ExpirationAlertDays > 365))
                results.Add(new ValidationResult("Dias antes do vencimento devem ser entre 1 e 365", new[] { nameof(ExpirationAlertDays) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for Advanced Reports module
    /// </summary>
    public class ReportsConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// Default report format (e.g., "PDF", "Excel", "CSV")
        /// </summary>
        [Required(ErrorMessage = "Formato padrão é obrigatório")]
        public string DefaultFormat { get; set; } = "PDF";

        /// <summary>
        /// Enable automatic report generation
        /// </summary>
        public bool EnableAutomaticGeneration { get; set; } = false;

        /// <summary>
        /// Schedule for automatic generation (Cron expression)
        /// </summary>
        public string? AutoGenerationSchedule { get; set; }

        /// <summary>
        /// Email recipients for automatic reports (comma-separated)
        /// </summary>
        public string? AutoReportRecipients { get; set; }

        /// <summary>
        /// Enable custom report templates
        /// </summary>
        public bool EnableCustomTemplates { get; set; } = true;

        /// <summary>
        /// Maximum report retention days
        /// </summary>
        [Range(1, 365, ErrorMessage = "Retenção deve ser entre 1 e 365 dias")]
        public int RetentionDays { get; set; } = 90;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            var validFormats = new[] { "PDF", "Excel", "CSV", "HTML" };
            if (!validFormats.Contains(DefaultFormat))
                results.Add(new ValidationResult("Formato deve ser PDF, Excel, CSV ou HTML", new[] { nameof(DefaultFormat) }));

            if (EnableAutomaticGeneration && string.IsNullOrWhiteSpace(AutoReportRecipients))
                results.Add(new ValidationResult("Destinatários são obrigatórios quando geração automática está ativada", new[] { nameof(AutoReportRecipients) }));

            if (RetentionDays < 1 || RetentionDays > 365)
                results.Add(new ValidationResult("Retenção deve ser entre 1 e 365 dias", new[] { nameof(RetentionDays) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for Waiting Queue module
    /// </summary>
    public class WaitingQueueConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// Enable automatic queue progression
        /// </summary>
        public bool EnableAutoProgression { get; set; } = true;

        /// <summary>
        /// Enable SMS notifications for queue position
        /// </summary>
        public bool EnableSmsNotifications { get; set; } = false;

        /// <summary>
        /// Enable display screen for waiting room
        /// </summary>
        public bool EnableDisplayScreen { get; set; } = true;

        /// <summary>
        /// Display screen refresh interval in seconds
        /// </summary>
        [Range(5, 300, ErrorMessage = "Intervalo de atualização deve ser entre 5 e 300 segundos")]
        public int DisplayRefreshSeconds { get; set; } = 30;

        /// <summary>
        /// Maximum waiting time in minutes before alert
        /// </summary>
        [Range(5, 480, ErrorMessage = "Tempo máximo de espera deve ser entre 5 e 480 minutos")]
        public int MaxWaitingMinutes { get; set; } = 120;

        /// <summary>
        /// Enable priority queues
        /// </summary>
        public bool EnablePriorityQueues { get; set; } = true;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (DisplayRefreshSeconds < 5 || DisplayRefreshSeconds > 300)
                results.Add(new ValidationResult("Intervalo de atualização deve ser entre 5 e 300 segundos", new[] { nameof(DisplayRefreshSeconds) }));

            if (MaxWaitingMinutes < 5 || MaxWaitingMinutes > 480)
                results.Add(new ValidationResult("Tempo máximo de espera deve ser entre 5 e 480 minutos", new[] { nameof(MaxWaitingMinutes) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for Doctor Fields Config module
    /// </summary>
    public class DoctorFieldsConfigOptions : ModuleConfigurationBase
    {
        /// <summary>
        /// Custom fields enabled for medical records
        /// </summary>
        public bool EnableCustomFields { get; set; } = true;

        /// <summary>
        /// Maximum number of custom fields per specialty
        /// </summary>
        [Range(1, 50, ErrorMessage = "Número máximo de campos deve ser entre 1 e 50")]
        public int MaxCustomFieldsPerSpecialty { get; set; } = 20;

        /// <summary>
        /// Enable field templates
        /// </summary>
        public bool EnableFieldTemplates { get; set; } = true;

        /// <summary>
        /// Enable conditional fields (fields that appear based on other field values)
        /// </summary>
        public bool EnableConditionalFields { get; set; } = false;

        /// <summary>
        /// Enable field validation rules
        /// </summary>
        public bool EnableValidationRules { get; set; } = true;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MaxCustomFieldsPerSpecialty < 1 || MaxCustomFieldsPerSpecialty > 50)
                results.Add(new ValidationResult("Número máximo de campos deve ser entre 1 e 50", new[] { nameof(MaxCustomFieldsPerSpecialty) }));

            return results;
        }
    }

    /// <summary>
    /// Configuration for Chat module
    /// </summary>
    public class ChatConfig : ModuleConfigurationBase
    {
        /// <summary>
        /// Enable file sharing in chat
        /// </summary>
        public bool EnableFileSharing { get; set; } = true;

        /// <summary>
        /// Maximum file size in MB
        /// </summary>
        [Range(1, 100, ErrorMessage = "Tamanho máximo do arquivo deve ser entre 1 e 100 MB")]
        public int MaxFileSizeMB { get; set; } = 10;

        /// <summary>
        /// Enable message history
        /// </summary>
        public bool EnableMessageHistory { get; set; } = true;

        /// <summary>
        /// Message history retention in days
        /// </summary>
        [Range(1, 365, ErrorMessage = "Retenção deve ser entre 1 e 365 dias")]
        public int MessageRetentionDays { get; set; } = 90;

        /// <summary>
        /// Enable read receipts
        /// </summary>
        public bool EnableReadReceipts { get; set; } = true;

        /// <summary>
        /// Enable typing indicators
        /// </summary>
        public bool EnableTypingIndicators { get; set; } = true;

        /// <summary>
        /// Enable group chats
        /// </summary>
        public bool EnableGroupChats { get; set; } = true;

        /// <summary>
        /// Maximum members per group chat
        /// </summary>
        [Range(2, 100, ErrorMessage = "Número máximo de membros deve ser entre 2 e 100")]
        public int MaxGroupMembers { get; set; } = 20;

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            if (MaxFileSizeMB < 1 || MaxFileSizeMB > 100)
                results.Add(new ValidationResult("Tamanho máximo do arquivo deve ser entre 1 e 100 MB", new[] { nameof(MaxFileSizeMB) }));

            if (MessageRetentionDays < 1 || MessageRetentionDays > 365)
                results.Add(new ValidationResult("Retenção deve ser entre 1 e 365 dias", new[] { nameof(MessageRetentionDays) }));

            if (EnableGroupChats && (MaxGroupMembers < 2 || MaxGroupMembers > 100))
                results.Add(new ValidationResult("Número máximo de membros deve ser entre 2 e 100", new[] { nameof(MaxGroupMembers) }));

            return results;
        }
    }
}
