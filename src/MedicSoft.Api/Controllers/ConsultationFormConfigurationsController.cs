using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/consultation-form-configurations")]
    [Authorize]
    public class ConsultationFormConfigurationsController : BaseController
    {
        private readonly IConsultationFormConfigurationService _formConfigService;
        private readonly IUserClinicLinkRepository _userClinicLinkRepository;

        public ConsultationFormConfigurationsController(
            IConsultationFormConfigurationService formConfigService,
            IUserClinicLinkRepository userClinicLinkRepository,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _formConfigService = formConfigService ?? throw new ArgumentNullException(nameof(formConfigService));
            _userClinicLinkRepository = userClinicLinkRepository ?? throw new ArgumentNullException(nameof(userClinicLinkRepository));
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
                // Validate user has access to the requested clinic
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized("User ID not found in token");
                }

                var hasAccess = await _userClinicLinkRepository.UserHasAccessToClinicAsync(userId, clinicId, GetTenantId());
                if (!hasAccess)
                {
                    return Forbid("You do not have access to this clinic's consultation forms");
                }

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
                // Validate user has access to the clinic
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized("User ID not found in token");
                }

                var hasAccess = await _userClinicLinkRepository.UserHasAccessToClinicAsync(userId, createDto.ClinicId, GetTenantId());
                if (!hasAccess)
                {
                    return Forbid("You do not have access to create configurations for this clinic");
                }

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
                // Validate user has access to the clinic
                var userId = GetUserId();
                if (userId == Guid.Empty)
                {
                    return Unauthorized("User ID not found in token");
                }

                var hasAccess = await _userClinicLinkRepository.UserHasAccessToClinicAsync(userId, request.ClinicId, GetTenantId());
                if (!hasAccess)
                {
                    return Forbid("You do not have access to create configurations for this clinic");
                }

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

        /// <summary>
        /// Get terminology for a specific professional specialty
        /// Note: AllowAnonymous is safe here as terminology is generic localization data
        /// (e.g., "Consulta" vs "Sessão") and contains no sensitive information
        /// </summary>
        [HttpGet("terminology/{specialty}")]
        [AllowAnonymous] // Allow anonymous access for public scheduling interfaces
        public ActionResult<Dictionary<string, string>> GetTerminology(string specialty)
        {
            try
            {
                if (!Enum.TryParse<Domain.Enums.ProfessionalSpecialty>(specialty, true, out var specialtyEnum))
                {
                    return BadRequest($"Invalid specialty: {specialty}");
                }

                var terminology = Domain.ValueObjects.TerminologyMap.For(specialtyEnum);
                return Ok(terminology.ToDictionary());
            }
            catch (Exception ex)
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
