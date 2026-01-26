namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Service for sending notifications via different channels
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Sends an email notification
    /// </summary>
    /// <param name="to">Recipient email address</param>
    /// <param name="subject">Email subject</param>
    /// <param name="body">Email body (HTML supported)</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an SMS notification
    /// </summary>
    /// <param name="phoneNumber">Recipient phone number</param>
    /// <param name="message">SMS message content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a WhatsApp notification
    /// </summary>
    /// <param name="phoneNumber">Recipient phone number</param>
    /// <param name="message">WhatsApp message content</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendWhatsAppAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
