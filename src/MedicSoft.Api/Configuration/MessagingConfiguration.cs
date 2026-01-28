namespace MedicSoft.Api.Configuration
{
    /// <summary>
    /// Configuration settings for messaging services (Email, SMS, WhatsApp)
    /// </summary>
    public class MessagingConfiguration
    {
        public const string SectionName = "Messaging";

        /// <summary>
        /// Email service configuration (SendGrid)
        /// </summary>
        public EmailConfiguration Email { get; set; } = new();

        /// <summary>
        /// SMS service configuration (Twilio)
        /// </summary>
        public SmsConfiguration Sms { get; set; } = new();

        /// <summary>
        /// WhatsApp service configuration (WhatsApp Business API)
        /// </summary>
        public WhatsAppConfiguration WhatsApp { get; set; } = new();
    }

    public class EmailConfiguration
    {
        /// <summary>
        /// SendGrid API Key
        /// </summary>
        public string ApiKey { get; set; } = string.Empty;

        /// <summary>
        /// Default sender email address
        /// </summary>
        public string FromEmail { get; set; } = string.Empty;

        /// <summary>
        /// Default sender name
        /// </summary>
        public string FromName { get; set; } = string.Empty;

        /// <summary>
        /// Use sandbox mode (for testing)
        /// </summary>
        public bool UseSandbox { get; set; } = false;

        /// <summary>
        /// Enable/disable email sending (useful for development)
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    public class SmsConfiguration
    {
        /// <summary>
        /// Twilio Account SID
        /// </summary>
        public string AccountSid { get; set; } = string.Empty;

        /// <summary>
        /// Twilio Auth Token
        /// </summary>
        public string AuthToken { get; set; } = string.Empty;

        /// <summary>
        /// Twilio phone number (sender)
        /// </summary>
        public string FromPhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Enable/disable SMS sending (useful for development)
        /// </summary>
        public bool Enabled { get; set; } = true;
    }

    public class WhatsAppConfiguration
    {
        /// <summary>
        /// WhatsApp Business API endpoint URL
        /// </summary>
        public string ApiUrl { get; set; } = string.Empty;

        /// <summary>
        /// WhatsApp Business API access token
        /// </summary>
        public string AccessToken { get; set; } = string.Empty;

        /// <summary>
        /// WhatsApp Business phone number ID
        /// </summary>
        public string PhoneNumberId { get; set; } = string.Empty;

        /// <summary>
        /// Enable/disable WhatsApp sending (useful for development)
        /// </summary>
        public bool Enabled { get; set; } = true;
    }
}
