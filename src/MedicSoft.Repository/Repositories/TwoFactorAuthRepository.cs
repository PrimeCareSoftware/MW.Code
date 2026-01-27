using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class TwoFactorAuthRepository : BaseRepository<TwoFactorAuth>, ITwoFactorAuthRepository
    {
        public TwoFactorAuthRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<TwoFactorAuth?> GetByUserIdAsync(string userId, string tenantId)
        {
            return await _context.Set<TwoFactorAuth>()
                .Where(t => t.UserId == userId && t.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsEnabledForUserAsync(string userId, string tenantId)
        {
            return await _context.Set<TwoFactorAuth>()
                .AnyAsync(t => t.UserId == userId 
                    && t.TenantId == tenantId 
                    && t.IsEnabled);
        }
    }
}
