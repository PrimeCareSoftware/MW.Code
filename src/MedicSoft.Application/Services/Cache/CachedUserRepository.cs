using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.Cache;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Repositories;

namespace MedicSoft.Application.Services.Cache
{
    /// <summary>
    /// Cached repository decorator for User entity - Category 4.2
    /// Adds caching layer to reduce database queries for frequently accessed user data
    /// </summary>
    public class CachedUserRepository
    {
        private readonly IUserRepository _userRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedUserRepository> _logger;
        private const string USER_CACHE_KEY_PREFIX = "PrimeCare:user:";
        private const string USER_PERMISSIONS_KEY_PREFIX = "PrimeCare:user:permissions:";
        private readonly TimeSpan _userCacheExpiration = TimeSpan.FromMinutes(30);
        private readonly TimeSpan _permissionsCacheExpiration = TimeSpan.FromMinutes(15);

        public CachedUserRepository(
            IUserRepository userRepository,
            ICacheService cacheService,
            ILogger<CachedUserRepository> logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<User> GetByIdAsync(string userId)
        {
            var cacheKey = $"{USER_CACHE_KEY_PREFIX}{userId}";

            _logger.LogDebug("Attempting to retrieve user {UserId} from cache", userId);

            var cachedUser = await _cacheService.GetAsync<User>(cacheKey);
            if (cachedUser != null)
            {
                _logger.LogDebug("User {UserId} found in cache", userId);
                return cachedUser;
            }

            _logger.LogDebug("User {UserId} not found in cache, retrieving from database", userId);
            var user = await _userRepository.GetByIdAsync(userId);

            if (user != null)
            {
                await _cacheService.SetAsync(cacheKey, user, _userCacheExpiration);
                _logger.LogDebug("User {UserId} cached for {Expiration} minutes", userId, _userCacheExpiration.TotalMinutes);
            }

            return user;
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            var cacheKey = $"{USER_CACHE_KEY_PREFIX}username:{username.ToLowerInvariant()}";

            var cachedUser = await _cacheService.GetAsync<User>(cacheKey);
            if (cachedUser != null)
            {
                return cachedUser;
            }

            var user = await _userRepository.GetByUsernameAsync(username);

            if (user != null)
            {
                await _cacheService.SetAsync(cacheKey, user, _userCacheExpiration);
                // Also cache by ID
                await _cacheService.SetAsync($"{USER_CACHE_KEY_PREFIX}{user.Id}", user, _userCacheExpiration);
            }

            return user;
        }

        public async Task<List<string>> GetUserPermissionsAsync(string userId)
        {
            var cacheKey = $"{USER_PERMISSIONS_KEY_PREFIX}{userId}";

            _logger.LogDebug("Attempting to retrieve permissions for user {UserId} from cache", userId);

            var cachedPermissions = await _cacheService.GetAsync<List<string>>(cacheKey);
            if (cachedPermissions != null)
            {
                _logger.LogDebug("Permissions for user {UserId} found in cache", userId);
                return cachedPermissions;
            }

            _logger.LogDebug("Permissions for user {UserId} not found in cache, retrieving from database", userId);
            
            // Get user with permissions loaded
            var user = await _userRepository.GetByIdAsync(userId);
            if (user?.Profile?.ProfilePermissions == null)
            {
                return new List<string>();
            }

            var permissions = user.Profile.ProfilePermissions
                .Where(pp => pp.IsActive)
                .Select(pp => pp.Permission.Name)
                .ToList();

            await _cacheService.SetAsync(cacheKey, permissions, _permissionsCacheExpiration);
            _logger.LogDebug("Permissions for user {UserId} cached for {Expiration} minutes", 
                userId, _permissionsCacheExpiration.TotalMinutes);

            return permissions;
        }

        public async Task InvalidateUserCacheAsync(string userId)
        {
            _logger.LogInformation("Invalidating cache for user {UserId}", userId);

            await _cacheService.RemoveAsync($"{USER_CACHE_KEY_PREFIX}{userId}");
            await _cacheService.RemoveAsync($"{USER_PERMISSIONS_KEY_PREFIX}{userId}");

            // Try to get username to invalidate that cache too
            var user = await _userRepository.GetByIdAsync(userId);
            if (user != null)
            {
                await _cacheService.RemoveAsync($"{USER_CACHE_KEY_PREFIX}username:{user.Username.ToLowerInvariant()}");
            }

            _logger.LogInformation("Cache invalidated for user {UserId}", userId);
        }

        public async Task UpdateAsync(User user)
        {
            await _userRepository.UpdateAsync(user);
            await InvalidateUserCacheAsync(user.Id);
        }
    }
}
