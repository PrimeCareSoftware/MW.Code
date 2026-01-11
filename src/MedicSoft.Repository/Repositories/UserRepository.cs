using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly MedicSoftDbContext _context;

        public UserRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<object?> GetByUsernameAsync(string username)
        {
            // Legacy method for compatibility
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant());
        }

        public async Task<User?> GetUserByUsernameAsync(string username, string tenantId)
        {
            return await _context.Users
                .Include(u => u.Clinic)
                .FirstOrDefaultAsync(u => u.Username == username.ToLowerInvariant() && u.TenantId == tenantId);
        }

        public async Task<User?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.Users
                .Include(u => u.Clinic)
                .FirstOrDefaultAsync(u => u.Id == id && u.TenantId == tenantId);
        }

        public async Task<IEnumerable<User>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.Users
                .Where(u => u.ClinicId == clinicId && u.TenantId == tenantId && u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync(string tenantId)
        {
            return await _context.Users
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();
        }

        public async Task<int> GetUserCountByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.Users
                .CountAsync(u => u.ClinicId == clinicId && u.TenantId == tenantId && u.IsActive);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a user to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task AddWithoutSaveAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        /// <summary>
        /// Marks a user for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task DeleteWithoutSaveAsync(Guid id, string tenantId)
        {
            var user = await GetByIdAsync(id, tenantId);
            if (user != null)
            {
                _context.Users.Remove(user);
            }
        }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
