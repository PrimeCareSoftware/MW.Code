using System;
using System.Collections.Generic;
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
        Task ActivateUserAsync(Guid id, string tenantId);
        Task DeactivateUserAsync(Guid id, string tenantId);
        Task<bool> UsernameExistsAsync(string username, string tenantId);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User> CreateUserAsync(string username, string email, string password, string fullName,
            string phone, UserRole role, string tenantId, Guid? clinicId = null,
            string? professionalId = null, string? specialty = null)
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            if (existingUser != null)
                throw new InvalidOperationException("Username already exists");

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

        public async Task<bool> UsernameExistsAsync(string username, string tenantId)
        {
            var user = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            return user != null;
        }
    }
}
