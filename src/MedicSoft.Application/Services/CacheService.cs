using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Interfaces;

namespace MedicSoft.Application.Services
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<CacheService> _logger;
        
        // Optimized serialization options for entities with parameterized constructors
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            // Allows deserialization of types with parameterized constructors
            PreferredObjectCreationHandling = JsonObjectCreationHandling.Replace
        };

        public CacheService(IDistributedCache cache, ILogger<CacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var data = await _cache.GetStringAsync(key);
                if (data == null)
                    return default;

                // Use JsonOptions configured for entities with parameterized constructors
                return JsonSerializer.Deserialize<T>(data, JsonOptions);
            }
            catch (JsonException ex)
            {
                // Handle corrupted or incompatible cached data
                // This can happen when entity schemas change between deployments
                _logger.LogWarning(ex, 
                    "Failed to deserialize cache key {Key}. Removing corrupted cache entry. " +
                    "This may occur after schema changes or entity modifications.", key);
                
                // Remove the corrupted cache entry to prevent repeated errors
                await RemoveAsync(key);
                
                return default;
            }
            catch (InvalidOperationException ex)
            {
                // Handle invalid deserialization scenarios (e.g., parameterized constructors)
                _logger.LogWarning(ex,
                    "Failed to deserialize cache key {Key} due to invalid constructor binding. " +
                    "Removing corrupted cache entry.", key);

                await RemoveAsync(key);
                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error getting cache key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(30)
                };

                // Use JsonOptions configured for entities with parameterized constructors
                var json = JsonSerializer.Serialize(value, JsonOptions);
                await _cache.SetStringAsync(key, json, options);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cache key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Cache key removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cache key: {Key}", key);
            }
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> factory, TimeSpan? expiration = null)
        {
            var cached = await GetAsync<T>(key);
            // For reference types, check if we got a value from cache
            // For value types, default will be returned if not in cache
            if (cached != null || typeof(T).IsValueType)
            {
                if (cached != null)
                    return cached;
            }

            var value = await factory();
            await SetAsync(key, value, expiration);
            return value;
        }
    }
}
