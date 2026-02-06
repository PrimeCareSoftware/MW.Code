using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ExternalServicesController : BaseController
    {
        private readonly ExternalServiceConfigurationService _service;

        public ExternalServicesController(
            ITenantContext tenantContext,
            ExternalServiceConfigurationService service) : base(tenantContext)
        {
            _service = service;
        }

        /// <summary>
        /// Gets all external service configurations
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExternalServiceConfigurationDto>>> GetAll()
        {
            var tenantId = GetTenantId();
            var configs = await _service.GetAllAsync(tenantId);
            
            return Ok(configs.Select(MapToDto));
        }

        /// <summary>
        /// Gets external service configuration by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExternalServiceConfigurationDto>> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var config = await _service.GetByIdAsync(id, tenantId);
            
            if (config == null)
                return NotFound(new { message = "External service configuration not found" });
            
            return Ok(MapToDto(config));
        }

        /// <summary>
        /// Gets external service configuration by service type
        /// </summary>
        [HttpGet("by-type/{serviceType}")]
        public async Task<ActionResult<ExternalServiceConfigurationDto>> GetByServiceType(
            ExternalServiceType serviceType,
            [FromQuery] Guid? clinicId = null)
        {
            var tenantId = GetTenantId();
            var config = await _service.GetByServiceTypeAsync(serviceType, tenantId, clinicId);
            
            if (config == null)
                return NotFound(new { message = "External service configuration not found" });
            
            return Ok(MapToDto(config));
        }

        /// <summary>
        /// Gets external service configurations for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<IEnumerable<ExternalServiceConfigurationDto>>> GetByClinicId(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var configs = await _service.GetByClinicIdAsync(clinicId, tenantId);
            
            return Ok(configs.Select(MapToDto));
        }

        /// <summary>
        /// Gets all active external service configurations
        /// </summary>
        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ExternalServiceConfigurationDto>>> GetActive()
        {
            var tenantId = GetTenantId();
            var configs = await _service.GetActiveServicesAsync(tenantId);
            
            return Ok(configs.Select(MapToDto));
        }

        /// <summary>
        /// Creates a new external service configuration
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExternalServiceConfigurationDto>> Create([FromBody] CreateExternalServiceConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            try
            {
                var tenantId = GetTenantId();
                var config = await _service.CreateAsync(
                    dto.ServiceType,
                    dto.ServiceName,
                    tenantId,
                    dto.ClinicId,
                    dto.Description);
                
                // Update credentials if provided
                if (!string.IsNullOrWhiteSpace(dto.ApiKey) || 
                    !string.IsNullOrWhiteSpace(dto.ApiSecret) ||
                    !string.IsNullOrWhiteSpace(dto.ClientId) ||
                    !string.IsNullOrWhiteSpace(dto.ClientSecret))
                {
                    await _service.UpdateCredentialsAsync(
                        config.Id,
                        tenantId,
                        dto.ApiKey,
                        dto.ApiSecret,
                        dto.ClientId,
                        dto.ClientSecret);
                }
                
                // Update tokens if provided
                if (!string.IsNullOrWhiteSpace(dto.AccessToken))
                {
                    await _service.UpdateTokensAsync(
                        config.Id,
                        tenantId,
                        dto.AccessToken,
                        dto.RefreshToken,
                        dto.TokenExpiresAt);
                }
                
                // Update service configuration
                await _service.UpdateServiceConfigurationAsync(
                    config.Id,
                    tenantId,
                    dto.ApiUrl,
                    dto.WebhookUrl,
                    dto.AccountId,
                    dto.ProjectId,
                    dto.Region,
                    dto.AdditionalConfiguration);
                
                // Set active status
                await _service.UpdateAsync(config.Id, dto.ServiceName, dto.Description, dto.IsActive, tenantId);
                
                // Fetch the updated config
                config = await _service.GetByIdAsync(config.Id, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById),
                    new { id = config!.Id },
                    MapToDto(config));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates an external service configuration
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateExternalServiceConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            try
            {
                var tenantId = GetTenantId();
                
                // Update basic info
                await _service.UpdateAsync(id, dto.ServiceName, dto.Description, dto.IsActive, tenantId);
                
                // Update credentials if provided
                if (!string.IsNullOrWhiteSpace(dto.ApiKey) || 
                    !string.IsNullOrWhiteSpace(dto.ApiSecret) ||
                    !string.IsNullOrWhiteSpace(dto.ClientId) ||
                    !string.IsNullOrWhiteSpace(dto.ClientSecret))
                {
                    await _service.UpdateCredentialsAsync(
                        id,
                        tenantId,
                        dto.ApiKey,
                        dto.ApiSecret,
                        dto.ClientId,
                        dto.ClientSecret);
                }
                
                // Update tokens if provided
                if (!string.IsNullOrWhiteSpace(dto.AccessToken))
                {
                    await _service.UpdateTokensAsync(
                        id,
                        tenantId,
                        dto.AccessToken,
                        dto.RefreshToken,
                        dto.TokenExpiresAt);
                }
                
                // Update service configuration
                await _service.UpdateServiceConfigurationAsync(
                    id,
                    tenantId,
                    dto.ApiUrl,
                    dto.WebhookUrl,
                    dto.AccountId,
                    dto.ProjectId,
                    dto.Region,
                    dto.AdditionalConfiguration);
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deletes an external service configuration
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _service.DeleteAsync(id, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Records a successful sync for a service
        /// </summary>
        [HttpPost("{id}/sync")]
        public async Task<ActionResult> RecordSync(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _service.RecordSyncAsync(id, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Records an error for a service
        /// </summary>
        [HttpPost("{id}/error")]
        public async Task<ActionResult> RecordError(Guid id, [FromBody] string errorMessage)
        {
            try
            {
                var tenantId = GetTenantId();
                await _service.RecordErrorAsync(id, tenantId, errorMessage);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private static ExternalServiceConfigurationDto MapToDto(Domain.Entities.ExternalServiceConfiguration config)
        {
            return new ExternalServiceConfigurationDto
            {
                Id = config.Id,
                ServiceType = config.ServiceType,
                ServiceName = config.ServiceName,
                Description = config.Description,
                IsActive = config.IsActive,
                HasApiKey = !string.IsNullOrWhiteSpace(config.ApiKey),
                HasApiSecret = !string.IsNullOrWhiteSpace(config.ApiSecret),
                HasClientId = !string.IsNullOrWhiteSpace(config.ClientId),
                HasClientSecret = !string.IsNullOrWhiteSpace(config.ClientSecret),
                HasAccessToken = !string.IsNullOrWhiteSpace(config.AccessToken),
                HasRefreshToken = !string.IsNullOrWhiteSpace(config.RefreshToken),
                TokenExpiresAt = config.TokenExpiresAt,
                IsTokenExpired = config.IsTokenExpired(),
                ApiUrl = config.ApiUrl,
                WebhookUrl = config.WebhookUrl,
                AccountId = config.AccountId,
                ProjectId = config.ProjectId,
                Region = config.Region,
                AdditionalConfiguration = config.AdditionalConfiguration,
                LastSyncAt = config.LastSyncAt,
                LastError = config.LastError,
                ErrorCount = config.ErrorCount,
                HasValidConfiguration = config.HasValidConfiguration(),
                ClinicId = config.ClinicId,
                ClinicName = config.Clinic?.Name,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt
            };
        }
    }
}
