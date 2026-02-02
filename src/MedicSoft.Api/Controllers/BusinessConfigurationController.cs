using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BusinessConfigurationController : BaseController
    {
        private readonly BusinessConfigurationService _service;

        public BusinessConfigurationController(
            ITenantContext tenantContext,
            BusinessConfigurationService service) : base(tenantContext)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the business configuration for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<BusinessConfigurationDto>> GetByClinicId(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var config = await _service.GetByClinicIdAsync(clinicId, tenantId);
            
            if (config == null)
                return NotFound(new { message = "Business configuration not found for this clinic" });
            
            return Ok(MapToDto(config));
        }

        /// <summary>
        /// Creates a new business configuration
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BusinessConfigurationDto>> Create([FromBody] CreateBusinessConfigurationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            try
            {
                var tenantId = GetTenantId();
                var config = await _service.CreateAsync(
                    dto.ClinicId,
                    dto.BusinessType,
                    dto.PrimarySpecialty,
                    tenantId);
                
                return CreatedAtAction(
                    nameof(GetByClinicId),
                    new { clinicId = config.ClinicId },
                    MapToDto(config));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the business type
        /// </summary>
        [HttpPut("{id}/business-type")]
        public async Task<ActionResult> UpdateBusinessType(Guid id, [FromBody] UpdateBusinessTypeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            try
            {
                var tenantId = GetTenantId();
                await _service.UpdateBusinessTypeAsync(id, dto.BusinessType, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the primary specialty
        /// </summary>
        [HttpPut("{id}/primary-specialty")]
        public async Task<ActionResult> UpdatePrimarySpecialty(Guid id, [FromBody] UpdatePrimarySpecialtyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            try
            {
                var tenantId = GetTenantId();
                await _service.UpdatePrimarySpecialtyAsync(id, dto.PrimarySpecialty, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a feature flag
        /// </summary>
        [HttpPut("{id}/feature")]
        public async Task<ActionResult> UpdateFeature(Guid id, [FromBody] UpdateFeatureDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();
            
            if (string.IsNullOrWhiteSpace(dto.FeatureName))
                return BadRequest(new { message = "Feature name is required" });
            
            try
            {
                var tenantId = GetTenantId();
                await _service.UpdateFeatureAsync(id, dto.FeatureName, dto.Enabled, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Checks if a feature is enabled for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}/feature/{featureName}")]
        public async Task<ActionResult<bool>> IsFeatureEnabled(Guid clinicId, string featureName)
        {
            var tenantId = GetTenantId();
            var isEnabled = await _service.IsFeatureEnabledAsync(clinicId, featureName, tenantId);
            return Ok(new { featureName, enabled = isEnabled });
        }

        /// <summary>
        /// Gets the terminology map for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}/terminology")]
        public async Task<ActionResult<Dictionary<string, string>>> GetTerminology(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var terminology = await _service.GetTerminologyAsync(clinicId, tenantId);
            return Ok(terminology);
        }

        private static BusinessConfigurationDto MapToDto(BusinessConfiguration config)
        {
            return new BusinessConfigurationDto
            {
                Id = config.Id,
                ClinicId = config.ClinicId,
                BusinessType = config.BusinessType,
                PrimarySpecialty = config.PrimarySpecialty,
                ElectronicPrescription = config.ElectronicPrescription,
                LabIntegration = config.LabIntegration,
                VaccineControl = config.VaccineControl,
                InventoryManagement = config.InventoryManagement,
                MultiRoom = config.MultiRoom,
                ReceptionQueue = config.ReceptionQueue,
                FinancialModule = config.FinancialModule,
                HealthInsurance = config.HealthInsurance,
                Telemedicine = config.Telemedicine,
                HomeVisit = config.HomeVisit,
                GroupSessions = config.GroupSessions,
                PublicProfile = config.PublicProfile,
                OnlineBooking = config.OnlineBooking,
                PatientReviews = config.PatientReviews,
                BiReports = config.BiReports,
                ApiAccess = config.ApiAccess,
                WhiteLabel = config.WhiteLabel,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt
            };
        }
    }
}
