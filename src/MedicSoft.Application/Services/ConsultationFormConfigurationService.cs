using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Services
{
    public interface IConsultationFormConfigurationService
    {
        // Profile methods
        Task<ConsultationFormProfileDto> CreateProfileAsync(CreateConsultationFormProfileDto createDto, string tenantId);
        Task<ConsultationFormProfileDto> UpdateProfileAsync(Guid id, UpdateConsultationFormProfileDto updateDto, string tenantId);
        Task<ConsultationFormProfileDto?> GetProfileByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<ConsultationFormProfileDto>> GetProfilesBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId);
        Task<IEnumerable<ConsultationFormProfileDto>> GetActiveProfilesAsync(string tenantId);
        Task DeleteProfileAsync(Guid id, string tenantId);
        
        // Configuration methods
        Task<ConsultationFormConfigurationDto> CreateConfigurationAsync(CreateConsultationFormConfigurationDto createDto, string tenantId);
        Task<ConsultationFormConfigurationDto> UpdateConfigurationAsync(Guid id, UpdateConsultationFormConfigurationDto updateDto, string tenantId);
        Task<ConsultationFormConfigurationDto?> GetConfigurationByIdAsync(Guid id, string tenantId);
        Task<ConsultationFormConfigurationDto?> GetActiveConfigurationByClinicIdAsync(Guid clinicId, string tenantId);
        Task DeleteConfigurationAsync(Guid id, string tenantId);
        
        // Helper method to create configuration from profile
        Task<ConsultationFormConfigurationDto> CreateConfigurationFromProfileAsync(Guid clinicId, Guid profileId, string tenantId);
    }

    public class ConsultationFormConfigurationService : IConsultationFormConfigurationService
    {
        private readonly IConsultationFormProfileRepository _profileRepository;
        private readonly IConsultationFormConfigurationRepository _configurationRepository;

        public ConsultationFormConfigurationService(
            IConsultationFormProfileRepository profileRepository,
            IConsultationFormConfigurationRepository configurationRepository)
        {
            _profileRepository = profileRepository;
            _configurationRepository = configurationRepository;
        }

        // Profile methods
        public async Task<ConsultationFormProfileDto> CreateProfileAsync(CreateConsultationFormProfileDto createDto, string tenantId)
        {
            var customFields = createDto.CustomFields?.Select(cf => new CustomField(
                cf.FieldKey,
                cf.Label,
                cf.FieldType,
                cf.IsRequired,
                cf.DisplayOrder,
                cf.Placeholder,
                cf.DefaultValue,
                cf.HelpText,
                cf.Options
            )).ToList();

            var profile = new ConsultationFormProfile(
                createDto.Name,
                createDto.Description,
                createDto.Specialty,
                tenantId,
                createDto.ShowChiefComplaint,
                createDto.ShowHistoryOfPresentIllness,
                createDto.ShowPastMedicalHistory,
                createDto.ShowFamilyHistory,
                createDto.ShowLifestyleHabits,
                createDto.ShowCurrentMedications,
                createDto.RequireChiefComplaint,
                createDto.RequireHistoryOfPresentIllness,
                createDto.RequirePastMedicalHistory,
                createDto.RequireFamilyHistory,
                createDto.RequireLifestyleHabits,
                createDto.RequireCurrentMedications,
                createDto.RequireClinicalExamination,
                createDto.RequireDiagnosticHypothesis,
                createDto.RequireInformedConsent,
                createDto.RequireTherapeuticPlan,
                customFields
            );

            await _profileRepository.AddAsync(profile);
            return MapToProfileDto(profile);
        }

        public async Task<ConsultationFormProfileDto> UpdateProfileAsync(Guid id, UpdateConsultationFormProfileDto updateDto, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            var customFields = updateDto.CustomFields?.Select(cf => new CustomField(
                cf.FieldKey,
                cf.Label,
                cf.FieldType,
                cf.IsRequired,
                cf.DisplayOrder,
                cf.Placeholder,
                cf.DefaultValue,
                cf.HelpText,
                cf.Options
            )).ToList();

            profile.Update(
                updateDto.Name,
                updateDto.Description,
                updateDto.ShowChiefComplaint,
                updateDto.ShowHistoryOfPresentIllness,
                updateDto.ShowPastMedicalHistory,
                updateDto.ShowFamilyHistory,
                updateDto.ShowLifestyleHabits,
                updateDto.ShowCurrentMedications,
                updateDto.RequireChiefComplaint,
                updateDto.RequireHistoryOfPresentIllness,
                updateDto.RequirePastMedicalHistory,
                updateDto.RequireFamilyHistory,
                updateDto.RequireLifestyleHabits,
                updateDto.RequireCurrentMedications,
                updateDto.RequireClinicalExamination,
                updateDto.RequireDiagnosticHypothesis,
                updateDto.RequireInformedConsent,
                updateDto.RequireTherapeuticPlan,
                customFields
            );

            await _profileRepository.UpdateAsync(profile);
            return MapToProfileDto(profile);
        }

        public async Task<ConsultationFormProfileDto?> GetProfileByIdAsync(Guid id, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            return profile != null ? MapToProfileDto(profile) : null;
        }

        public async Task<IEnumerable<ConsultationFormProfileDto>> GetProfilesBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            var profiles = await _profileRepository.GetBySpecialtyAsync(specialty, tenantId);
            return profiles.Select(MapToProfileDto);
        }

        public async Task<IEnumerable<ConsultationFormProfileDto>> GetActiveProfilesAsync(string tenantId)
        {
            var profiles = await _profileRepository.GetActiveProfilesAsync(tenantId);
            return profiles.Select(MapToProfileDto);
        }

        public async Task DeleteProfileAsync(Guid id, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            if (profile.IsSystemDefault)
                throw new InvalidOperationException("Cannot delete system default profiles");

            await _profileRepository.DeleteAsync(id, tenantId);
        }

        // Configuration methods
        public async Task<ConsultationFormConfigurationDto> CreateConfigurationAsync(CreateConsultationFormConfigurationDto createDto, string tenantId)
        {
            var customFields = createDto.CustomFields?.Select(cf => new CustomField(
                cf.FieldKey,
                cf.Label,
                cf.FieldType,
                cf.IsRequired,
                cf.DisplayOrder,
                cf.Placeholder,
                cf.DefaultValue,
                cf.HelpText,
                cf.Options
            )).ToList();

            var configuration = new ConsultationFormConfiguration(
                createDto.ClinicId,
                createDto.ConfigurationName,
                tenantId,
                createDto.ProfileId,
                createDto.ShowChiefComplaint,
                createDto.ShowHistoryOfPresentIllness,
                createDto.ShowPastMedicalHistory,
                createDto.ShowFamilyHistory,
                createDto.ShowLifestyleHabits,
                createDto.ShowCurrentMedications,
                createDto.RequireChiefComplaint,
                createDto.RequireHistoryOfPresentIllness,
                createDto.RequirePastMedicalHistory,
                createDto.RequireFamilyHistory,
                createDto.RequireLifestyleHabits,
                createDto.RequireCurrentMedications,
                createDto.RequireClinicalExamination,
                createDto.RequireDiagnosticHypothesis,
                createDto.RequireInformedConsent,
                createDto.RequireTherapeuticPlan,
                customFields
            );

            await _configurationRepository.AddAsync(configuration);
            
            // Load the profile if it exists
            if (createDto.ProfileId.HasValue)
            {
                var profile = await _profileRepository.GetByIdAsync(createDto.ProfileId.Value, tenantId);
                return MapToConfigurationDto(configuration, profile);
            }
            
            return MapToConfigurationDto(configuration, null);
        }

        public async Task<ConsultationFormConfigurationDto> UpdateConfigurationAsync(Guid id, UpdateConsultationFormConfigurationDto updateDto, string tenantId)
        {
            var configuration = await _configurationRepository.GetByIdAsync(id, tenantId);
            if (configuration == null)
                throw new InvalidOperationException("Configuration not found");

            var customFields = updateDto.CustomFields?.Select(cf => new CustomField(
                cf.FieldKey,
                cf.Label,
                cf.FieldType,
                cf.IsRequired,
                cf.DisplayOrder,
                cf.Placeholder,
                cf.DefaultValue,
                cf.HelpText,
                cf.Options
            )).ToList();

            configuration.Update(
                updateDto.ConfigurationName,
                updateDto.ShowChiefComplaint,
                updateDto.ShowHistoryOfPresentIllness,
                updateDto.ShowPastMedicalHistory,
                updateDto.ShowFamilyHistory,
                updateDto.ShowLifestyleHabits,
                updateDto.ShowCurrentMedications,
                updateDto.RequireChiefComplaint,
                updateDto.RequireHistoryOfPresentIllness,
                updateDto.RequirePastMedicalHistory,
                updateDto.RequireFamilyHistory,
                updateDto.RequireLifestyleHabits,
                updateDto.RequireCurrentMedications,
                updateDto.RequireClinicalExamination,
                updateDto.RequireDiagnosticHypothesis,
                updateDto.RequireInformedConsent,
                updateDto.RequireTherapeuticPlan,
                customFields
            );

            await _configurationRepository.UpdateAsync(configuration);
            
            // Load the profile if it exists
            ConsultationFormProfile? profile = null;
            if (configuration.ProfileId.HasValue)
            {
                profile = await _profileRepository.GetByIdAsync(configuration.ProfileId.Value, tenantId);
            }
            
            return MapToConfigurationDto(configuration, profile);
        }

        public async Task<ConsultationFormConfigurationDto?> GetConfigurationByIdAsync(Guid id, string tenantId)
        {
            var configuration = await _configurationRepository.GetByIdAsync(id, tenantId);
            if (configuration == null)
                return null;
                
            // Load the profile if it exists
            ConsultationFormProfile? profile = null;
            if (configuration.ProfileId.HasValue)
            {
                profile = await _profileRepository.GetByIdAsync(configuration.ProfileId.Value, tenantId);
            }
            
            return MapToConfigurationDto(configuration, profile);
        }

        public async Task<ConsultationFormConfigurationDto?> GetActiveConfigurationByClinicIdAsync(Guid clinicId, string tenantId)
        {
            var configuration = await _configurationRepository.GetActiveConfigurationByClinicIdAsync(clinicId, tenantId);
            if (configuration == null)
                return null;
                
            // Load the profile if it exists
            ConsultationFormProfile? profile = null;
            if (configuration.ProfileId.HasValue)
            {
                profile = await _profileRepository.GetByIdAsync(configuration.ProfileId.Value, tenantId);
            }
            
            return MapToConfigurationDto(configuration, profile);
        }

        public async Task DeleteConfigurationAsync(Guid id, string tenantId)
        {
            await _configurationRepository.DeleteAsync(id, tenantId);
        }

        public async Task<ConsultationFormConfigurationDto> CreateConfigurationFromProfileAsync(Guid clinicId, Guid profileId, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            var configuration = new ConsultationFormConfiguration(
                clinicId,
                $"Configuração baseada em {profile.Name}",
                tenantId,
                profileId,
                profile.ShowChiefComplaint,
                profile.ShowHistoryOfPresentIllness,
                profile.ShowPastMedicalHistory,
                profile.ShowFamilyHistory,
                profile.ShowLifestyleHabits,
                profile.ShowCurrentMedications,
                profile.RequireChiefComplaint,
                profile.RequireHistoryOfPresentIllness,
                profile.RequirePastMedicalHistory,
                profile.RequireFamilyHistory,
                profile.RequireLifestyleHabits,
                profile.RequireCurrentMedications,
                profile.RequireClinicalExamination,
                profile.RequireDiagnosticHypothesis,
                profile.RequireInformedConsent,
                profile.RequireTherapeuticPlan,
                profile.GetCustomFields()
            );

            await _configurationRepository.AddAsync(configuration);
            return MapToConfigurationDto(configuration, profile);
        }

        // Mapping methods
        private ConsultationFormProfileDto MapToProfileDto(ConsultationFormProfile profile)
        {
            return new ConsultationFormProfileDto
            {
                Id = profile.Id,
                Name = profile.Name,
                Description = profile.Description,
                Specialty = profile.Specialty,
                IsActive = profile.IsActive,
                IsSystemDefault = profile.IsSystemDefault,
                ShowChiefComplaint = profile.ShowChiefComplaint,
                ShowHistoryOfPresentIllness = profile.ShowHistoryOfPresentIllness,
                ShowPastMedicalHistory = profile.ShowPastMedicalHistory,
                ShowFamilyHistory = profile.ShowFamilyHistory,
                ShowLifestyleHabits = profile.ShowLifestyleHabits,
                ShowCurrentMedications = profile.ShowCurrentMedications,
                RequireChiefComplaint = profile.RequireChiefComplaint,
                RequireHistoryOfPresentIllness = profile.RequireHistoryOfPresentIllness,
                RequirePastMedicalHistory = profile.RequirePastMedicalHistory,
                RequireFamilyHistory = profile.RequireFamilyHistory,
                RequireLifestyleHabits = profile.RequireLifestyleHabits,
                RequireCurrentMedications = profile.RequireCurrentMedications,
                RequireClinicalExamination = profile.RequireClinicalExamination,
                RequireDiagnosticHypothesis = profile.RequireDiagnosticHypothesis,
                RequireInformedConsent = profile.RequireInformedConsent,
                RequireTherapeuticPlan = profile.RequireTherapeuticPlan,
                CustomFields = profile.GetCustomFields().Select(cf => new CustomFieldDto
                {
                    FieldKey = cf.FieldKey,
                    Label = cf.Label,
                    FieldType = cf.FieldType,
                    IsRequired = cf.IsRequired,
                    DisplayOrder = cf.DisplayOrder,
                    Placeholder = cf.Placeholder,
                    DefaultValue = cf.DefaultValue,
                    HelpText = cf.HelpText,
                    Options = cf.Options
                }).ToList(),
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt
            };
        }

        private ConsultationFormConfigurationDto MapToConfigurationDto(ConsultationFormConfiguration configuration, ConsultationFormProfile? profile)
        {
            return new ConsultationFormConfigurationDto
            {
                Id = configuration.Id,
                ClinicId = configuration.ClinicId,
                ProfileId = configuration.ProfileId,
                ConfigurationName = configuration.ConfigurationName,
                IsActive = configuration.IsActive,
                ShowChiefComplaint = configuration.ShowChiefComplaint,
                ShowHistoryOfPresentIllness = configuration.ShowHistoryOfPresentIllness,
                ShowPastMedicalHistory = configuration.ShowPastMedicalHistory,
                ShowFamilyHistory = configuration.ShowFamilyHistory,
                ShowLifestyleHabits = configuration.ShowLifestyleHabits,
                ShowCurrentMedications = configuration.ShowCurrentMedications,
                RequireChiefComplaint = configuration.RequireChiefComplaint,
                RequireHistoryOfPresentIllness = configuration.RequireHistoryOfPresentIllness,
                RequirePastMedicalHistory = configuration.RequirePastMedicalHistory,
                RequireFamilyHistory = configuration.RequireFamilyHistory,
                RequireLifestyleHabits = configuration.RequireLifestyleHabits,
                RequireCurrentMedications = configuration.RequireCurrentMedications,
                RequireClinicalExamination = configuration.RequireClinicalExamination,
                RequireDiagnosticHypothesis = configuration.RequireDiagnosticHypothesis,
                RequireInformedConsent = configuration.RequireInformedConsent,
                RequireTherapeuticPlan = configuration.RequireTherapeuticPlan,
                CustomFields = configuration.GetCustomFields().Select(cf => new CustomFieldDto
                {
                    FieldKey = cf.FieldKey,
                    Label = cf.Label,
                    FieldType = cf.FieldType,
                    IsRequired = cf.IsRequired,
                    DisplayOrder = cf.DisplayOrder,
                    Placeholder = cf.Placeholder,
                    DefaultValue = cf.DefaultValue,
                    HelpText = cf.HelpText,
                    Options = cf.Options
                }).ToList(),
                Profile = profile != null ? MapToProfileDto(profile) : null,
                CreatedAt = configuration.CreatedAt,
                UpdatedAt = configuration.UpdatedAt
            };
        }
    }
}
