namespace MedicSoft.Application.Configurations
{
    /// <summary>
    /// Payment gateway configuration settings
    /// </summary>
    public class PaymentGatewaySettings
    {
        public string Provider { get; set; } = "MercadoPago";
        public bool Enabled { get; set; }
        public MercadoPagoSettings MercadoPago { get; set; } = new();
        public bool EnableCreditCardPayments { get; set; }
        public bool EnablePixPayments { get; set; }
        public bool EnableBankSlipPayments { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// Mercado Pago specific configuration
    /// </summary>
    public class MercadoPagoSettings
    {
        public string AccessToken { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
        public string WebhookSecret { get; set; } = string.Empty;
        public string ApiUrl { get; set; } = "https://api.mercadopago.com";
        public bool Enabled { get; set; }
        public string NotificationUrl { get; set; } = string.Empty;
    }
}
