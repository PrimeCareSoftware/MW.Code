namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Types of external services that can be configured
    /// </summary>
    public enum ExternalServiceType
    {
        // Email Services
        SendGrid = 1,
        MailGun = 2,
        AmazonSES = 3,
        
        // SMS Services
        Twilio = 10,
        AmazonSNS = 11,
        
        // Video/Telemedicine
        DailyCo = 20,
        Zoom = 21,
        
        // Analytics
        GoogleAnalytics = 30,
        MixPanel = 31,
        Segment = 32,
        
        // CRM & Sales
        Salesforce = 40,
        HubSpot = 41,
        
        // Payment Gateways
        Stripe = 50,
        PagSeguro = 51,
        MercadoPago = 52,
        
        // Accounting/Fiscal
        Dominio = 60,
        ContaAzul = 61,
        Omie = 62,
        
        // Cloud Storage
        AmazonS3 = 70,
        GoogleCloudStorage = 71,
        Azure = 72,
        
        // Other
        Other = 100
    }
}
