using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/consultation-form-configurations")]
    [Authorize]
    public class ConsultationFormConfigurationsController : BaseController
    {
        private readonly IConsultationFormConfigurationService _formConfigService;

        public ConsultationFormConfigurationsController(
            IConsultationFormConfigurationService formConfigService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _formConfigService = formConfigService;
        }

        /// <summary>
        /// Get active consultation form configuration for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        public async Task<ActionResult<ConsultationFormConfigurationDto>> GetByClinicId(Guid clinicId)
        {
            try
            {
                var configuration = await _formConfigService.GetActiveConfigurationByClinicIdAsync(clinicId, GetTenantId());
                if (configuration == null)
                    return NotFound();

                return Ok(configuration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get consultation form configuration by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        public async Task<ActionResult<ConsultationFormConfigurationDto>> GetById(Guid id)
        {
            try
            {
                var configuration = await _formConfigService.GetConfigurationByIdAsync(id, GetTenantId());
                if (configuration == null)
                    return NotFound();

                return Ok(configuration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new consultation form configuration (requires form-configuration.manage permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult<ConsultationFormConfigurationDto>> Create([FromBody] CreateConsultationFormConfigurationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var configuration = await _formConfigService.CreateConfigurationAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = configuration.Id }, configuration);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new consultation form configuration from an existing profile (requires form-configuration.manage permission)
        /// </summary>
        [HttpPost("from-profile")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult<ConsultationFormConfigurationDto>> CreateFromProfile([FromBody] CreateConfigurationFromProfileRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var configuration = await _formConfigService.CreateConfigurationFromProfileAsync(
                    request.ClinicId,
                    request.ProfileId,
                    GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = configuration.Id }, configuration);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a consultation form configuration (requires form-configuration.manage permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult<ConsultationFormConfigurationDto>> Update(Guid id, [FromBody] UpdateConsultationFormConfigurationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var configuration = await _formConfigService.UpdateConfigurationAsync(id, updateDto, GetTenantId());
                return Ok(configuration);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a consultation form configuration (requires form-configuration.manage permission)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _formConfigService.DeleteConfigurationAsync(id, GetTenantId());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CreateConfigurationFromProfileRequest
    {
        public Guid ClinicId { get; set; }
        public Guid ProfileId { get; set; }
    }
}
