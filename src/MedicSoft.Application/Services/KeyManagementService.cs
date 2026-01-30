using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// File-based key management service for development and testing.
    /// Production systems should use Azure Key Vault or AWS KMS.
    /// </summary>
    public class KeyManagementService : IKeyManagementService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<KeyManagementService> _logger;
        private readonly string _keyStorePath;
        private const string DefaultKeyId = "medicsoft-data-encryption-key";
        private const string MasterKeyFileName = "master.key";
        private readonly string _tenantId;

        public KeyManagementService(
            IConfiguration configuration,
            ILogger<KeyManagementService> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _keyStorePath = _configuration["EncryptionSettings:KeyStorePath"] ?? "encryption-keys";
            _tenantId = _configuration["TenantId"] ?? "system";

            // Ensure key store directory exists
            if (!Directory.Exists(_keyStorePath))
            {
                Directory.CreateDirectory(_keyStorePath);
                _logger.LogInformation("Created key store directory: {KeyStorePath}", _keyStorePath);
            }
        }

        public async Task InitializeAsync()
        {
            var masterKeyPath = Path.Combine(_keyStorePath, MasterKeyFileName);
            
            if (!File.Exists(masterKeyPath))
            {
                _logger.LogWarning("No master encryption key found. Generating new key...");
                var newKey = GenerateKey();
                await File.WriteAllTextAsync(masterKeyPath, newKey);
                
                _logger.LogInformation("Generated new master encryption key and saved to {Path}", masterKeyPath);
            }
            else
            {
                _logger.LogDebug("Master encryption key found at {Path}", masterKeyPath);
            }
        }

        public async Task<string> GetCurrentEncryptionKeyAsync()
        {
            var masterKeyPath = Path.Combine(_keyStorePath, MasterKeyFileName);
            
            if (!File.Exists(masterKeyPath))
            {
                _logger.LogWarning("Master key not found, initializing...");
                await InitializeAsync();
            }

            var key = await File.ReadAllTextAsync(masterKeyPath);
            
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new InvalidOperationException("Master encryption key is empty");
            }

            return key.Trim();
        }

        public async Task<string?> GetKeyByIdAsync(string keyId, int keyVersion)
        {
            var keyFilePath = Path.Combine(_keyStorePath, $"{keyId}_v{keyVersion}.key");
            
            if (!File.Exists(keyFilePath))
            {
                _logger.LogWarning("Key file not found: {KeyFilePath}", keyFilePath);
                
                // Fallback to master key for version 1
                if (keyVersion == 1)
                {
                    return await GetCurrentEncryptionKeyAsync();
                }
                
                return null;
            }

            return await File.ReadAllTextAsync(keyFilePath);
        }

        public async Task<EncryptionKey> RotateKeyAsync(Guid rotatedByUserId, string reason)
        {
            _logger.LogInformation("Starting key rotation. Reason: {Reason}, User: {UserId}", reason, rotatedByUserId);

            // Generate new key
            var newKeyMaterial = GenerateKey();
            var currentVersion = await GetCurrentKeyVersionAsync();
            var newVersion = currentVersion + 1;

            // Save new key
            var newKeyPath = Path.Combine(_keyStorePath, $"{DefaultKeyId}_v{newVersion}.key");
            await File.WriteAllTextAsync(newKeyPath, newKeyMaterial);

            // Create metadata
            var metadata = new
            {
                KeyId = DefaultKeyId,
                Version = newVersion,
                CreatedAt = DateTime.UtcNow,
                RotatedBy = rotatedByUserId,
                Reason = reason,
                Algorithm = "AES-256-GCM"
            };

            var metadataPath = Path.Combine(_keyStorePath, $"{DefaultKeyId}_v{newVersion}.meta.json");
            await File.WriteAllTextAsync(metadataPath, JsonSerializer.Serialize(metadata, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            }));

            // Update master key to point to new version
            var masterKeyPath = Path.Combine(_keyStorePath, MasterKeyFileName);
            await File.WriteAllTextAsync(masterKeyPath, newKeyMaterial);

            _logger.LogInformation("Key rotation complete. New version: {Version}", newVersion);

            return new EncryptionKey(
                keyId: DefaultKeyId,
                keyVersion: newVersion,
                algorithm: "AES-256-GCM",
                purpose: "DATA_ENCRYPTION",
                tenantId: _tenantId,
                description: $"Rotated on {DateTime.UtcNow:yyyy-MM-dd}. Reason: {reason}"
            );
        }

        public async Task<IEnumerable<EncryptionKey>> ListKeysAsync()
        {
            var keys = new List<EncryptionKey>();
            var keyFiles = Directory.GetFiles(_keyStorePath, "*.meta.json");

            foreach (var metadataFile in keyFiles)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(metadataFile);
                    var metadata = JsonSerializer.Deserialize<KeyMetadata>(json);
                    
                    if (metadata != null)
                    {
                        var key = new EncryptionKey(
                            keyId: metadata.KeyId ?? DefaultKeyId,
                            keyVersion: metadata.Version,
                            algorithm: metadata.Algorithm ?? "AES-256-GCM",
                            purpose: "DATA_ENCRYPTION",
                            tenantId: _tenantId,
                            description: metadata.Reason
                        );
                        
                        keys.Add(key);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to load key metadata from {File}", metadataFile);
                }
            }

            return keys.OrderByDescending(k => k.KeyVersion);
        }

        public async Task<EncryptionKey?> GetActiveKeyEntityAsync()
        {
            var currentVersion = await GetCurrentKeyVersionAsync();
            
            return new EncryptionKey(
                keyId: DefaultKeyId,
                keyVersion: currentVersion,
                algorithm: "AES-256-GCM",
                purpose: "DATA_ENCRYPTION",
                tenantId: _tenantId,
                description: "Current active encryption key"
            );
        }

        private async Task<int> GetCurrentKeyVersionAsync()
        {
            var keyFiles = Directory.GetFiles(_keyStorePath, $"{DefaultKeyId}_v*.key");
            
            if (!keyFiles.Any())
                return 1;

            var versions = keyFiles
                .Select(f => Path.GetFileNameWithoutExtension(f))
                .Select(name => 
                {
                    var parts = name.Split('_');
                    if (parts.Length >= 2 && parts[^1].StartsWith("v") && int.TryParse(parts[^1][1..], out var version))
                        return version;
                    return 0;
                })
                .Where(v => v > 0)
                .ToList();

            return versions.Any() ? versions.Max() : 1;
        }

        private string GenerateKey()
        {
            var key = new byte[32]; // 256 bits
            RandomNumberGenerator.Fill(key);
            return Convert.ToBase64String(key);
        }

        private class KeyMetadata
        {
            public string? KeyId { get; set; }
            public int Version { get; set; }
            public DateTime CreatedAt { get; set; }
            public Guid RotatedBy { get; set; }
            public string? Reason { get; set; }
            public string? Algorithm { get; set; }
        }
    }
}
