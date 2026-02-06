using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Centralized configuration for external services and API keys
    /// </summary>
    public class ExternalServiceConfiguration : BaseEntity
    {
        public ExternalServiceType ServiceType { get; private set; }
        public string ServiceName { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; }
        
        // Configuration fields (stored encrypted in database)
        public string? ApiKey { get; private set; }
        public string? ApiSecret { get; private set; }
        public string? ClientId { get; private set; }
        public string? ClientSecret { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? TokenExpiresAt { get; private set; }
        
        // Service-specific configuration
        public string? ApiUrl { get; private set; }
        public string? WebhookUrl { get; private set; }
        public string? AccountId { get; private set; }
        public string? ProjectId { get; private set; }
        public string? Region { get; private set; }
        
        // Additional configuration as JSON
        public string? AdditionalConfiguration { get; private set; }
        
        // Metadata
        public DateTime? LastSyncAt { get; private set; }
        public string? LastError { get; private set; }
        public int ErrorCount { get; private set; }
        
        // Tenant/Clinic specific
        public Guid? ClinicId { get; private set; }
        public Clinic? Clinic { get; private set; }
        
        private ExternalServiceConfiguration()
        {
            // EF Constructor
            ServiceName = null!;
        }
        
        public ExternalServiceConfiguration(
            ExternalServiceType serviceType,
            string serviceName,
            string tenantId,
            Guid? clinicId = null,
            string? description = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Service name cannot be empty", nameof(serviceName));
            
            ServiceType = serviceType;
            ServiceName = serviceName.Trim();
            Description = description?.Trim();
            ClinicId = clinicId;
            IsActive = true;
            ErrorCount = 0;
        }
        
        public void UpdateBasicInfo(string serviceName, string? description)
        {
            if (string.IsNullOrWhiteSpace(serviceName))
                throw new ArgumentException("Service name cannot be empty", nameof(serviceName));
            
            ServiceName = serviceName.Trim();
            Description = description?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateApiCredentials(
            string? apiKey = null,
            string? apiSecret = null,
            string? clientId = null,
            string? clientSecret = null)
        {
            ApiKey = apiKey?.Trim();
            ApiSecret = apiSecret?.Trim();
            ClientId = clientId?.Trim();
            ClientSecret = clientSecret?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateTokens(
            string? accessToken,
            string? refreshToken = null,
            DateTime? expiresAt = null)
        {
            AccessToken = accessToken?.Trim();
            RefreshToken = refreshToken?.Trim();
            TokenExpiresAt = expiresAt;
            UpdateTimestamp();
        }
        
        public void UpdateServiceConfiguration(
            string? apiUrl = null,
            string? webhookUrl = null,
            string? accountId = null,
            string? projectId = null,
            string? region = null)
        {
            ApiUrl = apiUrl?.Trim();
            WebhookUrl = webhookUrl?.Trim();
            AccountId = accountId?.Trim();
            ProjectId = projectId?.Trim();
            Region = region?.Trim();
            UpdateTimestamp();
        }
        
        public void UpdateAdditionalConfiguration(string? additionalConfig)
        {
            AdditionalConfiguration = additionalConfig?.Trim();
            UpdateTimestamp();
        }
        
        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }
        
        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
        
        public void RecordSync()
        {
            LastSyncAt = DateTime.UtcNow;
            ErrorCount = 0;
            LastError = null;
            UpdateTimestamp();
        }
        
        public void RecordError(string errorMessage)
        {
            LastError = errorMessage?.Trim();
            ErrorCount++;
            UpdateTimestamp();
        }
        
        public void ClearErrors()
        {
            ErrorCount = 0;
            LastError = null;
            UpdateTimestamp();
        }
        
        public bool IsTokenExpired()
        {
            return TokenExpiresAt.HasValue && TokenExpiresAt.Value <= DateTime.UtcNow;
        }
        
        public bool HasValidConfiguration()
        {
            return IsActive && 
                   ((!string.IsNullOrWhiteSpace(ApiKey) || 
                     !string.IsNullOrWhiteSpace(ClientId)) ||
                    !string.IsNullOrWhiteSpace(AccessToken));
        }
    }
}
