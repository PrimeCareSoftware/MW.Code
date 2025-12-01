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
    public class OwnerRepository : IOwnerRepository
    {
        private readonly MedicSoftDbContext _context;

        public OwnerRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<Owner?> GetByUsernameAsync(string username, string tenantId)
        {
            return await _context.Owners
                .Include(o => o.Clinic)
                .FirstOrDefaultAsync(o => o.Username == username.ToLowerInvariant() && o.TenantId == tenantId);
        }

        public async Task<Owner?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.Owners
                .Include(o => o.Clinic)
                .FirstOrDefaultAsync(o => o.Id == id && o.TenantId == tenantId);
        }

        public async Task<Owner?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.Owners
                .Include(o => o.Clinic)
                .FirstOrDefaultAsync(o => o.ClinicId == clinicId && o.TenantId == tenantId);
        }

        public async Task<IEnumerable<Owner>> GetAllAsync(string tenantId)
        {
            return await _context.Owners
                .Include(o => o.Clinic)
                .Where(o => o.TenantId == tenantId)
                .OrderBy(o => o.FullName)
                .ToListAsync();
        }

        public async Task AddAsync(Owner owner)
        {
            await _context.Owners.AddAsync(owner);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds an owner to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task AddWithoutSaveAsync(Owner owner)
        {
            await _context.Owners.AddAsync(owner);
        }

        /// <summary>
        /// Marks an owner for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task DeleteWithoutSaveAsync(Guid id, string tenantId)
        {
            var owner = await GetByIdAsync(id, tenantId);
            if (owner != null)
            {
                _context.Owners.Remove(owner);
            }
        }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(Owner owner)
        {
            _context.Owners.Update(owner);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsByUsernameAsync(string username, string tenantId)
        {
            return await _context.Owners
                .AnyAsync(o => o.Username == username.ToLowerInvariant() && o.TenantId == tenantId);
        }

        public async Task<bool> ExistsByEmailAsync(string email, string tenantId)
        {
            return await _context.Owners
                .AnyAsync(o => o.Email == email.ToLowerInvariant() && o.TenantId == tenantId);
        }
    }
}
