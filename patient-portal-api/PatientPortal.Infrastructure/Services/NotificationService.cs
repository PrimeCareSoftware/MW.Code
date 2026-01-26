using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PatientPortal.Application.Configuration;
using PatientPortal.Application.Interfaces;

namespace PatientPortal.Infrastructure.Services;

/// <summary>
/// Implementation of notification service for sending emails, SMS, and WhatsApp messages
/// </summary>
public class NotificationService : INotificationService
{
    private readonly EmailSettings _emailSettings;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(
        IOptions<EmailSettings> emailSettings,
        ILogger<NotificationService> logger)
    {
        _emailSettings = emailSettings.Value;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sending email to {To} with subject: {Subject}", to, subject);

            // Validate email settings
            if (string.IsNullOrEmpty(_emailSettings.SmtpServer))
            {
                _logger.LogWarning("SMTP server not configured. Email not sent.");
                return;
            }

            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.From, _emailSettings.FromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(to);

            using var smtpClient = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.UseSsl
            };

            // Set credentials if provided
            if (!string.IsNullOrEmpty(_emailSettings.Username) && !string.IsNullOrEmpty(_emailSettings.Password))
            {
                smtpClient.Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password);
            }

            await smtpClient.SendMailAsync(message, cancellationToken);

            _logger.LogInformation("Email sent successfully to {To}", to);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending email to {To}", to);
            throw;
        }
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // Placeholder implementation for SMS
        // In production, integrate with Twilio, AWS SNS, or other SMS provider
        _logger.LogInformation("SMS to {PhoneNumber}: {Message} (Not implemented - placeholder only)", phoneNumber, message);
        return Task.CompletedTask;
    }

    public Task SendWhatsAppAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        // Placeholder implementation for WhatsApp
        // In production, integrate with Twilio WhatsApp API or WhatsApp Business API
        _logger.LogInformation("WhatsApp to {PhoneNumber}: {Message} (Not implemented - placeholder only)", phoneNumber, message);
        return Task.CompletedTask;
    }
}
