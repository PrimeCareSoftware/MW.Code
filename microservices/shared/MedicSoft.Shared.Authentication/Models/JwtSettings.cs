namespace MedicSoft.Shared.Authentication.Models;

/// <summary>
/// JWT settings configuration model used by all microservices
/// </summary>
public class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = "MedicWarehouse";
    public string Audience { get; set; } = "MedicWarehouse-API";
    public int ExpiryMinutes { get; set; } = 60;
}
