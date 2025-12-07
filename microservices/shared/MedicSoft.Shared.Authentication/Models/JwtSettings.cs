namespace MedicSoft.Shared.Authentication.Models;

/// <summary>
/// JWT settings configuration model used by all microservices
/// </summary>
public class JwtSettings
{
    private string _issuer = "MedicWarehouse";
    private string _audience = "MedicWarehouse-API";

    public string SecretKey { get; set; } = string.Empty;
    
    public string Issuer 
    { 
        get => _issuer;
        set => _issuer = string.IsNullOrWhiteSpace(value) ? "MedicWarehouse" : value;
    }
    
    public string Audience 
    { 
        get => _audience;
        set => _audience = string.IsNullOrWhiteSpace(value) ? "MedicWarehouse-API" : value;
    }
    
    public int ExpiryMinutes { get; set; } = 60;
}

/// <summary>
/// Session management settings
/// </summary>
public class SessionSettings
{
    /// <summary>
    /// Session expiration time in hours. Default is 24 hours.
    /// </summary>
    public int ExpiryHours { get; set; } = 24;
    
    /// <summary>
    /// Maximum number of concurrent sessions per user. 0 means unlimited.
    /// </summary>
    public int MaxConcurrentSessions { get; set; } = 0;
}
