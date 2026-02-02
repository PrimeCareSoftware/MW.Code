using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public class BusinessConfigurationService
    {
        private readonly IBusinessConfigurationRepository _repository;
        private readonly IClinicRepository _clinicRepository;
        
        public BusinessConfigurationService(
            IBusinessConfigurationRepository repository,
            IClinicRepository clinicRepository)
        {
            _repository = repository;
            _clinicRepository = clinicRepository;
        }
        
        public async Task<BusinessConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _repository.GetByClinicIdAsync(clinicId, tenantId);
        }
        
        public async Task<BusinessConfiguration> CreateAsync(
            Guid clinicId,
            BusinessType businessType,
            ProfessionalSpecialty primarySpecialty,
            string tenantId)
        {
            // Verify clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                throw new InvalidOperationException($"Clinic with ID {clinicId} not found");
            
            // Check if configuration already exists
            var existing = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            if (existing != null)
                throw new InvalidOperationException($"Business configuration already exists for clinic {clinicId}");
            
            var config = new BusinessConfiguration(clinicId, businessType, primarySpecialty, tenantId);
            await _repository.AddAsync(config);
            
            return config;
        }
        
        public async Task UpdateBusinessTypeAsync(Guid configId, BusinessType businessType, string tenantId)
        {
            var config = await _repository.GetByIdAsync(configId, tenantId);
            if (config == null)
                throw new InvalidOperationException($"Business configuration with ID {configId} not found");
            
            config.UpdateBusinessType(businessType);
            await _repository.UpdateAsync(config);
        }
        
        public async Task UpdatePrimarySpecialtyAsync(Guid configId, ProfessionalSpecialty primarySpecialty, string tenantId)
        {
            var config = await _repository.GetByIdAsync(configId, tenantId);
            if (config == null)
                throw new InvalidOperationException($"Business configuration with ID {configId} not found");
            
            config.UpdatePrimarySpecialty(primarySpecialty);
            await _repository.UpdateAsync(config);
        }
        
        public async Task UpdateFeatureAsync(Guid configId, string featureName, bool enabled, string tenantId)
        {
            var config = await _repository.GetByIdAsync(configId, tenantId);
            if (config == null)
                throw new InvalidOperationException($"Business configuration with ID {configId} not found");
            
            config.UpdateFeature(featureName, enabled);
            await _repository.UpdateAsync(config);
        }
        
        public async Task<bool> IsFeatureEnabledAsync(Guid clinicId, string featureName, string tenantId)
        {
            var config = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            if (config == null)
                return false;
            
            return config.IsFeatureEnabled(featureName);
        }
        
        public async Task<Dictionary<string, string>> GetTerminologyAsync(Guid clinicId, string tenantId)
        {
            var config = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            if (config == null)
            {
                // Return default terminology if no configuration exists
                return TerminologyMap.For(ProfessionalSpecialty.Medico).ToDictionary();
            }
            
            return TerminologyMap.For(config.PrimarySpecialty).ToDictionary();
        }
    }
}
