using System.Threading.Tasks;
using MedicSoft.Application.Services.Reports;
using MedicSoft.Application.Services.CRM;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Services.Reports
{
    /// <summary>
    /// Adapter that implements Reports.IEmailService by delegating to CRM.IEmailService
    /// This allows SmartActionService and other Report services to use the CRM email infrastructure
    /// </summary>
    public class EmailServiceAdapter : MedicSoft.Application.Services.Reports.IEmailService
    {
        private readonly MedicSoft.Application.Services.CRM.IEmailService _crmEmailService;
        private readonly ILogger<EmailServiceAdapter> _logger;

        public EmailServiceAdapter(
            MedicSoft.Application.Services.CRM.IEmailService crmEmailService,
            ILogger<EmailServiceAdapter> logger)
        {
            _crmEmailService = crmEmailService;
            _logger = logger;
        }

        public async Task SendEmailAsync(
            string[] recipients,
            string subject,
            string body,
            byte[]? attachment = null,
            string? attachmentFileName = null,
            string? attachmentContentType = null)
        {
            if (recipients == null || recipients.Length == 0)
            {
                _logger.LogWarning("No recipients specified for email with subject: {Subject}", subject);
                return;
            }

            // Note: Current CRM IEmailService doesn't support attachments in the interface signature
            // If attachments are provided, log a warning
            if (attachment != null)
            {
                _logger.LogWarning(
                    "Email attachments not yet supported in CRM email service. " +
                    "Email will be sent without attachment. Subject: {Subject}, Attachment: {FileName}",
                    subject, attachmentFileName ?? "unknown");
            }

            // Send email to each recipient
            foreach (var recipient in recipients)
            {
                try
                {
                    await _crmEmailService.SendEmailAsync(recipient, subject, body);
                    _logger.LogInformation("Email sent successfully to {Recipient}", recipient);
                }
                catch (System.Exception ex)
                {
                    _logger.LogError(ex, "Failed to send email to {Recipient}", recipient);
                    // Continue sending to other recipients even if one fails
                }
            }
        }
    }
}
