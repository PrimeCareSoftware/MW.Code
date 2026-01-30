using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.Cache;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Repositories;

namespace MedicSoft.Application.Services.Cache
{
    /// <summary>
    /// Cached repository decorator for Clinic entity - Category 4.2
    /// Adds caching layer to reduce database queries for clinic configuration data
    /// </summary>
    public class CachedClinicRepository
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<CachedClinicRepository> _logger;
        private const string CLINIC_CACHE_KEY_PREFIX = "PrimeCare:clinic:";
        private const string CLINIC_CONFIG_KEY_PREFIX = "PrimeCare:clinic:config:";
        private readonly TimeSpan _clinicCacheExpiration = TimeSpan.FromMinutes(60);

        public CachedClinicRepository(
            IClinicRepository clinicRepository,
            ICacheService cacheService,
            ILogger<CachedClinicRepository> logger)
        {
            _clinicRepository = clinicRepository ?? throw new ArgumentNullException(nameof(clinicRepository));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Clinic> GetByIdAsync(Guid clinicId)
        {
            var cacheKey = $"{CLINIC_CACHE_KEY_PREFIX}{clinicId}";

            _logger.LogDebug("Attempting to retrieve clinic {ClinicId} from cache", clinicId);

            var cachedClinic = await _cacheService.GetAsync<Clinic>(cacheKey);
            if (cachedClinic != null)
            {
                _logger.LogDebug("Clinic {ClinicId} found in cache", clinicId);
                return cachedClinic;
            }

            _logger.LogDebug("Clinic {ClinicId} not found in cache, retrieving from database", clinicId);
            var clinic = await _clinicRepository.GetByIdAsync(clinicId);

            if (clinic != null)
            {
                await _cacheService.SetAsync(cacheKey, clinic, _clinicCacheExpiration);
                _logger.LogDebug("Clinic {ClinicId} cached for {Expiration} minutes", 
                    clinicId, _clinicCacheExpiration.TotalMinutes);
            }

            return clinic;
        }

        public async Task<List<Clinic>> GetAllActiveAsync()
        {
            var cacheKey = $"{CLINIC_CACHE_KEY_PREFIX}all:active";

            var cachedClinics = await _cacheService.GetAsync<List<Clinic>>(cacheKey);
            if (cachedClinics != null)
            {
                _logger.LogDebug("Active clinics found in cache");
                return cachedClinics;
            }

            _logger.LogDebug("Active clinics not found in cache, retrieving from database");
            var clinics = await _clinicRepository.GetAllActiveAsync();

            if (clinics != null && clinics.Count > 0)
            {
                await _cacheService.SetAsync(cacheKey, clinics, _clinicCacheExpiration);
            }

            return clinics;
        }

        public async Task<ClinicCustomization> GetClinicCustomizationAsync(Guid clinicId)
        {
            var cacheKey = $"{CLINIC_CONFIG_KEY_PREFIX}{clinicId}";

            var cachedConfig = await _cacheService.GetAsync<ClinicCustomization>(cacheKey);
            if (cachedConfig != null)
            {
                return cachedConfig;
            }

            var config = await _clinicRepository.GetCustomizationAsync(clinicId);

            if (config != null)
            {
                // Cache clinic configuration for longer (2 hours) as it changes less frequently
                await _cacheService.SetAsync(cacheKey, config, TimeSpan.FromMinutes(120));
            }

            return config;
        }

        public async Task InvalidateClinicCacheAsync(Guid clinicId)
        {
            _logger.LogInformation("Invalidating cache for clinic {ClinicId}", clinicId);

            await _cacheService.RemoveAsync($"{CLINIC_CACHE_KEY_PREFIX}{clinicId}");
            await _cacheService.RemoveAsync($"{CLINIC_CONFIG_KEY_PREFIX}{clinicId}");
            await _cacheService.RemoveAsync($"{CLINIC_CACHE_KEY_PREFIX}all:active");

            _logger.LogInformation("Cache invalidated for clinic {ClinicId}", clinicId);
        }

        public async Task UpdateAsync(Clinic clinic)
        {
            await _clinicRepository.UpdateAsync(clinic);
            await InvalidateClinicCacheAsync(clinic.Id);
        }
    }
}
