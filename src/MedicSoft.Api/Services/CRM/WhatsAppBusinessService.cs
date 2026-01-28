using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Api.Configuration;
using MedicSoft.Application.Services.CRM;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// WhatsApp Business API implementation of IWhatsAppService for production use
    /// </summary>
    public class WhatsAppBusinessService : IWhatsAppService
    {
        private readonly ILogger<WhatsAppBusinessService> _logger;
        private readonly WhatsAppConfiguration _config;
        private readonly HttpClient _httpClient;

        public WhatsAppBusinessService(
            ILogger<WhatsAppBusinessService> logger,
            IOptions<MessagingConfiguration> messagingConfig,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _config = messagingConfig.Value.WhatsApp;
            _httpClient = httpClientFactory.CreateClient();

            if (_config.Enabled && !string.IsNullOrEmpty(_config.AccessToken))
            {
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_config.AccessToken}");
            }
        }

        public async Task SendWhatsAppAsync(string to, string message)
        {
            if (!_config.Enabled)
            {
                _logger.LogInformation("WhatsApp sending is disabled. Skipping message to {To}", to);
                return;
            }

            if (string.IsNullOrEmpty(_config.ApiUrl) || string.IsNullOrEmpty(_config.AccessToken) || string.IsNullOrEmpty(_config.PhoneNumberId))
            {
                _logger.LogWarning("WhatsApp Business API not configured. Cannot send message to {To}", to);
                throw new InvalidOperationException("WhatsApp Business API not configured");
            }

            try
            {
                // Format phone number (remove non-digits and ensure it has country code)
                var formattedTo = FormatPhoneNumber(to);

                // Build WhatsApp API request
                var requestUrl = $"{_config.ApiUrl}/{_config.PhoneNumberId}/messages";
                var requestBody = new
                {
                    messaging_product = "whatsapp",
                    to = formattedTo,
                    type = "text",
                    text = new
                    {
                        body = message
                    }
                };

                var jsonContent = JsonSerializer.Serialize(requestBody);
                var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, httpContent);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("WhatsApp message sent successfully to {To}", to);
                }
                else
                {
                    _logger.LogError("Failed to send WhatsApp message to {To}. Status: {StatusCode}, Response: {Response}", 
                        to, response.StatusCode, responseContent);
                    throw new Exception($"Failed to send WhatsApp message. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending WhatsApp message to {To}", to);
                throw;
            }
        }

        private string FormatPhoneNumber(string phoneNumber)
        {
            // Remove all non-digit characters
            var cleaned = new string(phoneNumber.Where(char.IsDigit).ToArray());

            // WhatsApp requires number without + or leading zeros
            // If it's a Brazilian number (10 or 11 digits), add country code 55
            if (cleaned.Length == 10 || cleaned.Length == 11)
            {
                return $"55{cleaned}";
            }

            // If it already has country code (12+ digits), return as is
            if (cleaned.Length >= 12)
            {
                return cleaned;
            }

            // Default: prepend Brazil country code
            return $"55{cleaned}";
        }
    }
}
