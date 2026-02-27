using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string username, string email, string password, string fullName,
            string phone, UserRole role, string tenantId, Guid? clinicId = null,
            string? professionalId = null, string? specialty = null, bool showInAppointmentScheduling = true);
        Task<User?> GetUserByIdAsync(Guid id, string tenantId);
        Task<User?> GetUserByUsernameAsync(string username, string tenantId);
        Task<IEnumerable<User>> GetUsersByClinicIdAsync(Guid clinicId, string tenantId);
        Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId);
        Task UpdateUserProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null, bool? showInAppointmentScheduling = null);
        Task ChangeUserRoleAsync(Guid id, UserRole newRole, string tenantId);
        Task ChangeUserPasswordAsync(Guid id, string newPassword, string tenantId);
        Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string tenantId);
        Task ActivateUserAsync(Guid id, string tenantId);
        Task DeactivateUserAsync(Guid id, string tenantId);
        Task<bool> UsernameExistsAsync(string username, string tenantId);
        Task<DoctorFieldsConfiguration> GetDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId);
        Task UpdateDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId, DoctorFieldsConfiguration configuration);
        
        // User-Clinic link management methods for Phase 4
        Task<UserClinicLink> LinkUserToClinicAsync(Guid userId, Guid clinicId, string tenantId, bool isPreferred = false);
        Task RemoveUserClinicLinkAsync(Guid userId, Guid clinicId, string tenantId);
        Task SetPreferredClinicAsync(Guid userId, Guid clinicId, string tenantId);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IModuleConfigurationRepository _moduleConfigurationRepository;
        private readonly IUserClinicLinkRepository _userClinicLinkRepository;
        private readonly IClinicRepository _clinicRepository;

        public UserService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            IModuleConfigurationRepository moduleConfigurationRepository,
            IUserClinicLinkRepository userClinicLinkRepository,
            IClinicRepository clinicRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _moduleConfigurationRepository = moduleConfigurationRepository;
            _userClinicLinkRepository = userClinicLinkRepository;
            _clinicRepository = clinicRepository;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string fullName,
            string phone, UserRole role, string tenantId, Guid? clinicId = null,
            string? professionalId = null, string? specialty = null, bool showInAppointmentScheduling = true)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

            // Validate professional fields for attending clinical roles
            if (IsAttendingClinicalRole(role) && clinicId.HasValue)
            {
                var doctorConfig = await GetDoctorFieldsConfigurationAsync(clinicId.Value, tenantId);

                if (doctorConfig.ProfessionalIdRequired && string.IsNullOrWhiteSpace(professionalId))
                    throw new InvalidOperationException("Professional registration is required for attending professionals in this clinic");

                if (doctorConfig.SpecialtyRequired && string.IsNullOrWhiteSpace(specialty))
                    throw new InvalidOperationException("Specialty is required for attending professionals in this clinic");
            }

            var passwordHash = _passwordHasher.HashPassword(password);
            var user = new User(username, email, passwordHash, fullName, phone, role, tenantId, 
                clinicId, professionalId, specialty, showInAppointmentScheduling);
            
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User?> GetUserByIdAsync(Guid id, string tenantId)
        {
            return await _userRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<User?> GetUserByUsernameAsync(string username, string tenantId)
        {
            return await _userRepository.GetUserByUsernameAsync(username, tenantId);
        }

        public async Task<IEnumerable<User>> GetUsersByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _userRepository.GetByClinicIdAsync(clinicId, tenantId);
        }

        public async Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _userRepository.GetUserCountByClinicIdAsync(clinicId, tenantId);
        }

        public async Task UpdateUserProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null, bool? showInAppointmentScheduling = null)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Validate professional fields for attending clinical roles
            if (IsAttendingClinicalRole(user.Role) && user.ClinicId.HasValue)
            {
                var doctorConfig = await GetDoctorFieldsConfigurationAsync(user.ClinicId.Value, tenantId);

                if (doctorConfig.ProfessionalIdRequired && string.IsNullOrWhiteSpace(professionalId))
                    throw new InvalidOperationException("Professional registration is required for attending professionals in this clinic");

                if (doctorConfig.SpecialtyRequired && string.IsNullOrWhiteSpace(specialty))
                    throw new InvalidOperationException("Specialty is required for attending professionals in this clinic");
            }

            user.UpdateProfile(email, fullName, phone, professionalId, specialty, showInAppointmentScheduling);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangeUserRoleAsync(Guid id, UserRole newRole, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.ChangeRole(newRole);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ActivateUserAsync(Guid id, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Activate();
            await _userRepository.UpdateAsync(user);
        }

        public async Task DeactivateUserAsync(Guid id, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            user.Deactivate();
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangeUserPasswordAsync(Guid id, string newPassword, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var passwordHash = _passwordHasher.HashPassword(newPassword);
            user.UpdatePassword(passwordHash);
            await _userRepository.UpdateAsync(user);
        }

        public async Task ChangePasswordAsync(Guid userId, string currentPassword, string newPassword, string tenantId)
        {
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
                throw new InvalidOperationException("Usuário não encontrado");

            // Verify current password
            if (!_passwordHasher.VerifyPassword(currentPassword, user.PasswordHash))
                throw new UnauthorizedAccessException("Senha atual incorreta");

            // Validate new password strength
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(newPassword, 8);
            if (!isValid)
                throw new InvalidOperationException(errorMessage);

            // Update password
            var newPasswordHash = _passwordHasher.HashPassword(newPassword);
            user.UpdatePassword(newPasswordHash);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<bool> UsernameExistsAsync(string username, string tenantId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            return user != null;
        }

        public async Task<DoctorFieldsConfiguration> GetDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId)
        {
            var moduleConfig = await _moduleConfigurationRepository.GetByClinicAndModuleAsync(
                clinicId, SystemModules.DoctorFieldsConfig, tenantId);

            if (moduleConfig == null || string.IsNullOrWhiteSpace(moduleConfig.Configuration))
            {
                // Return default configuration (both fields optional)
                return new DoctorFieldsConfiguration();
            }

            try
            {
                var config = JsonSerializer.Deserialize<DoctorFieldsConfiguration>(moduleConfig.Configuration);
                return config ?? new DoctorFieldsConfiguration();
            }
            catch
            {
                // If deserialization fails, return default configuration
                return new DoctorFieldsConfiguration();
            }
        }

        public async Task UpdateDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId, DoctorFieldsConfiguration configuration)
        {
            var moduleConfig = await _moduleConfigurationRepository.GetByClinicAndModuleAsync(
                clinicId, SystemModules.DoctorFieldsConfig, tenantId);

            var configJson = JsonSerializer.Serialize(configuration);

            if (moduleConfig == null)
            {
                // Create new configuration
                moduleConfig = new ModuleConfiguration(
                    clinicId, 
                    SystemModules.DoctorFieldsConfig, 
                    tenantId, 
                    true, 
                    configJson);
                await _moduleConfigurationRepository.AddAsync(moduleConfig);
            }
            else
            {
                // Update existing configuration
                moduleConfig.UpdateConfiguration(configJson);
                await _moduleConfigurationRepository.UpdateAsync(moduleConfig);
            }
        }

        /// <summary>
        /// Links a user to a clinic, allowing them to access that clinic.
        /// Validates that both user and clinic exist in the same tenant.
        /// </summary>
        public async Task<UserClinicLink> LinkUserToClinicAsync(Guid userId, Guid clinicId, string tenantId, bool isPreferred = false)
        {
            // Validate user exists
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user == null)
                throw new InvalidOperationException($"User with ID {userId} not found");

            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
            if (clinic == null)
                throw new InvalidOperationException($"Clinic with ID {clinicId} not found");

            // Check if link already exists
            var existingLink = await _userClinicLinkRepository.GetByUserAndClinicAsync(userId, clinicId, tenantId);
            if (existingLink != null)
            {
                // If link exists but is inactive, reactivate it
                if (!existingLink.IsActive)
                {
                    existingLink.Reactivate();
                    if (isPreferred)
                    {
                        existingLink.SetAsPreferred();
                    }
                    await _userClinicLinkRepository.UpdateAsync(existingLink);
                    return existingLink;
                }
                
                throw new InvalidOperationException($"User is already linked to this clinic");
            }

            // If this should be preferred, clear any existing preferred clinics
            if (isPreferred)
            {
                var existingLinks = await _userClinicLinkRepository.GetByUserIdAsync(userId, tenantId);
                var preferredLinks = existingLinks.Where(l => l.IsPreferredClinic).ToList();
                
                // Batch update all preferred links to remove preferred status
                foreach (var link in preferredLinks)
                {
                    link.RemoveAsPreferred();
                }
                
                // Save all changes at once if there are any
                if (preferredLinks.Any())
                {
                    foreach (var link in preferredLinks)
                    {
                        await _userClinicLinkRepository.UpdateAsync(link);
                    }
                }
            }

            // Create new link
            var newLink = new UserClinicLink(userId, clinicId, tenantId, isPreferred);
            await _userClinicLinkRepository.AddAsync(newLink);

            return newLink;
        }

        /// <summary>
        /// Removes the link between a user and a clinic by deactivating it.
        /// Does not delete the record for audit purposes.
        /// </summary>
        public async Task RemoveUserClinicLinkAsync(Guid userId, Guid clinicId, string tenantId)
        {
            var link = await _userClinicLinkRepository.GetByUserAndClinicAsync(userId, clinicId, tenantId);
            
            if (link == null)
                throw new InvalidOperationException($"No link found between user {userId} and clinic {clinicId}");

            if (!link.IsActive)
                throw new InvalidOperationException("Link is already inactive");

            // Deactivate the link
            link.Deactivate("Removed by administrator");
            await _userClinicLinkRepository.UpdateAsync(link);

            // If this was the user's current clinic, update User.CurrentClinicId
            var user = await _userRepository.GetByIdAsync(userId, tenantId);
            if (user != null && user.CurrentClinicId == clinicId)
            {
                // Get all user's clinic links (excluding the one being removed)
                var userLinks = await _userClinicLinkRepository.GetByUserIdAsync(userId, tenantId);
                var activeLinks = userLinks.Where(l => l.IsActive && l.ClinicId != clinicId).ToList();
                
                if (activeLinks.Any())
                {
                    // Set to preferred clinic first, otherwise first active clinic
                    var preferredClinic = activeLinks.FirstOrDefault(l => l.IsPreferredClinic);
                    var nextClinic = preferredClinic ?? activeLinks.FirstOrDefault();
                    if (nextClinic != null)
                    {
                        user.SetCurrentClinic(nextClinic.ClinicId);
                        await _userRepository.UpdateAsync(user);
                    }
                }
                // Note: If no active clinics remain, we leave CurrentClinicId as is.
                // The user won't be able to access the system until linked to a new clinic.
                // This is intentional for security - we don't clear it to maintain audit trail.
            }
        }

        /// <summary>
        /// Sets a clinic as the user's preferred (default) clinic.
        /// Clears any existing preferred clinic setting for this user.
        /// </summary>
        public async Task SetPreferredClinicAsync(Guid userId, Guid clinicId, string tenantId)
        {
            // Validate the user has access to this clinic
            var targetLink = await _userClinicLinkRepository.GetByUserAndClinicAsync(userId, clinicId, tenantId);
            
            if (targetLink == null)
                throw new InvalidOperationException($"User {userId} does not have access to clinic {clinicId}");

            if (!targetLink.IsActive)
                throw new InvalidOperationException("Cannot set an inactive clinic as preferred");

            // Clear existing preferred clinic and set new one
            var userLinks = await _userClinicLinkRepository.GetByUserIdAsync(userId, tenantId);
            var preferredLinks = userLinks.Where(l => l.IsPreferredClinic).ToList();
            
            // Batch update: remove preferred status from all existing preferred clinics
            foreach (var link in preferredLinks)
            {
                link.RemoveAsPreferred();
            }
            
            // Save all preference removals
            foreach (var link in preferredLinks)
            {
                await _userClinicLinkRepository.UpdateAsync(link);
            }

            // Set new preferred clinic
            targetLink.SetAsPreferred();
            await _userClinicLinkRepository.UpdateAsync(targetLink);
        }


        private static bool IsAttendingClinicalRole(UserRole role)
        {
            return role == UserRole.Doctor
                || role == UserRole.Dentist
                || role == UserRole.Nurse
                || role == UserRole.Psychologist;
        }
    }
}
