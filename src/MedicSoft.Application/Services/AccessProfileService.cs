using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Application.Services
{
    public interface IAccessProfileService
    {
        Task<AccessProfileDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<AccessProfileDto>> GetAllAsync(string tenantId);
        Task<IEnumerable<AccessProfileDto>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<AccessProfileDto> CreateAsync(CreateAccessProfileDto dto, string tenantId);
        Task<AccessProfileDto> UpdateAsync(Guid id, UpdateAccessProfileDto dto, string tenantId);
        Task DeleteAsync(Guid id, string tenantId);
        Task<IEnumerable<PermissionCategoryDto>> GetAllPermissionsAsync();
        Task AssignProfileToUserAsync(Guid userId, Guid profileId, string tenantId);
        Task<IEnumerable<AccessProfileDto>> CreateDefaultProfilesAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<AccessProfileDto>> CreateDefaultProfilesForClinicTypeAsync(Guid clinicId, string tenantId, ClinicType clinicType);
        Task<AccessProfileDto> SetConsultationFormProfileAsync(Guid profileId, Guid? consultationFormProfileId, string tenantId);
        Task<BackfillProfilesResult> BackfillMissingProfilesForAllClinicsAsync(string tenantId);
    }

    public class AccessProfileService : IAccessProfileService
    {
        private readonly IAccessProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IConsultationFormProfileRepository _consultationFormProfileRepository;
        private readonly IClinicRepository _clinicRepository;

        public AccessProfileService(
            IAccessProfileRepository profileRepository,
            IUserRepository userRepository,
            IConsultationFormProfileRepository consultationFormProfileRepository,
            IClinicRepository clinicRepository)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _consultationFormProfileRepository = consultationFormProfileRepository;
            _clinicRepository = clinicRepository;
        }

        public async Task<AccessProfileDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            return profile != null ? MapToDto(profile) : null;
        }

        public async Task<IEnumerable<AccessProfileDto>> GetAllAsync(string tenantId)
        {
            var profiles = await _profileRepository.GetAllAsync(tenantId);
            return profiles.Select(MapToDto);
        }

        public async Task<IEnumerable<AccessProfileDto>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            var profiles = await _profileRepository.GetByClinicIdAsync(clinicId, tenantId);
            var profileDtos = new List<AccessProfileDto>();

            foreach (var profile in profiles)
            {
                var dto = MapToDto(profile);
                // Count users assigned to this profile
                var users = await _userRepository.GetByClinicIdAsync(clinicId, tenantId);
                dto.UserCount = users.Count(u => u.ProfileId == profile.Id);
                profileDtos.Add(dto);
            }

            return profileDtos;
        }

        public async Task<AccessProfileDto> CreateAsync(CreateAccessProfileDto dto, string tenantId)
        {
            // Check if profile with same name exists in this clinic
            var existing = await _profileRepository.GetByNameAsync(dto.Name, dto.ClinicId, tenantId);
            if (existing != null)
                throw new InvalidOperationException($"Profile with name '{dto.Name}' already exists in this clinic");

            var profile = new AccessProfile(dto.Name, dto.Description, tenantId, dto.ClinicId, isDefault: false);
            
            // Set permissions
            if (dto.Permissions != null && dto.Permissions.Any())
            {
                profile.SetPermissions(dto.Permissions);
            }

            await _profileRepository.AddAsync(profile);
            return MapToDto(profile);
        }

        public async Task<AccessProfileDto> UpdateAsync(Guid id, UpdateAccessProfileDto dto, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            if (profile.IsDefault)
                throw new InvalidOperationException("Cannot modify default profiles");

            profile.Update(dto.Name, dto.Description);
            
            // Update permissions
            if (dto.Permissions != null)
            {
                profile.SetPermissions(dto.Permissions);
            }

            await _profileRepository.UpdateAsync(profile);
            return MapToDto(profile);
        }

        public async Task DeleteAsync(Guid id, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(id, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            if (profile.IsDefault)
                throw new InvalidOperationException("Cannot delete default profiles");

            // Check if profile is in use
            var isInUse = await _profileRepository.IsProfileInUseAsync(id, tenantId);
            if (isInUse)
                throw new InvalidOperationException("Cannot delete profile that is assigned to users");

            await _profileRepository.DeleteAsync(id, tenantId);
        }

        public Task<IEnumerable<PermissionCategoryDto>> GetAllPermissionsAsync()
        {
            var permissions = PermissionKeys.GetAllPermissionsByCategory();
            var result = permissions.Select(kvp => new PermissionCategoryDto
            {
                Category = kvp.Key,
                Permissions = kvp.Value.Select(p => new PermissionDto
                {
                    Key = p.Key,
                    Description = p.Description
                }).ToList()
            }).ToList();

            return Task.FromResult<IEnumerable<PermissionCategoryDto>>(result);
        }

        public async Task AssignProfileToUserAsync(Guid userId, Guid profileId, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Load profile with navigation properties to access ConsultationFormProfile
            var profile = await _profileRepository
                .GetAllQueryable()
                .Include(p => p.ConsultationFormProfile)
                .FirstOrDefaultAsync(p => p.Id == profileId && p.TenantId == tenantId);
            
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            if (!profile.IsActive)
                throw new InvalidOperationException("Cannot assign inactive profile");

            user.AssignProfile(profileId);
            
            // Synchronize professional specialty from profile's consultation form profile
            if (profile.ConsultationFormProfile != null)
            {
                user.SetProfessionalSpecialty(profile.ConsultationFormProfile.Specialty);
            }
            
            await _userRepository.UpdateAsync(user);
        }

        public async Task<IEnumerable<AccessProfileDto>> CreateDefaultProfilesAsync(Guid clinicId, string tenantId)
        {
            var profiles = new List<AccessProfile>
            {
                AccessProfile.CreateDefaultOwnerProfile(tenantId, clinicId),
                AccessProfile.CreateDefaultMedicalProfile(tenantId, clinicId),
                AccessProfile.CreateDefaultReceptionProfile(tenantId, clinicId),
                AccessProfile.CreateDefaultFinancialProfile(tenantId, clinicId)
            };

            var createdProfiles = new List<AccessProfileDto>();
            foreach (var profile in profiles)
            {
                // Check if default profile already exists
                var existing = await _profileRepository.GetByNameAsync(profile.Name, clinicId, tenantId);
                if (existing == null)
                {
                    await _profileRepository.AddAsync(profile);
                    createdProfiles.Add(MapToDto(profile));
                }
                else
                {
                    createdProfiles.Add(MapToDto(existing));
                }
            }

            return createdProfiles;
        }

        public async Task<IEnumerable<AccessProfileDto>> CreateDefaultProfilesForClinicTypeAsync(Guid clinicId, string tenantId, ClinicType clinicType)
        {
            // Get the appropriate consultation form profile for this clinic type
            var specialty = AccessProfile.GetProfessionalSpecialtyForClinicType(clinicType);
            var allSystemProfiles = await _consultationFormProfileRepository.GetSystemDefaultProfilesAsync("system");
            var consultationFormProfile = allSystemProfiles.FirstOrDefault(p => p.Specialty == specialty);

            var profiles = AccessProfile.GetDefaultProfilesForClinicType(tenantId, clinicId, clinicType);

            var createdProfiles = new List<AccessProfileDto>();
            foreach (var profile in profiles)
            {
                // Check if default profile already exists
                var existing = await _profileRepository.GetByNameAsync(profile.Name, clinicId, tenantId);
                if (existing == null)
                {
                    // Link consultation form profile to professional profiles only
                    if (consultationFormProfile != null && profile.IsProfessionalProfile())
                    {
                        profile.SetConsultationFormProfile(consultationFormProfile.Id);
                    }
                    
                    await _profileRepository.AddAsync(profile);
                    createdProfiles.Add(MapToDto(profile));
                }
                else
                {
                    createdProfiles.Add(MapToDto(existing));
                }
            }

            return createdProfiles;
        }

        public async Task<AccessProfileDto> SetConsultationFormProfileAsync(Guid profileId, Guid? consultationFormProfileId, string tenantId)
        {
            var profile = await _profileRepository.GetByIdAsync(profileId, tenantId);
            if (profile == null)
                throw new InvalidOperationException("Profile not found");

            // Validate consultation form profile exists if provided
            if (consultationFormProfileId.HasValue)
            {
                // Check both system and clinic-specific profiles in a single query
                var formProfile = await _consultationFormProfileRepository
                    .GetAllQueryable()
                    .Where(p => p.Id == consultationFormProfileId.Value && 
                               (p.TenantId == "system" || p.TenantId == tenantId))
                    .FirstOrDefaultAsync();
                
                if (formProfile == null)
                    throw new InvalidOperationException("Consultation form profile not found");
            }

            profile.SetConsultationFormProfile(consultationFormProfileId);
            await _profileRepository.UpdateAsync(profile);

            return MapToDto(profile);
        }

        /// <summary>
        /// Backfills missing default profiles for all clinics in the tenant.
        /// This ensures existing clinics have all professional profiles available, not just their clinic type's profile.
        /// </summary>
        public async Task<BackfillProfilesResult> BackfillMissingProfilesForAllClinicsAsync(string tenantId)
        {
            var result = new BackfillProfilesResult
            {
                ClinicsProcessed = 0,
                ProfilesCreated = 0,
                ProfilesSkipped = 0,
                ClinicDetails = new List<ClinicBackfillDetail>()
            };

            // Load all system consultation form profiles once to avoid repeated queries
            var allSystemFormProfiles = await _consultationFormProfileRepository.GetSystemDefaultProfilesAsync("system");
            
            // Map profile names to their corresponding specialties for consultation form linking
            var profileToSpecialtyMap = new Dictionary<string, ProfessionalSpecialty>
            {
                { "Médico", ProfessionalSpecialty.Medico },
                { "Dentista", ProfessionalSpecialty.Dentista },
                { "Nutricionista", ProfessionalSpecialty.Nutricionista },
                { "Psicólogo", ProfessionalSpecialty.Psicologo },
                { "Fisioterapeuta", ProfessionalSpecialty.Fisioterapeuta },
                { "Veterinário", ProfessionalSpecialty.Veterinario }
            };

            // Get all clinics in this tenant
            var clinics = await _clinicRepository.GetAllQueryable()
                .Where(c => c.TenantId == tenantId && c.IsActive)
                .ToListAsync();

            foreach (var clinic in clinics)
            {
                var clinicDetail = new ClinicBackfillDetail
                {
                    ClinicId = clinic.Id,
                    ClinicName = clinic.Name,
                    ProfilesCreated = new List<string>(),
                    ProfilesSkipped = new List<string>()
                };

                // Create all default profiles for this clinic
                var profiles = AccessProfile.GetDefaultProfilesForClinicType(tenantId, clinic.Id, clinic.ClinicType);

                foreach (var profile in profiles)
                {
                    // Check if profile already exists
                    var existing = await _profileRepository.GetByNameAsync(profile.Name, clinic.Id, tenantId);
                    if (existing == null)
                    {
                        // Link consultation form profile to professional profiles based on the profile's specialty, not the clinic type
                        if (profile.IsProfessionalProfile() && profileToSpecialtyMap.TryGetValue(profile.Name, out var specialty))
                        {
                            var consultationFormProfile = allSystemFormProfiles.FirstOrDefault(p => p.Specialty == specialty);
                            
                            if (consultationFormProfile != null)
                            {
                                profile.SetConsultationFormProfile(consultationFormProfile.Id);
                            }
                        }

                        await _profileRepository.AddAsync(profile);
                        clinicDetail.ProfilesCreated.Add(profile.Name);
                        result.ProfilesCreated++;
                    }
                    else
                    {
                        clinicDetail.ProfilesSkipped.Add(profile.Name);
                        result.ProfilesSkipped++;
                    }
                }

                result.ClinicDetails.Add(clinicDetail);
                result.ClinicsProcessed++;
            }

            return result;
        }

        private AccessProfileDto MapToDto(AccessProfile profile)
        {
            return new AccessProfileDto
            {
                Id = profile.Id,
                Name = profile.Name,
                Description = profile.Description,
                IsDefault = profile.IsDefault,
                IsActive = profile.IsActive,
                ClinicId = profile.ClinicId,
                ClinicName = profile.Clinic?.Name,
                ConsultationFormProfileId = profile.ConsultationFormProfileId,
                ConsultationFormProfileName = profile.ConsultationFormProfile?.Name,
                CreatedAt = profile.CreatedAt,
                UpdatedAt = profile.UpdatedAt,
                Permissions = profile.GetPermissionKeys().ToList()
            };
        }
    }
}
