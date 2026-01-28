using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Repository.Context;
using MedicSoft.CrossCutting.Security;

namespace MedicSoft.Application.Services.SystemAdmin
{
    public interface ICrossTenantUserService
    {
        Task<(List<CrossTenantUserDto> Users, int TotalCount)> GetUsers(CrossTenantUserFilterDto filters);
        Task<CrossTenantUserDto?> GetUserById(Guid userId);
        Task<bool> ResetPassword(Guid userId, string newPassword);
        Task<bool> ToggleUserActivation(Guid userId);
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
                    u.Name.ToLower().Contains(searchLower) ||
                    u.Email.ToLower().Contains(searchLower) ||
                    (u.Phone != null && u.Phone.Contains(filters.SearchTerm)));
            }

            if (!string.IsNullOrEmpty(filters.Role))
            {
                query = query.Where(u => u.Role == filters.Role);
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
                .OrderBy(u => u.Name)
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
                    Name = item.User.Name,
                    Email = item.User.Email,
                    Phone = item.User.Phone,
                    Role = item.User.Role,
                    IsActive = item.User.IsActive,
                    CreatedAt = item.User.CreatedAt,
                    ClinicId = clinicId ?? Guid.Empty,
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
                Name = user.Name,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
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
            
            // Use reflection to update password since User entity might not have a public setter
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

            // Use reflection to toggle IsActive
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
    }
}
