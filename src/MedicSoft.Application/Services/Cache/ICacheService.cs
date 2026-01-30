using System;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services.Cache
{
    /// <summary>
    /// Interface for distributed caching service - Category 4.2
    /// Provides unified caching abstraction over Redis or In-Memory cache
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Get cached value by key
        /// </summary>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Set cached value with absolute expiration
        /// </summary>
        Task SetAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

        /// <summary>
        /// Set cached value with sliding expiration
        /// </summary>
        Task SetSlidingAsync<T>(string key, T value, TimeSpan? expiration = null) where T : class;

        /// <summary>
        /// Remove cached value by key
        /// </summary>
        Task RemoveAsync(string key);

        /// <summary>
        /// Remove all cached values matching a pattern
        /// </summary>
        Task RemoveByPatternAsync(string pattern);

        /// <summary>
        /// Check if key exists in cache
        /// </summary>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// Refresh sliding expiration for a key
        /// </summary>
        Task RefreshAsync(string key);
    }
}
