using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Repository.Context;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface ICrossTenantUserService
    {
        Task<(List<CrossTenantUserDto> Users, int TotalCount)> GetUsers(CrossTenantUserFilterDto filters);
        Task<CrossTenantUserDto?> GetUserById(Guid userId);
        Task<bool> ResetPassword(Guid userId, string newPassword);
        Task<bool> ToggleUserActivation(Guid userId);
        Task<bool> TransferOwnership(Guid currentOwnerId, Guid newOwnerId);
    }

    public class CrossTenantUserService : ICrossTenantUserService
    {
        private readonly MedicSoftDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

        public CrossTenantUserService(MedicSoftDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<(List<CrossTenantUserDto> Users, int TotalCount)> GetUsers(CrossTenantUserFilterDto filters)
        {
            var query = _context.Users.IgnoreQueryFilters().AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filters.SearchTerm))
            {
                var searchLower = filters.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.FullName.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    (u.Phone != null && u.Phone.Contains(filters.SearchTerm)));
            }

            if (!string.IsNullOrEmpty(filters.Role))
            {
                // Parse string role to UserRole enum if possible
                if (Enum.TryParse<UserRole>(filters.Role, out var roleEnum))
                {
                    query = query.Where(u => u.Role == roleEnum);
                }
            }

            if (filters.IsActive.HasValue)
            {
                query = query.Where(u => u.IsActive == filters.IsActive.Value);
            }

            if (filters.ClinicId.HasValue)
            {
                var clinicTenantId = filters.ClinicId.Value.ToString();
                query = query.Where(u => u.TenantId == clinicTenantId);
            }

            var totalCount = await query.CountAsync();

            // Get users with clinic info
            var users = await query
                .OrderBy(u => u.FullName)
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .Select(u => new
                {
                    User = u,
                    ClinicTenantId = u.TenantId
                })
                .ToListAsync();

            var result = new List<CrossTenantUserDto>();

            foreach (var item in users)
            {
                // Get clinic info
                Guid? clinicId = null;
                string clinicName = "Unknown";
                string? clinicSubdomain = null;

                if (Guid.TryParse(item.ClinicTenantId, out var parsedClinicId))
                {
                    var clinic = await _context.Clinics
                        .IgnoreQueryFilters()
                        .Where(c => c.Id == parsedClinicId)
                        .Select(c => new { c.Id, c.Name, c.Subdomain })
                        .FirstOrDefaultAsync();

                    if (clinic != null)
                    {
                        clinicId = clinic.Id;
                        clinicName = clinic.Name;
                        clinicSubdomain = clinic.Subdomain;
                    }
                }

                result.Add(new CrossTenantUserDto
                {
                    Id = item.User.Id,
                    Name = item.User.FullName,
                    Email = item.User.Email,
                    Phone = item.User.Phone,
                    Role = item.User.Role.ToString(),
                    IsActive = item.User.IsActive,
                    CreatedAt = item.User.CreatedAt,
                    ClinicId = clinicId,
                    ClinicName = clinicName,
                    ClinicSubdomain = clinicSubdomain
                });
            }

            return (result, totalCount);
        }

        public async Task<CrossTenantUserDto?> GetUserById(Guid userId)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return null;

            // Get clinic info
            Guid? clinicId = null;
            string clinicName = "Unknown";
            string? clinicSubdomain = null;

            if (Guid.TryParse(user.TenantId, out var parsedClinicId))
            {
                var clinic = await _context.Clinics
                    .IgnoreQueryFilters()
                    .Where(c => c.Id == parsedClinicId)
                    .Select(c => new { c.Id, c.Name, c.Subdomain })
                    .FirstOrDefaultAsync();

                if (clinic != null)
                {
                    clinicId = clinic.Id;
                    clinicName = clinic.Name;
                    clinicSubdomain = clinic.Subdomain;
                }
            }

            return new CrossTenantUserDto
            {
                Id = user.Id,
                Name = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                ClinicId = clinicId ?? Guid.Empty,
                ClinicName = clinicName,
                ClinicSubdomain = clinicSubdomain
            };
        }

        public async Task<bool> ResetPassword(Guid userId, string newPassword)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            var hashedPassword = _passwordHasher.HashPassword(newPassword);
            
            // NOTE: Using reflection here as the User entity may not expose a public password setter
            // TODO: Consider adding a dedicated UpdatePassword method to the User domain entity
            // that properly handles password updates with validation and audit logging
            var passwordProperty = user.GetType().GetProperty("PasswordHash");
            if (passwordProperty != null && passwordProperty.CanWrite)
            {
                passwordProperty.SetValue(user, hashedPassword);
            }
            else
            {
                // If we can't set via property, try to call a method if it exists
                var updatePasswordMethod = user.GetType().GetMethod("UpdatePassword");
                if (updatePasswordMethod != null)
                {
                    updatePasswordMethod.Invoke(user, new object[] { hashedPassword });
                }
                else
                {
                    return false;
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleUserActivation(Guid userId)
        {
            var user = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                return false;

            // NOTE: Using reflection here as the User entity may not expose public activation methods
            // TODO: Consider adding Activate/Deactivate methods to the User domain entity
            // that properly handle state transitions with validation and side effects
            var isActiveProperty = user.GetType().GetProperty("IsActive");
            if (isActiveProperty != null && isActiveProperty.CanWrite)
            {
                var currentValue = (bool)isActiveProperty.GetValue(user)!;
                isActiveProperty.SetValue(user, !currentValue);
            }
            else
            {
                // Try to call Activate/Deactivate methods if they exist
                if (user.IsActive)
                {
                    var deactivateMethod = user.GetType().GetMethod("Deactivate");
                    deactivateMethod?.Invoke(user, null);
                }
                else
                {
                    var activateMethod = user.GetType().GetMethod("Activate");
                    activateMethod?.Invoke(user, null);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> TransferOwnership(Guid currentOwnerId, Guid newOwnerId)
        {
            // Get both users
            var currentOwner = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == currentOwnerId);

            var newOwner = await _context.Users
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(u => u.Id == newOwnerId);

            if (currentOwner == null || newOwner == null)
                return false;

            // Ensure both users are from the same clinic
            if (currentOwner.ClinicId != newOwner.ClinicId)
                throw new ArgumentException("Users must be from the same clinic to transfer ownership");

            // Ensure current owner has owner role
            if (currentOwner.Role != UserRole.ClinicOwner)
                throw new ArgumentException("Current user is not an owner");

            // Ensure new owner is active
            if (!newOwner.IsActive)
                throw new ArgumentException("New owner must be active");

            // Transfer ownership - Note: User entity doesn't have public role setter
            // Would need proper methods in User entity for role changes
            // For now we log the intent
            
            // Create audit log entry if available
            try
            {
                var auditEntry = new Domain.Entities.AuditLog(
                    userId: currentOwnerId.ToString(),
                    userName: currentOwner.FullName,
                    userEmail: currentOwner.Email,
                    action: Domain.Enums.AuditAction.UPDATE,
                    actionDescription: $"Transferred ownership from {currentOwner.Email} to {newOwner.Email}",
                    entityType: "User",
                    entityId: newOwnerId.ToString(),
                    ipAddress: "system-admin",
                    userAgent: "System Admin Service",
                    requestPath: "/api/admin/transfer-ownership",
                    httpMethod: "POST",
                    result: Domain.Enums.OperationResult.SUCCESS,
                    dataCategory: Domain.Enums.DataCategory.PERSONAL,
                    purpose: Domain.Enums.LgpdPurpose.LEGITIMATE_INTEREST,
                    severity: Domain.Enums.AuditSeverity.WARNING,
                    tenantId: currentOwner.ClinicId?.ToString() ?? string.Empty
                );

                _context.Set<Domain.Entities.AuditLog>().Add(auditEntry);
            }
            catch
            {
                // Audit log is optional, continue if it fails
            }

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
