using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface for managing encryption keys with versioning and rotation support.
    /// </summary>
    public interface IKeyManagementService
    {
        /// <summary>
        /// Gets the current active encryption key.
        /// </summary>
        Task<string> GetCurrentEncryptionKeyAsync();

        /// <summary>
        /// Gets a specific encryption key by its ID and version.
        /// </summary>
        Task<string?> GetKeyByIdAsync(string keyId, int keyVersion);

        /// <summary>
        /// Rotates the encryption key (creates new version and deprecates current).
        /// </summary>
        Task<EncryptionKey> RotateKeyAsync(Guid rotatedByUserId, string reason);

        /// <summary>
        /// Lists all encryption keys.
        /// </summary>
        Task<IEnumerable<EncryptionKey>> ListKeysAsync();

        /// <summary>
        /// Gets the active encryption key entity.
        /// </summary>
        Task<EncryptionKey?> GetActiveKeyEntityAsync();

        /// <summary>
        /// Initializes the key management system with a master key if none exists.
        /// </summary>
        Task InitializeAsync();
    }
}
