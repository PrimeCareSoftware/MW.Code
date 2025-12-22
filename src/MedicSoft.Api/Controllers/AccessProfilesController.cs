using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using System.Security.Claims;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for access profile management - only accessible by clinic owners
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AccessProfilesController : BaseController
    {
        private readonly IAccessProfileService _profileService;

        public AccessProfilesController(
            ITenantContext tenantContext,
            IAccessProfileService profileService) : base(tenantContext)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Get all access profiles for the clinic (Owner only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AccessProfileDto>>> GetProfiles()
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                var profiles = await _profileService.GetByClinicIdAsync(clinicId, tenantId);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get profile by ID (Owner only)
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<AccessProfileDto>> GetProfile(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                var profile = await _profileService.GetByIdAsync(id, tenantId);
                if (profile == null)
                    return NotFound(new { message = "Profile not found" });

                // Verify profile belongs to user's clinic
                var clinicId = GetClinicIdFromToken();
                if (profile.ClinicId != clinicId)
                    return Forbid();

                return Ok(profile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create new access profile (Owner only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AccessProfileDto>> CreateProfile([FromBody] CreateAccessProfileDto request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Override ClinicId from token
                request.ClinicId = clinicId;

                var profile = await _profileService.CreateAsync(request, tenantId);
                return CreatedAtAction(nameof(GetProfile), new { id = profile.Id }, profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update access profile (Owner only)
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<AccessProfileDto>> UpdateProfile(Guid id, [FromBody] UpdateAccessProfileDto request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Verify profile belongs to user's clinic
                var existingProfile = await _profileService.GetByIdAsync(id, tenantId);
                if (existingProfile == null)
                    return NotFound(new { message = "Profile not found" });

                if (existingProfile.ClinicId != clinicId)
                    return Forbid();

                var profile = await _profileService.UpdateAsync(id, request, tenantId);
                return Ok(profile);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete access profile (Owner only)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteProfile(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Verify profile belongs to user's clinic
                var existingProfile = await _profileService.GetByIdAsync(id, tenantId);
                if (existingProfile == null)
                    return NotFound(new { message = "Profile not found" });

                if (existingProfile.ClinicId != clinicId)
                    return Forbid();

                await _profileService.DeleteAsync(id, tenantId);
                return Ok(new { message = "Profile deleted successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get all available permissions grouped by category
        /// </summary>
        [HttpGet("permissions")]
        public async Task<ActionResult<IEnumerable<PermissionCategoryDto>>> GetAllPermissions()
        {
            try
            {
                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                var permissions = await _profileService.GetAllPermissionsAsync();
                return Ok(permissions);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Assign profile to user (Owner only)
        /// </summary>
        [HttpPost("assign")]
        public async Task<ActionResult> AssignProfileToUser([FromBody] AssignProfileDto request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Verify profile belongs to user's clinic
                var profile = await _profileService.GetByIdAsync(request.ProfileId, tenantId);
                if (profile == null)
                    return NotFound(new { message = "Profile not found" });

                if (profile.ClinicId != clinicId)
                    return Forbid();

                await _profileService.AssignProfileToUserAsync(request.UserId, request.ProfileId, tenantId);
                return Ok(new { message = "Profile assigned successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Create default profiles for the clinic (Owner only, called during clinic registration)
        /// </summary>
        [HttpPost("create-defaults")]
        public async Task<ActionResult<IEnumerable<AccessProfileDto>>> CreateDefaultProfiles()
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                var profiles = await _profileService.CreateDefaultProfilesAsync(clinicId, tenantId);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
        }

        private bool IsOwner()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
            return roleClaim == RoleNames.ClinicOwner || roleClaim == RoleNames.SystemAdmin;
        }
    }
}
