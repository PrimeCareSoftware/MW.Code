using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/consultation-form-profiles")]
    [Authorize]
    public class ConsultationFormProfilesController : BaseController
    {
        private readonly IConsultationFormConfigurationService _formConfigService;

        public ConsultationFormProfilesController(
            IConsultationFormConfigurationService formConfigService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _formConfigService = formConfigService;
        }

        /// <summary>
        /// Get all active consultation form profiles
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        public async Task<ActionResult<IEnumerable<ConsultationFormProfileDto>>> GetAll()
        {
            try
            {
                var profiles = await _formConfigService.GetActiveProfilesAsync(GetTenantId());
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get consultation form profile by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        public async Task<ActionResult<ConsultationFormProfileDto>> GetById(Guid id)
        {
            try
            {
                var profile = await _formConfigService.GetProfileByIdAsync(id, GetTenantId());
                if (profile == null)
                    return NotFound();

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get consultation form profiles by specialty
        /// </summary>
        [HttpGet("specialty/{specialty}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        public async Task<ActionResult<IEnumerable<ConsultationFormProfileDto>>> GetBySpecialty(ProfessionalSpecialty specialty)
        {
            try
            {
                var profiles = await _formConfigService.GetProfilesBySpecialtyAsync(specialty, GetTenantId());
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new consultation form profile (requires form-configuration.manage permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult<ConsultationFormProfileDto>> Create([FromBody] CreateConsultationFormProfileDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var profile = await _formConfigService.CreateProfileAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = profile.Id }, profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a consultation form profile (requires form-configuration.manage permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult<ConsultationFormProfileDto>> Update(Guid id, [FromBody] UpdateConsultationFormProfileDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var profile = await _formConfigService.UpdateProfileAsync(id, updateDto, GetTenantId());
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a consultation form profile (requires form-configuration.manage permission)
        /// System default profiles cannot be deleted
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _formConfigService.DeleteProfileAsync(id, GetTenantId());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
