using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Interfaces;
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
        private readonly IClinicRepository _clinicRepository;

        public AccessProfilesController(
            ITenantContext tenantContext,
            IAccessProfileService profileService,
            IClinicRepository clinicRepository) : base(tenantContext)
        {
            _profileService = profileService;
            _clinicRepository = clinicRepository;
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

        /// <summary>
        /// Create default profiles for the clinic based on clinic type (Owner only)
        /// This endpoint will create appropriate profiles based on the clinic specialty
        /// </summary>
        [HttpPost("create-defaults-by-type")]
        public async Task<ActionResult<IEnumerable<AccessProfileDto>>> CreateDefaultProfilesByClinicType()
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Get clinic to determine type
                var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
                if (clinic == null)
                    return NotFound(new { message = "Clinic not found" });

                var profiles = await _profileService.CreateDefaultProfilesForClinicTypeAsync(clinicId, tenantId, clinic.ClinicType);
                return Ok(profiles);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Set or update the consultation form profile for an access profile (Owner only)
        /// </summary>
        [HttpPut("{id}/consultation-form-profile")]
        public async Task<ActionResult<AccessProfileDto>> SetConsultationFormProfile(Guid id, [FromBody] SetConsultationFormProfileDto request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                // Verify profile exists and belongs to clinic (combined check to avoid information disclosure)
                var profile = await _profileService.GetByIdAsync(id, tenantId);
                if (profile == null || profile.ClinicId != clinicId)
                    return NotFound(new { message = "Profile not found" });

                var updatedProfile = await _profileService.SetConsultationFormProfileAsync(id, request.ConsultationFormProfileId, tenantId);
                return Ok(updatedProfile);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Backfill missing default profiles for all clinics in the tenant (Owner only)
        /// This ensures existing clinics have access to all professional profile types, not just their clinic type's profile.
        /// This is useful after updating the system to support multi-specialty clinics.
        /// </summary>
        [HttpPost("backfill-missing-profiles")]
        public async Task<ActionResult<BackfillProfilesResult>> BackfillMissingProfiles()
        {
            try
            {
                var tenantId = GetTenantId();

                // Verify user is owner
                if (!IsOwner())
                    return Forbid();

                var result = await _profileService.BackfillMissingProfilesForAllClinicsAsync(tenantId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Sync default profile permissions with latest definitions (System Admin only)
        /// This endpoint updates existing default profiles with any missing permissions
        /// </summary>
        [HttpPost("sync-permissions")]
        public async Task<ActionResult<SyncProfilePermissionsResult>> SyncDefaultProfilePermissions()
        {
            try
            {
                // Only system admins can run this operation
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;
                if (roleClaim != RoleNames.SystemAdmin)
                    return Forbid();

                var tenantId = GetTenantId();
                var result = await _profileService.SyncDefaultProfilePermissionsAsync(tenantId);
                
                return Ok(new
                {
                    message = $"Sync completed. {result.ProfilesUpdated} profiles updated, {result.ProfilesSkipped} skipped.",
                    data = result
                });
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
