using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.CRM;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// Stub implementation of IEmailService for development.
    /// Replace with actual email service implementation (SendGrid, AWS SES, etc.)
    /// </summary>
    public class StubEmailService : IEmailService
    {
        private readonly ILogger<StubEmailService> _logger;

        public StubEmailService(ILogger<StubEmailService> logger)
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            _logger.LogInformation("STUB: Sending email to {To} with subject: {Subject}", to, subject);
            _logger.LogDebug("Email body: {Body}", body);
            
            // In production, integrate with SendGrid, AWS SES, or similar service
            return Task.CompletedTask;
        }

        public Task SendEmailWithTemplateAsync(string to, Guid templateId, Dictionary<string, string> variables, string? tenantId = null)
        {
            _logger.LogInformation("STUB: Sending template email {TemplateId} to {To} (TenantId: {TenantId})", templateId, to, tenantId ?? "N/A");
            
            // In production, load template and render with variables
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Stub implementation of ISmsService for development.
    /// Replace with actual SMS service implementation (Twilio, AWS SNS, etc.)
    /// </summary>
    public class StubSmsService : ISmsService
    {
        private readonly ILogger<StubSmsService> _logger;

        public StubSmsService(ILogger<StubSmsService> logger)
        {
            _logger = logger;
        }

        public Task SendSmsAsync(string to, string message)
        {
            _logger.LogInformation("STUB: Sending SMS to {To}", to);
            _logger.LogDebug("SMS message: {Message}", message);
            
            // In production, integrate with Twilio, AWS SNS, or similar service
            return Task.CompletedTask;
        }
    }

    /// <summary>
    /// Stub implementation of IWhatsAppService for development.
    /// Replace with actual WhatsApp Business API implementation.
    /// </summary>
    public class StubWhatsAppService : IWhatsAppService
    {
        private readonly ILogger<StubWhatsAppService> _logger;

        public StubWhatsAppService(ILogger<StubWhatsAppService> logger)
        {
            _logger = logger;
        }

        public Task SendWhatsAppAsync(string to, string message)
        {
            _logger.LogInformation("STUB: Sending WhatsApp message to {To}", to);
            _logger.LogDebug("WhatsApp message: {Message}", message);
            
            // In production, integrate with WhatsApp Business API
            return Task.CompletedTask;
        }
    }
}
