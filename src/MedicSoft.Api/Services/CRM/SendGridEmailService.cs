using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Api.Configuration;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Interfaces;
using Polly;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// SendGrid implementation of IEmailService for production use
    /// Includes retry logic with exponential backoff for resilience
    /// </summary>
    public class SendGridEmailService : IEmailService
    {
        private readonly ILogger<SendGridEmailService> _logger;
        private readonly EmailConfiguration _config;
        private readonly ISendGridClient _sendGridClient;
        private readonly IEmailTemplateRepository _templateRepository;
        private readonly ResiliencePipeline _retryPolicy;

        public SendGridEmailService(
            ILogger<SendGridEmailService> logger,
            IOptions<MessagingConfiguration> messagingConfig,
            IEmailTemplateRepository templateRepository)
        {
            _logger = logger;
            _config = messagingConfig.Value.Email;
            _sendGridClient = new SendGridClient(_config.ApiKey);
            _templateRepository = templateRepository;
            _retryPolicy = ResiliencePolicies.CreateGenericRetryPolicy();
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("Email sending is disabled. Skipping email to {To} with subject: {Subject}", to, subject);
                return;
            }

            // Validate input parameters
            if (string.IsNullOrWhiteSpace(to))
                throw new ArgumentException("Email recipient (to) cannot be null or empty", nameof(to));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Email subject cannot be null or empty", nameof(subject));
            if (string.IsNullOrWhiteSpace(body))
                throw new ArgumentException("Email body cannot be null or empty", nameof(body));

            await _retryPolicy.ExecuteAsync(async cancellationToken =>
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
                    
                    // Throw exception for retry if it's a transient error (5xx or rate limit)
                    var statusCode = (int)response.StatusCode;
                    if (statusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        throw new TransientMessagingException($"Transient error sending email. Status: {response.StatusCode}");
                    }
                    
                    throw new Exception($"Failed to send email. Status: {response.StatusCode}");
                }
            }, CancellationToken.None);
        }

        public async Task SendEmailWithTemplateAsync(string to, Guid templateId, Dictionary<string, string> variables, string? tenantId = null)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("Email sending is disabled. Skipping template email {TemplateId} to {To}", templateId, to);
                return;
            }

            await _retryPolicy.ExecuteAsync(async cancellationToken =>
            {
                var from = new EmailAddress(_config.FromEmail, _config.FromName);
                var toAddress = new EmailAddress(to);

                // Build template content with variables (loads from database if tenantId provided)
                var (subject, templateContent) = await BuildTemplateContent(templateId, variables, tenantId);

                // For now, use a simple template approach
                // In production, this should integrate with SendGrid Dynamic Templates
                var msg = new SendGridMessage
                {
                    From = from,
                    Subject = subject
                };
                
                msg.AddTo(toAddress);
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
                    
                    // Throw exception for retry if it's a transient error
                    var statusCode = (int)response.StatusCode;
                    if (statusCode >= 500 || response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                    {
                        throw new TransientMessagingException($"Transient error sending template email. Status: {response.StatusCode}");
                    }
                    
                    throw new Exception($"Failed to send template email. Status: {response.StatusCode}");
                }
            }, CancellationToken.None);
        }

        private async Task<(string subject, string content)> BuildTemplateContent(Guid templateId, Dictionary<string, string> variables, string? tenantId)
        {
            string subject = "Notification from MedicSoft";
            string content;

            // Load template from database if tenantId is provided
            if (!string.IsNullOrEmpty(tenantId))
            {
                var template = await _templateRepository.GetByIdAsync(templateId, tenantId);
                
                if (template != null)
                {
                    subject = template.Subject;
                    content = template.HtmlBody;

                    // Replace template variables
                    if (variables != null && variables.Any())
                    {
                        foreach (var kvp in variables)
                        {
                            // Replace {{variable}} format with actual values
                            // HTML encode values to prevent XSS
                            var encodedValue = System.Net.WebUtility.HtmlEncode(kvp.Value);
                            var pattern = $@"{{\{{\s*{Regex.Escape(kvp.Key)}\s*\}}}}";
                            content = Regex.Replace(content, pattern, encodedValue, RegexOptions.IgnoreCase);
                            subject = Regex.Replace(subject, pattern, encodedValue, RegexOptions.IgnoreCase);
                        }
                    }

                    _logger.LogInformation("Template {TemplateId} loaded from database for tenant {TenantId}", templateId, tenantId);
                    return (subject, content);
                }

                _logger.LogWarning("Template {TemplateId} not found in database for tenant {TenantId}. Using fallback template.", templateId, tenantId);
            }

            // Fallback: return a simple template structure
            content = "<html><body>";
            content += "<h1>MedicSoft Notification</h1>";
            content += "<p>This is an automated notification.</p>";
            
            if (variables != null && variables.Any())
            {
                content += "<ul>";
                foreach (var kvp in variables)
                {
                    // HTML encode values to prevent XSS
                    var encodedKey = System.Net.WebUtility.HtmlEncode(kvp.Key);
                    var encodedValue = System.Net.WebUtility.HtmlEncode(kvp.Value);
                    content += $"<li><strong>{encodedKey}:</strong> {encodedValue}</li>";
                }
                content += "</ul>";
            }
            
            content += "</body></html>";
            
            return (subject, content);
        }
    }
}
