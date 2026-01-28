using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services.Reports
{
    /// <summary>
    /// Interface for email service
    /// Used for sending scheduled reports
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Send email with optional attachment
        /// </summary>
        Task SendEmailAsync(
            string[] recipients,
            string subject,
            string body,
            byte[] attachment = null,
            string attachmentFileName = null,
            string attachmentContentType = null
        );
    }
}
