using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Api.Configuration;
using MedicSoft.Application.Services.CRM;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// SendGrid implementation of IEmailService for production use
    /// </summary>
    public class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly EmailConfiguration _config;
        private readonly ISendGridClient _sendGridClient;

        public SendGridEmailService(
            ILogger<SendGridEmailService> logger,
            IOptions<MessagingConfiguration> messagingConfig)
        {
            _logger = logger;
            _config = messagingConfig.Value.Email;
            _sendGridClient = new SendGridClient(_config.ApiKey);
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("Email sending is disabled. Skipping email to {To} with subject: {Subject}", to, subject);
                return;
            }

            try
            {
                var from = new EmailAddress(_config.FromEmail, _config.FromName);
                var toAddress = new EmailAddress(to);
                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, body, body);

                if (_config.UseSandbox)
                {
                    msg.MailSettings = new MailSettings
                    {
                        SandboxMode = new SandboxMode { Enable = true }
                    };
                }

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Error: {Error}", 
                        to, response.StatusCode, errorBody);
                    throw new Exception($"Failed to send email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {To} with subject: {Subject}", to, subject);
                throw;
            }
        }

        public async Task SendEmailWithTemplateAsync(string to, Guid templateId, Dictionary<string, string> variables)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("Email sending is disabled. Skipping template email {TemplateId} to {To}", templateId, to);
                return;
            }

            try
            {
                var from = new EmailAddress(_config.FromEmail, _config.FromName);
                var toAddress = new EmailAddress(to);

                // For now, use a simple template approach
                // In production, this should integrate with SendGrid Dynamic Templates
                var msg = new SendGridMessage
                {
                    From = from,
                    Subject = "Notification from MedicSoft"
                };
                
                msg.AddTo(toAddress);

                // Build template content with variables
                var templateContent = await BuildTemplateContent(templateId, variables);
                msg.AddContent(MimeType.Html, templateContent);

                if (_config.UseSandbox)
                {
                    msg.MailSettings = new MailSettings
                    {
                        SandboxMode = new SandboxMode { Enable = true }
                    };
                }

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Template email {TemplateId} sent successfully to {To}", templateId, to);
                }
                else
                {
                    var errorBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send template email {TemplateId} to {To}. Status: {StatusCode}, Error: {Error}", 
                        templateId, to, response.StatusCode, errorBody);
                    throw new Exception($"Failed to send template email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending template email {TemplateId} to {To}", templateId, to);
                throw;
            }
        }

        private async Task<string> BuildTemplateContent(Guid templateId, Dictionary<string, string> variables)
        {
            // TODO: Load template from database (EmailTemplate entity)
            // For now, return a simple template structure
            var content = "<html><body>";
            content += "<h1>MedicSoft Notification</h1>";
            content += "<p>This is an automated notification.</p>";
            
            if (variables != null && variables.Any())
            {
                content += "<ul>";
                foreach (var kvp in variables)
                {
                    content += $"<li><strong>{kvp.Key}:</strong> {kvp.Value}</li>";
                }
                content += "</ul>";
            }
            
            content += "</body></html>";
            
            return await Task.FromResult(content);
        }
    }
}
