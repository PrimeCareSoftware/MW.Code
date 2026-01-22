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
            string? professionalId = null, string? specialty = null);
        Task<User?> GetUserByIdAsync(Guid id, string tenantId);
        Task<User?> GetUserByUsernameAsync(string username, string tenantId);
        Task<IEnumerable<User>> GetUsersByClinicIdAsync(Guid clinicId, string tenantId);
        Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId);
        Task UpdateUserProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null);
        Task ChangeUserRoleAsync(Guid id, UserRole newRole, string tenantId);
        Task ChangeUserPasswordAsync(Guid id, string newPassword, string tenantId);
        Task ActivateUserAsync(Guid id, string tenantId);
        Task DeactivateUserAsync(Guid id, string tenantId);
        Task<bool> UsernameExistsAsync(string username, string tenantId);
        Task<DoctorFieldsConfiguration> GetDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId);
        Task UpdateDoctorFieldsConfigurationAsync(Guid clinicId, string tenantId, DoctorFieldsConfiguration configuration);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IModuleConfigurationRepository _moduleConfigurationRepository;

        public UserService(
            IUserRepository userRepository, 
            IPasswordHasher passwordHasher,
            IModuleConfigurationRepository moduleConfigurationRepository)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _moduleConfigurationRepository = moduleConfigurationRepository;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string fullName,
            string phone, UserRole role, string tenantId, Guid? clinicId = null,
            string? professionalId = null, string? specialty = null)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

            // Validate doctor-specific fields if role is Doctor
            if (role == UserRole.Doctor && clinicId.HasValue)
            {
                var doctorConfig = await GetDoctorFieldsConfigurationAsync(clinicId.Value, tenantId);
                
                if (doctorConfig.ProfessionalIdRequired && string.IsNullOrWhiteSpace(professionalId))
                    throw new InvalidOperationException("Professional ID (CRM) is required for doctors in this clinic");
                
                if (doctorConfig.SpecialtyRequired && string.IsNullOrWhiteSpace(specialty))
                    throw new InvalidOperationException("Specialty is required for doctors in this clinic");
            }

            var passwordHash = _passwordHasher.HashPassword(password);
            var user = new User(username, email, passwordHash, fullName, phone, role, tenantId, 
                clinicId, professionalId, specialty);
            
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
            string tenantId, string? professionalId = null, string? specialty = null)
        {
            var user = await _userRepository.GetByIdAsync(id, tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            // Validate doctor-specific fields if user is a Doctor
            if (user.Role == UserRole.Doctor && user.ClinicId.HasValue)
            {
                var doctorConfig = await GetDoctorFieldsConfigurationAsync(user.ClinicId.Value, tenantId);
                
                if (doctorConfig.ProfessionalIdRequired && string.IsNullOrWhiteSpace(professionalId))
                    throw new InvalidOperationException("Professional ID (CRM) is required for doctors in this clinic");
                
                if (doctorConfig.SpecialtyRequired && string.IsNullOrWhiteSpace(specialty))
                    throw new InvalidOperationException("Specialty is required for doctors in this clinic");
            }

            user.UpdateProfile(email, fullName, phone, professionalId, specialty);
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
    }
}
