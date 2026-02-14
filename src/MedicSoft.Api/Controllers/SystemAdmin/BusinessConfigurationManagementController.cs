using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for system admins to manage business configurations for clinics
    /// </summary>
    [ApiController]
    [Route("api/system-admin/business-configuration")]
    [Authorize(Roles = "SystemAdmin")]
    public class BusinessConfigurationManagementController : BaseController
    {
        private readonly BusinessConfigurationService _service;

        public BusinessConfigurationManagementController(
            ITenantContext tenantContext,
            BusinessConfigurationService service) : base(tenantContext)
        {
            _service = service;
        }

        /// <summary>
        /// Gets the business configuration for any clinic (system admin)
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        public async Task<ActionResult<BusinessConfigurationDto>> GetByClinicId(Guid clinicId, [FromQuery] string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest(new { message = "TenantId is required" });

            var config = await _service.GetByClinicIdAsync(clinicId, tenantId);
            
            if (config == null)
                return NotFound(new { message = "Business configuration not found for this clinic" });
            
            return Ok(MapToDto(config));
        }

        /// <summary>
        /// Creates a new business configuration for any clinic (system admin)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<BusinessConfigurationDto>> Create([FromBody] CreateBusinessConfigurationForClinicDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (string.IsNullOrWhiteSpace(dto.TenantId))
                return BadRequest(new { message = "TenantId is required" });
            
            try
            {
                var config = await _service.CreateAsync(
                    dto.ClinicId,
                    dto.BusinessType,
                    dto.PrimarySpecialty,
                    dto.TenantId);
                
                return CreatedAtAction(
                    nameof(GetByClinicId),
                    new { clinicId = config.ClinicId, tenantId = dto.TenantId },
                    MapToDto(config));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the business type for any clinic (system admin)
        /// </summary>
        [HttpPut("{id}/business-type")]
        public async Task<ActionResult> UpdateBusinessType(Guid id, [FromBody] UpdateBusinessTypeForClinicDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (string.IsNullOrWhiteSpace(dto.TenantId))
                return BadRequest(new { message = "TenantId is required" });
            
            try
            {
                await _service.UpdateBusinessTypeAsync(id, dto.BusinessType, dto.TenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates the primary specialty for any clinic (system admin)
        /// </summary>
        [HttpPut("{id}/primary-specialty")]
        public async Task<ActionResult> UpdatePrimarySpecialty(Guid id, [FromBody] UpdatePrimarySpecialtyForClinicDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (string.IsNullOrWhiteSpace(dto.TenantId))
                return BadRequest(new { message = "TenantId is required" });
            
            try
            {
                await _service.UpdatePrimarySpecialtyAsync(id, dto.PrimarySpecialty, dto.TenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Updates a feature flag for any clinic (system admin)
        /// </summary>
        [HttpPut("{id}/feature")]
        public async Task<ActionResult> UpdateFeature(Guid id, [FromBody] UpdateFeatureForClinicDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (string.IsNullOrWhiteSpace(dto.TenantId))
                return BadRequest(new { message = "TenantId is required" });

            if (string.IsNullOrWhiteSpace(dto.FeatureName))
                return BadRequest(new { message = "Feature name is required" });
            
            try
            {
                await _service.UpdateFeatureAsync(id, dto.FeatureName, dto.Enabled, dto.TenantId);
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

        private static BusinessConfigurationDto MapToDto(Domain.Entities.BusinessConfiguration config)
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
                CreditCardPayments = config.CreditCardPayments,
                CreatedAt = config.CreatedAt,
                UpdatedAt = config.UpdatedAt
            };
        }
    }

    public class CreateBusinessConfigurationForClinicDto
    {
        public Guid ClinicId { get; set; }
        public BusinessType BusinessType { get; set; }
        public ProfessionalSpecialty PrimarySpecialty { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }

    public class UpdateBusinessTypeForClinicDto
    {
        public BusinessType BusinessType { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }

    public class UpdatePrimarySpecialtyForClinicDto
    {
        public ProfessionalSpecialty PrimarySpecialty { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }

    public class UpdateFeatureForClinicDto
    {
        public string FeatureName { get; set; } = string.Empty;
        public bool Enabled { get; set; }
        public string TenantId { get; set; } = string.Empty;
    }
}
