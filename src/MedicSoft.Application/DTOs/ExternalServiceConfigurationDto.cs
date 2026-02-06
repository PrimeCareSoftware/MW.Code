using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for reading external service configuration
    /// </summary>
    public class ExternalServiceConfigurationDto
    {
        public Guid Id { get; set; }
        public ExternalServiceType ServiceType { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        
        // Configuration fields (sensitive data masked)
        public bool HasApiKey { get; set; }
        public bool HasApiSecret { get; set; }
        public bool HasClientId { get; set; }
        public bool HasClientSecret { get; set; }
        public bool HasAccessToken { get; set; }
        public bool HasRefreshToken { get; set; }
        public DateTime? TokenExpiresAt { get; set; }
        public bool IsTokenExpired { get; set; }
        
        // Service-specific configuration
        public string? ApiUrl { get; set; }
        public string? WebhookUrl { get; set; }
        public string? AccountId { get; set; }
        public string? ProjectId { get; set; }
        public string? Region { get; set; }
        public string? AdditionalConfiguration { get; set; }
        
        // Metadata
        public DateTime? LastSyncAt { get; set; }
        public string? LastError { get; set; }
        public int ErrorCount { get; set; }
        public bool HasValidConfiguration { get; set; }
        
        // Tenant/Clinic
        public Guid? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for creating external service configuration
    /// </summary>
    public class CreateExternalServiceConfigurationDto
    {
        [Required]
        public ExternalServiceType ServiceType { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        public Guid? ClinicId { get; set; }
        
        // Configuration fields
        [MaxLength(1000)]
        public string? ApiKey { get; set; }
        
        [MaxLength(1000)]
        public string? ApiSecret { get; set; }
        
        [MaxLength(500)]
        public string? ClientId { get; set; }
        
        [MaxLength(1000)]
        public string? ClientSecret { get; set; }
        
        [MaxLength(2000)]
        public string? AccessToken { get; set; }
        
        [MaxLength(2000)]
        public string? RefreshToken { get; set; }
        
        public DateTime? TokenExpiresAt { get; set; }
        
        // Service-specific configuration
        [MaxLength(500)]
        public string? ApiUrl { get; set; }
        
        [MaxLength(500)]
        public string? WebhookUrl { get; set; }
        
        [MaxLength(200)]
        public string? AccountId { get; set; }
        
        [MaxLength(200)]
        public string? ProjectId { get; set; }
        
        [MaxLength(100)]
        public string? Region { get; set; }
        
        public string? AdditionalConfiguration { get; set; }
        
        public bool IsActive { get; set; } = true;
    }
    
    /// <summary>
    /// DTO for updating external service configuration
    /// </summary>
    public class UpdateExternalServiceConfigurationDto
    {
        [Required]
        [MaxLength(200)]
        public string ServiceName { get; set; } = string.Empty;
        
        [MaxLength(1000)]
        public string? Description { get; set; }
        
        // Configuration fields
        [MaxLength(1000)]
        public string? ApiKey { get; set; }
        
        [MaxLength(1000)]
        public string? ApiSecret { get; set; }
        
        [MaxLength(500)]
        public string? ClientId { get; set; }
        
        [MaxLength(1000)]
        public string? ClientSecret { get; set; }
        
        [MaxLength(2000)]
        public string? AccessToken { get; set; }
        
        [MaxLength(2000)]
        public string? RefreshToken { get; set; }
        
        public DateTime? TokenExpiresAt { get; set; }
        
        // Service-specific configuration
        [MaxLength(500)]
        public string? ApiUrl { get; set; }
        
        [MaxLength(500)]
        public string? WebhookUrl { get; set; }
        
        [MaxLength(200)]
        public string? AccountId { get; set; }
        
        [MaxLength(200)]
        public string? ProjectId { get; set; }
        
        [MaxLength(100)]
        public string? Region { get; set; }
        
        public string? AdditionalConfiguration { get; set; }
        
        public bool IsActive { get; set; }
    }
}
