using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.CrossCutting.Security
{
    /// <summary>
    /// Service for encrypting and decrypting sensitive medical data.
    /// Uses AES-256-GCM (Galois/Counter Mode) for authenticated encryption.
    /// Compliant with LGPD (Lei Geral de Proteção de Dados) requirements for healthcare data protection.
    /// </summary>
    public class DataEncryptionService : IDataEncryptionService
    {
        private readonly byte[] _encryptionKey;
        private const int NonceSize = 12; // 96 bits for GCM
        private const int TagSize = 16; // 128 bits authentication tag

        /// <summary>
        /// Initializes a new instance of the DataEncryptionService.
        /// </summary>
        /// <param name="encryptionKey">Base64-encoded 256-bit encryption key</param>
        public DataEncryptionService(string encryptionKey)
        {
            if (string.IsNullOrWhiteSpace(encryptionKey))
                throw new ArgumentException("Encryption key cannot be null or empty", nameof(encryptionKey));

            try
            {
                _encryptionKey = Convert.FromBase64String(encryptionKey);
            }
            catch (FormatException)
            {
                throw new ArgumentException("Encryption key must be a valid Base64 string", nameof(encryptionKey));
            }

            if (_encryptionKey.Length != 32) // 256 bits
                throw new ArgumentException("Encryption key must be 256 bits (32 bytes)", nameof(encryptionKey));
        }

        /// <summary>
        /// Encrypts sensitive data using AES-256-GCM.
        /// Returns null if input is null or whitespace.
        /// </summary>
        /// <param name="plainText">Data to encrypt</param>
        /// <returns>Base64-encoded encrypted data with nonce and tag, or null if input is empty</returns>
        public string? Encrypt(string? plainText)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                return plainText;

            try
            {
                using var aesGcm = new AesGcm(_encryptionKey, TagSize);
                
                // Generate a random nonce
                var nonce = new byte[NonceSize];
                RandomNumberGenerator.Fill(nonce);

                // Convert plaintext to bytes
                var plainBytes = Encoding.UTF8.GetBytes(plainText);
                
                // Prepare buffers
                var cipherBytes = new byte[plainBytes.Length];
                var tag = new byte[TagSize];

                // Encrypt
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

                // Combine nonce + tag + ciphertext for storage
                var result = new byte[NonceSize + TagSize + cipherBytes.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
                Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
                Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

                return Convert.ToBase64String(result);
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to encrypt data", ex);
            }
        }

        /// <summary>
        /// Decrypts data that was encrypted using AES-256-GCM.
        /// Returns null if input is null or whitespace.
        /// </summary>
        /// <param name="cipherText">Base64-encoded encrypted data</param>
        /// <returns>Decrypted plaintext, or null if input is empty</returns>
        public string? Decrypt(string? cipherText)
        {
            if (string.IsNullOrWhiteSpace(cipherText))
                return cipherText;

            // If the data doesn't look like Base64, assume it's unencrypted legacy data
            if (!IsBase64String(cipherText))
            {
                // Return the original value for unencrypted legacy data
                return cipherText;
            }

            try
            {
                var encryptedData = Convert.FromBase64String(cipherText);

                if (encryptedData.Length < NonceSize + TagSize)
                {
                    // Data is too short to be properly encrypted, likely unencrypted
                    return cipherText;
                }

                using var aesGcm = new AesGcm(_encryptionKey, TagSize);

                // Extract nonce, tag, and ciphertext
                var nonce = new byte[NonceSize];
                var tag = new byte[TagSize];
                var cipherBytes = new byte[encryptedData.Length - NonceSize - TagSize];

                Buffer.BlockCopy(encryptedData, 0, nonce, 0, NonceSize);
                Buffer.BlockCopy(encryptedData, NonceSize, tag, 0, TagSize);
                Buffer.BlockCopy(encryptedData, NonceSize + TagSize, cipherBytes, 0, cipherBytes.Length);

                // Decrypt
                var plainBytes = new byte[cipherBytes.Length];
                aesGcm.Decrypt(nonce, cipherBytes, tag, plainBytes);

                return Encoding.UTF8.GetString(plainBytes);
            }
            catch (FormatException)
            {
                // Not valid Base64, return original value
                return cipherText;
            }
            catch (CryptographicException)
            {
                // Decryption failed (wrong key or corrupted data), return original
                return cipherText;
            }
            catch (Exception)
            {
                // Any other error, return original value
                return cipherText;
            }
        }

        /// <summary>
        /// Encrypts binary data using AES-256-GCM (for certificates and keys).
        /// </summary>
        /// <param name="plainBytes">Data to encrypt</param>
        /// <returns>Encrypted data with nonce and tag</returns>
        public byte[] EncryptBytes(byte[] plainBytes)
        {
            if (plainBytes == null || plainBytes.Length == 0)
                throw new ArgumentException("Data to encrypt cannot be null or empty", nameof(plainBytes));

            try
            {
                using var aesGcm = new AesGcm(_encryptionKey, TagSize);
                
                // Generate a random nonce
                var nonce = new byte[NonceSize];
                RandomNumberGenerator.Fill(nonce);

                // Prepare buffers
                var cipherBytes = new byte[plainBytes.Length];
                var tag = new byte[TagSize];

                // Encrypt
                aesGcm.Encrypt(nonce, plainBytes, cipherBytes, tag);

                // Combine nonce + tag + ciphertext for storage
                var result = new byte[NonceSize + TagSize + cipherBytes.Length];
                Buffer.BlockCopy(nonce, 0, result, 0, NonceSize);
                Buffer.BlockCopy(tag, 0, result, NonceSize, TagSize);
                Buffer.BlockCopy(cipherBytes, 0, result, NonceSize + TagSize, cipherBytes.Length);

                return result;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to encrypt binary data", ex);
            }
        }

        /// <summary>
        /// Decrypts binary data that was encrypted using AES-256-GCM (for certificates and keys).
        /// </summary>
        /// <param name="cipherBytes">Encrypted data with nonce and tag</param>
        /// <returns>Decrypted data</returns>
        public byte[] DecryptBytes(byte[] cipherBytes)
        {
            if (cipherBytes == null || cipherBytes.Length == 0)
                throw new ArgumentException("Data to decrypt cannot be null or empty", nameof(cipherBytes));

            if (cipherBytes.Length < NonceSize + TagSize)
                throw new ArgumentException("Encrypted data is too short to be valid", nameof(cipherBytes));

            try
            {
                using var aesGcm = new AesGcm(_encryptionKey, TagSize);

                // Extract nonce, tag, and ciphertext
                var nonce = new byte[NonceSize];
                var tag = new byte[TagSize];
                var encrypted = new byte[cipherBytes.Length - NonceSize - TagSize];

                Buffer.BlockCopy(cipherBytes, 0, nonce, 0, NonceSize);
                Buffer.BlockCopy(cipherBytes, NonceSize, tag, 0, TagSize);
                Buffer.BlockCopy(cipherBytes, NonceSize + TagSize, encrypted, 0, encrypted.Length);

                // Decrypt
                var plainBytes = new byte[encrypted.Length];
                aesGcm.Decrypt(nonce, encrypted, tag, plainBytes);

                return plainBytes;
            }
            catch (Exception ex)
            {
                throw new CryptographicException("Failed to decrypt binary data", ex);
            }
        }

        /// <summary>
        /// Checks if a string is valid Base64.
        /// </summary>
        private bool IsBase64String(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            value = value.Trim();
            
            // Base64 strings should only contain valid Base64 characters
            // and have length that's a multiple of 4
            return (value.Length % 4 == 0) && 
                   System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,2}$");
        }

        /// <summary>
        /// Generates a new random 256-bit encryption key.
        /// </summary>
        /// <returns>Base64-encoded 256-bit key</returns>
        public static string GenerateKey()
        {
            var key = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }
    }
}
