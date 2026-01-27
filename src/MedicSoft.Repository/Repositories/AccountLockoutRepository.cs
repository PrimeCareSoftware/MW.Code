using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AccountLockoutRepository : BaseRepository<AccountLockout>, IAccountLockoutRepository
    {
        public AccountLockoutRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<AccountLockout?> GetActiveLockedAccountAsync(string userId, string tenantId)
        {
            return await _context.Set<AccountLockout>()
                .Where(l => l.UserId == userId 
                    && l.TenantId == tenantId
                    && l.IsActive 
                    && l.UnlocksAt > DateTime.UtcNow)
                .OrderByDescending(l => l.LockedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetLockoutCountAsync(string userId, string tenantId)
        {
            return await _context.Set<AccountLockout>()
                .CountAsync(l => l.UserId == userId && l.TenantId == tenantId);
        }
    }
}
