using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public interface IOwnerService
    {
        Task<Owner> CreateOwnerAsync(string username, string email, string password, string fullName,
            string phone, string tenantId, Guid? clinicId = null, string? professionalId = null, string? specialty = null, 
            string? performedBy = null, string? ipAddress = null);
        Task<Owner?> GetOwnerByIdAsync(Guid id, string tenantId);
        Task<Owner?> GetOwnerByUsernameAsync(string username, string tenantId);
        Task<Owner?> GetOwnerByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<Owner>> GetAllOwnersAsync(string tenantId);
        Task<IEnumerable<Owner>> GetSystemOwnersAsync(string tenantId);
        Task UpdateOwnerProfileAsync(Guid id, string email, string fullName, string phone, 
            string tenantId, string? professionalId = null, string? specialty = null,
            string? performedBy = null, string? ipAddress = null);
        Task ActivateOwnerAsync(Guid id, string tenantId, string? performedBy = null, string? ipAddress = null);
        Task DeactivateOwnerAsync(Guid id, string tenantId, string? performedBy = null, string? ipAddress = null);
        Task<bool> ExistsByUsernameAsync(string username, string tenantId);
        Task<bool> ExistsByEmailAsync(string email, string tenantId);
    }

    public class OwnerService : IOwnerService
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IAuditService _auditService;

        public OwnerService(IOwnerRepository ownerRepository, IPasswordHasher passwordHasher, IAuditService auditService)
        {
            _ownerRepository = ownerRepository;
            _passwordHasher = passwordHasher;
            _auditService = auditService;
        }

        public async Task<Owner> CreateOwnerAsync(string username, string email, string password, string fullName,
            string phone, string tenantId, Guid? clinicId = null, string? professionalId = null, string? specialty = null,
            string? performedBy = null, string? ipAddress = null)
        {
            // Check if username or email already exists
            if (await _ownerRepository.ExistsByUsernameAsync(username, tenantId))
                throw new InvalidOperationException("Username already exists");

            if (await _ownerRepository.ExistsByEmailAsync(email, tenantId))
                throw new InvalidOperationException("Email already exists");

            var passwordHash = _passwordHasher.HashPassword(password);
            var owner = new Owner(username, email, passwordHash, fullName, phone, tenantId, clinicId, professionalId, specialty);
            
            await _ownerRepository.AddAsync(owner);
            
            // Audit log - Owner creation
            if (_auditService != null)
            {
                await _auditService.LogDataModificationAsync(
                    userId: performedBy ?? "SYSTEM",
                    userName: performedBy ?? "SYSTEM",
                    userEmail: "",
                    entityType: "Owner",
                    entityId: owner.Id.ToString(),
                    entityDisplayName: $"{fullName} ({username})",
                    oldValues: new { },
                    newValues: new { 
                        Username = username, 
                        Email = email, 
                        FullName = fullName,
                        IsSystemOwner = !clinicId.HasValue,
                        ClinicId = clinicId
                    },
                    ipAddress: ipAddress ?? "unknown",
                    userAgent: "API",
                    requestPath: "/api/owners",
                    httpMethod: "POST",
                    tenantId: tenantId,
                    dataCategory: DataCategory.PERSONAL,
                    purpose: LgpdPurpose.LEGAL_OBLIGATION
                );
            }
            
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
            string tenantId, string? professionalId = null, string? specialty = null,
            string? performedBy = null, string? ipAddress = null)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            // Capture old values for audit
            var oldValues = new
            {
                Email = owner.Email,
                FullName = owner.FullName,
                Phone = owner.Phone,
                ProfessionalId = owner.ProfessionalId,
                Specialty = owner.Specialty
            };

            owner.UpdateProfile(email, fullName, phone, professionalId, specialty);
            await _ownerRepository.UpdateAsync(owner);
            
            // Audit log - Owner update
            if (_auditService != null)
            {
                await _auditService.LogDataModificationAsync(
                    userId: performedBy ?? "SYSTEM",
                    userName: performedBy ?? "SYSTEM",
                    userEmail: "",
                    entityType: "Owner",
                    entityId: owner.Id.ToString(),
                    entityDisplayName: $"{fullName} ({owner.Username})",
                    oldValues: oldValues,
                    newValues: new
                    {
                        Email = email,
                        FullName = fullName,
                        Phone = phone,
                        ProfessionalId = professionalId,
                        Specialty = specialty
                    },
                    ipAddress: ipAddress ?? "unknown",
                    userAgent: "API",
                    requestPath: $"/api/owners/{id}",
                    httpMethod: "PUT",
                    tenantId: tenantId,
                    dataCategory: DataCategory.PERSONAL,
                    purpose: LgpdPurpose.LEGAL_OBLIGATION
                );
            }
        }

        public async Task ActivateOwnerAsync(Guid id, string tenantId, string? performedBy = null, string? ipAddress = null)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            var wasActive = owner.IsActive;
            owner.Activate();
            await _ownerRepository.UpdateAsync(owner);
            
            // Audit log - Owner activation
            if (_auditService != null)
            {
                await _auditService.LogDataModificationAsync(
                    userId: performedBy ?? "SYSTEM",
                    userName: performedBy ?? "SYSTEM",
                    userEmail: "",
                    entityType: "Owner",
                    entityId: owner.Id.ToString(),
                    entityDisplayName: $"{owner.FullName} ({owner.Username})",
                    oldValues: new { IsActive = wasActive },
                    newValues: new { IsActive = true },
                    ipAddress: ipAddress ?? "unknown",
                    userAgent: "API",
                    requestPath: $"/api/owners/{id}/activate",
                    httpMethod: "POST",
                    tenantId: tenantId,
                    dataCategory: DataCategory.PERSONAL,
                    purpose: LgpdPurpose.LEGAL_OBLIGATION
                );
            }
        }

        public async Task DeactivateOwnerAsync(Guid id, string tenantId, string? performedBy = null, string? ipAddress = null)
        {
            var owner = await _ownerRepository.GetByIdAsync(id, tenantId);
            if (owner == null)
                throw new InvalidOperationException("Owner not found");

            var wasActive = owner.IsActive;
            owner.Deactivate();
            await _ownerRepository.UpdateAsync(owner);
            
            // Audit log - Owner deactivation (CRITICAL SECURITY EVENT)
            if (_auditService != null)
            {
                await _auditService.LogDataModificationAsync(
                    userId: performedBy ?? "SYSTEM",
                    userName: performedBy ?? "SYSTEM",
                    userEmail: "",
                    entityType: "Owner",
                    entityId: owner.Id.ToString(),
                    entityDisplayName: $"{owner.FullName} ({owner.Username})",
                    oldValues: new { IsActive = wasActive },
                    newValues: new { IsActive = false },
                    ipAddress: ipAddress ?? "unknown",
                    userAgent: "API",
                    requestPath: $"/api/owners/{id}/deactivate",
                    httpMethod: "POST",
                    tenantId: tenantId,
                    dataCategory: DataCategory.PERSONAL,
                    purpose: LgpdPurpose.LEGAL_OBLIGATION
                );
            }
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
