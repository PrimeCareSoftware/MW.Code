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
    public class EncryptionKeyRepository : BaseRepository<EncryptionKey>, IEncryptionKeyRepository
    {
        public EncryptionKeyRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<EncryptionKey?> GetActiveKeyAsync(string tenantId)
        {
            return await _dbSet
                .Where(k => k.TenantId == tenantId && k.IsActive && (!k.ExpiresAt.HasValue || k.ExpiresAt > DateTime.UtcNow))
                .OrderByDescending(k => k.KeyVersion)
                .FirstOrDefaultAsync();
        }

        public async Task<EncryptionKey?> GetKeyByVersionAsync(string keyId, int version, string tenantId)
        {
            return await _dbSet
                .Where(k => k.KeyId == keyId && k.KeyVersion == version && k.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EncryptionKey>> GetAllKeysAsync(string tenantId)
        {
            return await _dbSet
                .Where(k => k.TenantId == tenantId)
                .OrderByDescending(k => k.KeyVersion)
                .ToListAsync();
        }
    }
}
