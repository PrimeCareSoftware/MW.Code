using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class ExternalServiceConfigurationService
    {
        private readonly IExternalServiceConfigurationRepository _repository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IDataEncryptionService _encryptionService;
        
        public ExternalServiceConfigurationService(
            IExternalServiceConfigurationRepository repository,
            IClinicRepository clinicRepository,
            IDataEncryptionService encryptionService)
        {
            _repository = repository;
            _clinicRepository = clinicRepository;
            _encryptionService = encryptionService;
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetAllAsync(string tenantId)
        {
            return await _repository.GetAllByTenantAsync(tenantId);
        }
        
        public async Task<ExternalServiceConfiguration?> GetByIdAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            
            // Decrypt sensitive fields before returning
            if (config != null)
            {
                await DecryptSensitiveFieldsAsync(config);
            }
            
            return config;
        }
        
        public async Task<ExternalServiceConfiguration?> GetByServiceTypeAsync(
            ExternalServiceType serviceType,
            string tenantId,
            Guid? clinicId = null)
        {
            var config = await _repository.GetByServiceTypeAsync(serviceType, tenantId, clinicId);
            
            // Decrypt sensitive fields before returning
            if (config != null)
            {
                await DecryptSensitiveFieldsAsync(config);
            }
            
            return config;
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetByClinicIdAsync(
            Guid clinicId,
            string tenantId)
        {
            return await _repository.GetByClinicIdAsync(clinicId, tenantId);
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetActiveServicesAsync(string tenantId)
        {
            return await _repository.GetActiveServicesAsync(tenantId);
        }
        
        public async Task<ExternalServiceConfiguration> CreateAsync(
            ExternalServiceType serviceType,
            string serviceName,
            string tenantId,
            Guid? clinicId = null,
            string? description = null)
        {
            // Verify clinic exists if clinicId is provided
            if (clinicId.HasValue)
            {
                var clinic = await _clinicRepository.GetByIdAsync(clinicId.Value, tenantId);
                if (clinic == null)
                    throw new InvalidOperationException($"Clinic with ID {clinicId} not found");
            }
            
            // Check if service type already exists for this tenant/clinic
            var existing = await _repository.ExistsByServiceTypeAsync(serviceType, tenantId, clinicId);
            if (existing)
                throw new InvalidOperationException($"Configuration for service type {serviceType} already exists for this tenant/clinic");
            
            var config = new ExternalServiceConfiguration(
                serviceType,
                serviceName,
                tenantId,
                clinicId,
                description);
            
            await _repository.AddAsync(config);
            
            return config;
        }
        
        public async Task UpdateAsync(
            Guid id,
            string serviceName,
            string? description,
            bool isActive,
            string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            config.UpdateBasicInfo(serviceName, description);
            
            if (isActive)
                config.Activate();
            else
                config.Deactivate();
            
            await _repository.UpdateAsync(config);
        }
        
        public async Task UpdateCredentialsAsync(
            Guid id,
            string tenantId,
            string? apiKey = null,
            string? apiSecret = null,
            string? clientId = null,
            string? clientSecret = null)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            // Encrypt sensitive data before storing
            var encryptedApiKey = !string.IsNullOrWhiteSpace(apiKey) 
                ? await _encryptionService.EncryptAsync(apiKey) 
                : null;
            var encryptedApiSecret = !string.IsNullOrWhiteSpace(apiSecret) 
                ? await _encryptionService.EncryptAsync(apiSecret) 
                : null;
            var encryptedClientSecret = !string.IsNullOrWhiteSpace(clientSecret) 
                ? await _encryptionService.EncryptAsync(clientSecret) 
                : null;
            
            config.UpdateApiCredentials(
                encryptedApiKey,
                encryptedApiSecret,
                clientId,
                encryptedClientSecret);
            
            await _repository.UpdateAsync(config);
        }
        
        public async Task UpdateTokensAsync(
            Guid id,
            string tenantId,
            string? accessToken,
            string? refreshToken = null,
            DateTime? expiresAt = null)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            // Encrypt tokens before storing
            var encryptedAccessToken = !string.IsNullOrWhiteSpace(accessToken) 
                ? await _encryptionService.EncryptAsync(accessToken) 
                : null;
            var encryptedRefreshToken = !string.IsNullOrWhiteSpace(refreshToken) 
                ? await _encryptionService.EncryptAsync(refreshToken) 
                : null;
            
            config.UpdateTokens(encryptedAccessToken, encryptedRefreshToken, expiresAt);
            
            await _repository.UpdateAsync(config);
        }
        
        public async Task UpdateServiceConfigurationAsync(
            Guid id,
            string tenantId,
            string? apiUrl = null,
            string? webhookUrl = null,
            string? accountId = null,
            string? projectId = null,
            string? region = null,
            string? additionalConfiguration = null)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            config.UpdateServiceConfiguration(apiUrl, webhookUrl, accountId, projectId, region);
            
            if (additionalConfiguration != null)
                config.UpdateAdditionalConfiguration(additionalConfiguration);
            
            await _repository.UpdateAsync(config);
        }
        
        public async Task DeleteAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            await _repository.DeleteAsync(config);
        }
        
        public async Task RecordSyncAsync(Guid id, string tenantId)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            config.RecordSync();
            await _repository.UpdateAsync(config);
        }
        
        public async Task RecordErrorAsync(Guid id, string tenantId, string errorMessage)
        {
            var config = await _repository.GetByIdAsync(id, tenantId);
            if (config == null)
                throw new InvalidOperationException($"External service configuration with ID {id} not found");
            
            config.RecordError(errorMessage);
            await _repository.UpdateAsync(config);
        }
        
        private async Task DecryptSensitiveFieldsAsync(ExternalServiceConfiguration config)
        {
            // Note: In practice, you may want to decrypt these fields only when specifically requested
            // For security, consider not decrypting in the general GetById method
            // This is a placeholder to show where decryption would occur
            
            // For now, we'll leave them encrypted and only show masked values in DTOs
            await Task.CompletedTask;
        }
    }
}
