namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface for encrypting and decrypting sensitive medical data.
    /// Implementations must use strong encryption algorithms (e.g., AES-256).
    /// </summary>
    public interface IDataEncryptionService
    {
        /// <summary>
        /// Encrypts sensitive data.
        /// </summary>
        /// <param name="plainText">Data to encrypt</param>
        /// <returns>Encrypted data, or null if input is null/empty</returns>
        string? Encrypt(string? plainText);

        /// <summary>
        /// Decrypts previously encrypted data.
        /// </summary>
        /// <param name="cipherText">Encrypted data</param>
        /// <returns>Decrypted plaintext, or null if input is null/empty</returns>
        string? Decrypt(string? cipherText);

        /// <summary>
        /// Encrypts binary data (for certificates and keys).
        /// </summary>
        /// <param name="plainBytes">Data to encrypt</param>
        /// <returns>Encrypted data</returns>
        byte[] EncryptBytes(byte[] plainBytes);

        /// <summary>
        /// Decrypts binary data (for certificates and keys).
        /// </summary>
        /// <param name="cipherBytes">Encrypted data</param>
        /// <returns>Decrypted data</returns>
        byte[] DecryptBytes(byte[] cipherBytes);

        /// <summary>
        /// Generates a SHA-256 hash for searchable encrypted fields.
        /// </summary>
        /// <param name="plainText">Data to hash</param>
        /// <returns>Base64-encoded hash, or empty string if input is null/empty</returns>
        string GenerateSearchableHash(string? plainText);

        /// <summary>
        /// Encrypts a batch of strings efficiently.
        /// </summary>
        /// <param name="plainTexts">Data to encrypt</param>
        /// <returns>Encrypted data in the same order</returns>
        IEnumerable<string?> EncryptBatch(IEnumerable<string?> plainTexts);

        /// <summary>
        /// Decrypts a batch of strings efficiently.
        /// </summary>
        /// <param name="cipherTexts">Encrypted data</param>
        /// <returns>Decrypted data in the same order</returns>
        IEnumerable<string?> DecryptBatch(IEnumerable<string?> cipherTexts);
    }
}
