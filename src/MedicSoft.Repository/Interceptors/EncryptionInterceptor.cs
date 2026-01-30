using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;
using MedicSoft.Domain.Attributes;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Repository.Interceptors
{
    /// <summary>
    /// EF Core interceptor that automatically encrypts/decrypts properties marked with [Encrypted] attribute.
    /// Implements transparent encryption for sensitive medical data at rest.
    /// Compliant with LGPD Art. 46 (Lei Geral de Proteção de Dados).
    /// </summary>
    public class EncryptionInterceptor : SaveChangesInterceptor
    {
        private readonly IDataEncryptionService _encryptionService;
        private readonly ILogger<EncryptionInterceptor> _logger;
        private static readonly ConcurrentDictionary<Type, List<EncryptedPropertyInfo>> _encryptedPropertiesCache = new();

        public EncryptionInterceptor(IDataEncryptionService encryptionService, ILogger<EncryptionInterceptor> logger)
        {
            _encryptionService = encryptionService ?? throw new ArgumentNullException(nameof(encryptionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
        {
            ProcessEntities(eventData.Context);
            return base.SavingChanges(eventData, result);
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, 
            InterceptionResult<int> result, 
            CancellationToken cancellationToken = default)
        {
            ProcessEntities(eventData.Context);
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        private void ProcessEntities(DbContext? context)
        {
            if (context == null) return;

            var entries = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .ToList();

            foreach (var entry in entries)
            {
                var entityType = entry.Entity.GetType();
                var encryptedProperties = GetEncryptedProperties(entityType);

                if (!encryptedProperties.Any())
                    continue;

                foreach (var propInfo in encryptedProperties)
                {
                    try
                    {
                        var currentValue = propInfo.Property.GetValue(entry.Entity) as string;
                        
                        if (string.IsNullOrWhiteSpace(currentValue))
                            continue;

                        // Avoid double encryption: check if value is already encrypted
                        if (IsAlreadyEncrypted(currentValue))
                        {
                            _logger.LogDebug("Property {PropertyName} on {EntityType} is already encrypted, skipping",
                                propInfo.Property.Name, entityType.Name);
                            continue;
                        }

                        // Encrypt the value
                        var encryptedValue = _encryptionService.Encrypt(currentValue);
                        propInfo.Property.SetValue(entry.Entity, encryptedValue);

                        // Generate hash for searchable fields
                        if (propInfo.Attribute.Searchable && propInfo.HashProperty != null)
                        {
                            var hash = GenerateHash(currentValue);
                            propInfo.HashProperty.SetValue(entry.Entity, hash);
                            
                            _logger.LogDebug("Generated searchable hash for {PropertyName} on {EntityType}",
                                propInfo.Property.Name, entityType.Name);
                        }

                        _logger.LogInformation("Encrypted property {PropertyName} on {EntityType} (Priority: {Priority})",
                            propInfo.Property.Name, entityType.Name, propInfo.Attribute.Priority);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to encrypt property {PropertyName} on {EntityType}",
                            propInfo.Property.Name, entityType.Name);
                        throw new InvalidOperationException(
                            $"Failed to encrypt property {propInfo.Property.Name} on {entityType.Name}", ex);
                    }
                }
            }
        }

        private List<EncryptedPropertyInfo> GetEncryptedProperties(Type entityType)
        {
            return _encryptedPropertiesCache.GetOrAdd(entityType, type =>
            {
                var properties = new List<EncryptedPropertyInfo>();

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    var encryptedAttr = property.GetCustomAttribute<EncryptedAttribute>();
                    if (encryptedAttr == null)
                        continue;

                    // Only support string properties
                    if (property.PropertyType != typeof(string))
                    {
                        _logger.LogWarning(
                            "Property {PropertyName} on {EntityType} has [Encrypted] attribute but is not a string. Skipping.",
                            property.Name, type.Name);
                        continue;
                    }

                    PropertyInfo? hashProperty = null;
                    if (encryptedAttr.Searchable)
                    {
                        var hashPropertyName = $"{property.Name}Hash";
                        hashProperty = type.GetProperty(hashPropertyName, BindingFlags.Public | BindingFlags.Instance);
                        
                        if (hashProperty == null)
                        {
                            _logger.LogWarning(
                                "Property {PropertyName} on {EntityType} is marked as Searchable but no {HashPropertyName} property found",
                                property.Name, type.Name, hashPropertyName);
                        }
                    }

                    properties.Add(new EncryptedPropertyInfo
                    {
                        Property = property,
                        Attribute = encryptedAttr,
                        HashProperty = hashProperty
                    });

                    _logger.LogDebug("Registered encrypted property: {EntityType}.{PropertyName} (Priority: {Priority}, Searchable: {Searchable})",
                        type.Name, property.Name, encryptedAttr.Priority, encryptedAttr.Searchable);
                }

                return properties;
            });
        }

        /// <summary>
        /// Checks if a value is already encrypted by attempting to detect Base64 format with proper length.
        /// This helps prevent double-encryption during migrations.
        /// </summary>
        private bool IsAlreadyEncrypted(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return false;

            // Basic heuristic: encrypted values are Base64 and longer than typical plaintext
            // AES-GCM encrypted data has nonce (12) + tag (16) + ciphertext, so minimum 28 bytes Base64
            if (value.Length < 40) // Minimum reasonable encrypted length
                return false;

            // Check if it looks like Base64
            value = value.Trim();
            if (value.Length % 4 != 0)
                return false;

            return System.Text.RegularExpressions.Regex.IsMatch(value, @"^[a-zA-Z0-9\+/]*={0,2}$");
        }

        /// <summary>
        /// Generates SHA-256 hash for searchable encrypted fields.
        /// </summary>
        private string GenerateHash(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return string.Empty;

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Convert.ToBase64String(hashBytes);
        }

        private class EncryptedPropertyInfo
        {
            public PropertyInfo Property { get; set; } = null!;
            public EncryptedAttribute Attribute { get; set; } = null!;
            public PropertyInfo? HashProperty { get; set; }
        }
    }
}
