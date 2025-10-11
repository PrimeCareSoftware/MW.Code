using System;

namespace MedicSoft.WhatsAppAgent.DTOs
{
    public class WhatsAppMessageDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class WhatsAppWebhookDto
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string MessageId { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class WhatsAppResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ResponseText { get; set; }
    }
}
