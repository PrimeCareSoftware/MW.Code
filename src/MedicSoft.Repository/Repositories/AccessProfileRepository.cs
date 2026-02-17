using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AccessProfileRepository : IAccessProfileRepository
    {
        private readonly MedicSoftDbContext _context;

        public AccessProfileRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<AccessProfile?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.AccessProfiles
                .Include(ap => ap.Permissions)
                .Include(ap => ap.Clinic)
                .FirstOrDefaultAsync(ap => ap.Id == id && ap.TenantId == tenantId);
        }

        public async Task<IEnumerable<AccessProfile>> GetAllAsync(string tenantId)
        {
            return await _context.AccessProfiles
                .Include(ap => ap.Permissions)
                .Where(ap => ap.TenantId == tenantId && ap.IsActive)
                .OrderBy(ap => ap.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccessProfile>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            // Get profiles for this clinic (custom profiles) PLUS all default profiles from all clinics in the tenant
            // This allows owners to see and assign all default profile types (Medical, Dental, Nutritionist, etc.) to their users
            // regardless of their clinic's primary specialty, supporting multi-specialty and expanding clinics
            return await _context.AccessProfiles
                .Include(ap => ap.Permissions)
                .Where(ap => ap.TenantId == tenantId && ap.IsActive && 
                            (ap.ClinicId == clinicId || (ap.IsDefault && ap.ClinicId != null)))
                .OrderByDescending(ap => ap.IsDefault)
                .ThenBy(ap => ap.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccessProfile>> GetDefaultProfilesAsync(string tenantId)
        {
            return await _context.AccessProfiles
                .Include(ap => ap.Permissions)
                .Where(ap => ap.IsDefault && ap.TenantId == tenantId && ap.IsActive)
                .ToListAsync();
        }

        public IQueryable<AccessProfile> GetAllQueryable()
        {
            return _context.AccessProfiles.AsQueryable();
        }

        public async Task<AccessProfile?> GetByNameAsync(string name, Guid clinicId, string tenantId)
        {
            return await _context.AccessProfiles
                .Include(ap => ap.Permissions)
                .FirstOrDefaultAsync(ap => 
                    ap.Name == name && 
                    ap.ClinicId == clinicId && 
                    ap.TenantId == tenantId);
        }

        public async Task AddAsync(AccessProfile profile)
        {
            await _context.AccessProfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AccessProfile profile)
        {
            _context.AccessProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id, string tenantId)
        {
            var profile = await GetByIdAsync(id, tenantId);
            if (profile != null)
            {
                if (profile.IsDefault)
                {
                    throw new InvalidOperationException("Cannot delete default profiles");
                }

                _context.AccessProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> ExistsAsync(Guid id, string tenantId)
        {
            return await _context.AccessProfiles
                .AnyAsync(ap => ap.Id == id && ap.TenantId == tenantId);
        }

        public async Task<bool> IsProfileInUseAsync(Guid profileId, string tenantId)
        {
            return await _context.Users
                .AnyAsync(u => u.ProfileId == profileId && u.TenantId == tenantId);
        }
    }
}
