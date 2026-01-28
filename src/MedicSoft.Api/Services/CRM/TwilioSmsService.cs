using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Api.Configuration;
using MedicSoft.Application.Services.CRM;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// Twilio implementation of ISmsService for production use
    /// </summary>
    public class TwilioSmsService : ISmsService
    {
        private readonly ILogger<TwilioSmsService> _logger;
        private readonly SmsConfiguration _config;

        public TwilioSmsService(
            ILogger<TwilioSmsService> logger,
            IOptions<MessagingConfiguration> messagingConfig)
        {
            _logger = logger;
            _config = messagingConfig.Value.Sms;

            if (_config.Enabled && !string.IsNullOrEmpty(_config.AccountSid) && !string.IsNullOrEmpty(_config.AuthToken))
            {
                TwilioClient.Init(_config.AccountSid, _config.AuthToken);
            }
        }

        public async Task SendSmsAsync(string to, string message)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("SMS sending is disabled. Skipping SMS to {To}", to);
                return;
            }

            if (string.IsNullOrEmpty(_config.AccountSid) || string.IsNullOrEmpty(_config.AuthToken))
            {
                _logger.LogWarning("Twilio credentials not configured. Cannot send SMS to {To}", to);
                throw new InvalidOperationException("Twilio credentials not configured");
            }

            try
            {
                // Format phone number to E.164 format if needed
                var formattedTo = FormatPhoneNumber(to);
                var formattedFrom = FormatPhoneNumber(_config.FromPhoneNumber);

                var messageResource = await MessageResource.CreateAsync(
                    body: message,
                    from: new PhoneNumber(formattedFrom),
                    to: new PhoneNumber(formattedTo)
                );

                if (messageResource.ErrorCode.HasValue)
                {
                    _logger.LogError("Failed to send SMS to {To}. Error Code: {ErrorCode}, Message: {ErrorMessage}", 
                        to, messageResource.ErrorCode, messageResource.ErrorMessage);
                    throw new Exception($"Failed to send SMS. Error: {messageResource.ErrorMessage}");
                }

                _logger.LogInformation("SMS sent successfully to {To}. Message SID: {MessageSid}", to, messageResource.Sid);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending SMS to {To}", to);
                throw;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Remove common formatting characters
            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // If the number doesn't start with +, add Brazil country code (+55) as default
            if (!phoneNumber.StartsWith("+"))
            {
                // If it's a Brazilian number (10 or 11 digits), add +55
                if (cleaned.Length == 10 || cleaned.Length == 11)
                {
                    return $"+55{cleaned}";
                }
                // Otherwise, assume it needs + prefix
                return $"+{cleaned}";
            }

            return phoneNumber;
        }
    }
}
