using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IOwnerService
    {
        Task<Owner> CreateOwnerAsync(string username, string email, string password, string fullName,
            string phone, string tenantId, Guid? clinicId = null, string? professionalId = null, string? specialty = null);
        Task<Owner?> GetOwnerByIdAsync(Guid id, string tenantId);
        Task<Owner?> GetOwnerByUsernameAsync(string username, string tenantId);
        Task<Owner?> GetOwnerByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Owner>> GetAllOwnersAsync(string tenantId);
        Task<IEnumerable<Owner>> GetSystemOwnersAsync(string tenantId);
        Task UpdateOwnerProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null);
        Task ActivateOwnerAsync(Guid id, string tenantId);
        Task DeactivateOwnerAsync(Guid id, string tenantId);
        Task<bool> ExistsByUsernameAsync(string username, string tenantId);
        Task<bool> ExistsByEmailAsync(string email, string tenantId);
    }

    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher _passwordHasher;

        public OwnerService(IOwnerRepository ownerRepository, IPasswordHasher passwordHasher)
        {
            _ownerRepository = ownerRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<Owner> CreateOwnerAsync(string username, string email, string password, string fullName,
            string phone, string tenantId, Guid? clinicId = null, string? professionalId = null, string? specialty = null)
        {
            // Check if username or email already exists
            if (await _ownerRepository.ExistsByUsernameAsync(username, tenantId))
                throw new InvalidOperationException("Username already exists");

            if (await _ownerRepository.ExistsByEmailAsync(email, tenantId))
                throw new InvalidOperationException("Email already exists");

            var passwordHash = _passwordHasher.HashPassword(password);
            var owner = new Owner(username, email, passwordHash, fullName, phone, tenantId, clinicId, professionalId, specialty);
            
            await _ownerRepository.AddAsync(owner);
            return owner;
        }

        public async Task<Owner?> GetOwnerByIdAsync(Guid id, string tenantId)
        {
            return await _ownerRepository.GetByIdAsync(id, tenantId);
        }

        public async Task<Owner?> GetOwnerByUsernameAsync(string username, string tenantId)
        {
            return await _ownerRepository.GetByUsernameAsync(username, tenantId);
        }

        public async Task<Owner?> GetOwnerByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _ownerRepository.GetByClinicIdAsync(clinicId, tenantId);
        }

        public async Task<IEnumerable<Owner>> GetAllOwnersAsync(string tenantId)
        {
            return await _ownerRepository.GetAllAsync(tenantId);
        }

        public async Task<IEnumerable<Owner>> GetSystemOwnersAsync(string tenantId)
        {
            var allOwners = await _ownerRepository.GetAllAsync(tenantId);
            return allOwners.Where(o => o.IsSystemOwner);
        }

        public async Task UpdateOwnerProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            owner.UpdateProfile(email, fullName, phone, professionalId, specialty);
            await _ownerRepository.UpdateAsync(owner);
        }

        public async Task ActivateOwnerAsync(Guid id, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            owner.Activate();
            await _ownerRepository.UpdateAsync(owner);
        }

        public async Task DeactivateOwnerAsync(Guid id, string tenantId)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            owner.Deactivate();
            await _ownerRepository.UpdateAsync(owner);
        }

        public async Task<bool> ExistsByUsernameAsync(string username, string tenantId)
        {
            return await _ownerRepository.ExistsByUsernameAsync(username, tenantId);
        }

        public async Task<bool> ExistsByEmailAsync(string email, string tenantId)
        {
            return await _ownerRepository.ExistsByEmailAsync(email, tenantId);
        }
    }
}
