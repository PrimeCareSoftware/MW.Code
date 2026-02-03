namespace MedicSoft.Application.DTOs.CRM
{
    /// <summary>
    /// Salesforce API configuration
    /// </summary>
    public class SalesforceConfiguration
    {
        public bool Enabled { get; set; }
        public string? InstanceUrl { get; set; }
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? SecurityToken { get; set; }
        public string? ApiVersion { get; set; } = "v57.0";
        public bool AutoSyncEnabled { get; set; }
        public int SyncIntervalMinutes { get; set; } = 60;
        public int MaxSyncAttempts { get; set; } = 3;
    }
}
