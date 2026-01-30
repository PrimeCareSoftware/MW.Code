using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services.Cache
{
    /// <summary>
    /// Redis-based distributed cache service implementation - Category 4.2
    /// Provides caching for frequently accessed data to improve performance
    /// </summary>
    public class DistributedCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedCacheService> _logger;
        private readonly TimeSpan _defaultExpiration = TimeSpan.FromMinutes(60);

        public DistributedCacheService(
            IDistributedCache cache,
            ILogger<DistributedCacheService> logger)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            try
            {
                _logger.LogDebug("Getting cached value for key: {Key}", key);

                var cachedData = await _cache.GetStringAsync(key);

                if (string.IsNullOrEmpty(cachedData))
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return JsonSerializer.Deserialize<T>(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving cached value for key: {Key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                _logger.LogDebug("Setting cached value for key: {Key}", key);

                var serializedData = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiration ?? _defaultExpiration
                };

                await _cache.SetStringAsync(key, serializedData, options);

                _logger.LogDebug("Cached value set for key: {Key} with expiration: {Expiration}",
                    key, expiration ?? _defaultExpiration);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value for key: {Key}", key);
            }
        }

        public async Task SetSlidingAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class
        {
            try
            {
                _logger.LogDebug("Setting cached value with sliding expiration for key: {Key}", key);

                var serializedData = JsonSerializer.Serialize(value);
                var options = new DistributedCacheEntryOptions
                {
                    SlidingExpiration = expiration ?? TimeSpan.FromMinutes(30)
                };

                await _cache.SetStringAsync(key, serializedData, options);

                _logger.LogDebug("Cached value set with sliding expiration for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting cached value with sliding expiration for key: {Key}", key);
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                _logger.LogDebug("Removing cached value for key: {Key}", key);
                await _cache.RemoveAsync(key);
                _logger.LogDebug("Cached value removed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached value for key: {Key}", key);
            }
        }

        public async Task RemoveByPatternAsync(string pattern)
        {
            try
            {
                _logger.LogWarning("RemoveByPatternAsync not fully implemented for IDistributedCache. " +
                    "Consider using StackExchange.Redis directly for pattern-based operations. Pattern: {Pattern}", pattern);
                
                // Note: IDistributedCache doesn't support pattern-based deletion out of the box
                // For full Redis pattern support, implement using StackExchange.Redis directly
                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing cached values by pattern: {Pattern}", pattern);
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var cachedData = await _cache.GetStringAsync(key);
                return !string.IsNullOrEmpty(cachedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if key exists: {Key}", key);
                return false;
            }
        }

        public async Task RefreshAsync(string key)
        {
            try
            {
                _logger.LogDebug("Refreshing cached value for key: {Key}", key);
                await _cache.RefreshAsync(key);
                _logger.LogDebug("Cached value refreshed for key: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing cached value for key: {Key}", key);
            }
        }
    }
}
