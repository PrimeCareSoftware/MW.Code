using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for EncryptionKey entity.
    /// </summary>
    public interface IEncryptionKeyRepository : IRepository<EncryptionKey>
    {
        Task<EncryptionKey?> GetActiveKeyAsync(string tenantId);
        Task<EncryptionKey?> GetKeyByVersionAsync(string keyId, int version, string tenantId);
        Task<IEnumerable<EncryptionKey>> GetAllKeysAsync(string tenantId);
    }
}
