using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Application.Services.CRM;

namespace MedicSoft.Application.Services.EmailService
{
    /// <summary>
    /// Configuration settings for SMTP email
    /// </summary>
    public class SmtpEmailSettings
    {
        public const string SectionName = "Email";

        public string SmtpServer { get; set; } = string.Empty;
        public int SmtpPort { get; set; } = 587;
        public bool UseSsl { get; set; } = true;
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string From { get; set; } = string.Empty;
        public string FromName { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// SMTP-based email service implementation for direct email sending
    /// Replaces external API dependencies like SendGrid with direct SMTP
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly ILogger<SmtpEmailService> _logger;
        private readonly SmtpEmailSettings _settings;

        public SmtpEmailService(
            ILogger<SmtpEmailService> logger,
            IOptions<SmtpEmailSettings> settings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _settings = settings?.Value ?? throw new ArgumentNullException(nameof(settings));
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (!_settings.Enabled)
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

            // Validate SMTP configuration
            if (string.IsNullOrWhiteSpace(_settings.SmtpServer))
            {
                _logger.LogError("SMTP server not configured. Cannot send email.");
                throw new InvalidOperationException("SMTP server not configured");
            }

            try
            {
                _logger.LogInformation("Sending email to {To} with subject: {Subject}", to, subject);

                using var message = new MailMessage
                {
                    From = new MailAddress(_settings.From, _settings.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                using var smtpClient = new SmtpClient(_settings.SmtpServer, _settings.SmtpPort)
                {
                    EnableSsl = _settings.UseSsl,
                    Timeout = _settings.TimeoutSeconds * 1000 // Convert to milliseconds
                };

                // Set credentials if provided
                if (!string.IsNullOrWhiteSpace(_settings.Username) && !string.IsNullOrWhiteSpace(_settings.Password))
                {
                    smtpClient.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
                }

                await smtpClient.SendMailAsync(message);

                _logger.LogInformation("Email sent successfully to {To}", to);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {To}. Status: {StatusCode}", to, ex.StatusCode);
                throw new InvalidOperationException($"Failed to send email: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error sending email to {To}", to);
                throw;
            }
        }

        public Task SendEmailWithTemplateAsync(string to, Guid templateId, Dictionary<string, string> variables, string? tenantId = null)
        {
            // For simple SMTP implementation, we'll use the basic SendEmailAsync
            // Template rendering should be done before calling this method
            _logger.LogWarning("Template-based email sending not implemented in SmtpEmailService. " +
                              "Please render the template before calling SendEmailAsync.");
            
            throw new NotImplementedException(
                "Template-based email is not supported in SmtpEmailService. " +
                "Please use SendEmailAsync with pre-rendered HTML body.");
        }
    }
}
