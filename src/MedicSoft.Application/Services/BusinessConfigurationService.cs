using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    public class BusinessConfigurationService
    {
        private readonly IBusinessConfigurationRepository _repository;
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<BusinessConfigurationService> _logger;
        
        // Default number of rooms when MultiRoom is enabled
        private const int DefaultMultiRoomCount = 2;
        
        public BusinessConfigurationService(
            IBusinessConfigurationRepository repository,
            IClinicRepository clinicRepository,
            ILogger<BusinessConfigurationService> logger)
        {
            _repository = repository;
            _clinicRepository = clinicRepository;
            _logger = logger;
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
            
            // Sync relevant features with Clinic entity
            await SyncFeatureWithClinicAsync(config.ClinicId, featureName, enabled, tenantId);
        }
        
        /// <summary>
        /// Syncs specific business configuration features with corresponding Clinic properties
        /// </summary>
        private async Task SyncFeatureWithClinicAsync(Guid clinicId, string featureName, bool enabled, string tenantId)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
            {
                _logger.LogWarning("Cannot sync feature {FeatureName} to clinic {ClinicId}: Clinic not found", featureName, clinicId);
                return;
            }
            
            switch (featureName)
            {
                case "OnlineBooking":
                    // Sync OnlineBooking with EnableOnlineAppointmentScheduling
                    clinic.UpdateOnlineSchedulingSetting(enabled);
                    await _clinicRepository.UpdateAsync(clinic);
                    break;
                    
                case "MultiRoom":
                    // Sync MultiRoom with NumberOfRooms
                    // MultiRoom=true means multiple rooms (default to DefaultMultiRoomCount), false means 1 room
                    var numberOfRooms = enabled ? Math.Max(DefaultMultiRoomCount, clinic.NumberOfRooms) : 1;
                    clinic.UpdateNumberOfRooms(numberOfRooms);
                    await _clinicRepository.UpdateAsync(clinic);
                    break;
            }
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
        
        /// <summary>
        /// Syncs Clinic properties to BusinessConfiguration
        /// Called when clinic operational settings change
        /// </summary>
        public async Task SyncClinicPropertiesToBusinessConfigAsync(Guid clinicId, string tenantId)
        {
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
            {
                _logger.LogWarning("Cannot sync clinic {ClinicId} properties to business config: Clinic not found", clinicId);
                return;
            }
            
            var config = await _repository.GetByClinicIdAsync(clinicId, tenantId);
            if (config == null)
            {
                _logger.LogWarning("Cannot sync clinic {ClinicId} properties to business config: Business configuration not found", clinicId);
                return;
            }
            
            // Sync EnableOnlineAppointmentScheduling -> OnlineBooking
            if (config.OnlineBooking != clinic.EnableOnlineAppointmentScheduling)
            {
                config.UpdateFeature("OnlineBooking", clinic.EnableOnlineAppointmentScheduling);
            }
            
            // Sync NumberOfRooms -> MultiRoom (>1 rooms = true, 1 room = false)
            var shouldHaveMultiRoom = clinic.NumberOfRooms > 1;
            if (config.MultiRoom != shouldHaveMultiRoom)
            {
                config.UpdateFeature("MultiRoom", shouldHaveMultiRoom);
            }
            
            await _repository.UpdateAsync(config);
        }
    }
}
