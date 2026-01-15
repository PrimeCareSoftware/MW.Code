using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClinicCustomizationController : BaseController
    {
        private readonly IClinicCustomizationRepository _customizationRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IOwnerClinicLinkRepository _ownerClinicLinkRepository;
        private readonly ILogger<ClinicCustomizationController> _logger;

        public ClinicCustomizationController(
            IClinicCustomizationRepository customizationRepository,
            IClinicRepository clinicRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            ILogger<ClinicCustomizationController> logger,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _customizationRepository = customizationRepository;
            _clinicRepository = clinicRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get clinic customization by subdomain (public endpoint for login page)
        /// </summary>
        [HttpGet("by-subdomain/{subdomain}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClinicCustomizationPublicDto>> GetBySubdomain(string subdomain)
        {
            try
            {
                var customization = await _customizationRepository.GetBySubdomainAsync(subdomain);
                
                if (customization == null || customization.Clinic == null)
                {
                    return NotFound(new { message = "Customization not found for this subdomain" });
                }

                return Ok(new ClinicCustomizationPublicDto
                {
                    LogoUrl = customization.LogoUrl,
                    BackgroundImageUrl = customization.BackgroundImageUrl,
                    PrimaryColor = customization.PrimaryColor,
                    SecondaryColor = customization.SecondaryColor,
                    FontColor = customization.FontColor,
                    ClinicName = customization.Clinic.Name
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting customization for subdomain: {Subdomain}", subdomain);
                return StatusCode(500, new { message = "An error occurred while retrieving customization" });
            }
        }

        /// <summary>
        /// Get current clinic customization (owner only)
        /// </summary>
        [HttpGet]
        [Authorize]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult<ClinicCustomizationDto>> GetCurrentClinicCustomization()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);

                if (!isAuthorized)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                // If no customization exists, create default one
                if (customization == null)
                {
                    customization = new ClinicCustomization(clinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                return Ok(MapToDto(customization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinic customization");
                return StatusCode(500, new { message = "An error occurred while retrieving customization" });
            }
        }

        /// <summary>
        /// Update clinic customization colors (owner only)
        /// </summary>
        [HttpPut("colors")]
        [Authorize]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateColors([FromBody] UpdateClinicCustomizationRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);

                if (!isAuthorized)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(clinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.UpdateColors(request.PrimaryColor, request.SecondaryColor, request.FontColor);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic customization colors updated for clinic: {ClinicId}", clinicId);

                return Ok(MapToDto(customization));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic customization colors");
                return StatusCode(500, new { message = "An error occurred while updating customization" });
            }
        }

        /// <summary>
        /// Update logo URL (owner only)
        /// </summary>
        [HttpPut("logo")]
        [Authorize]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateLogo([FromBody] string logoUrl)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);

                if (!isAuthorized)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(clinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.SetLogoUrl(logoUrl);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic logo updated for clinic: {ClinicId}", clinicId);

                return Ok(MapToDto(customization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic logo");
                return StatusCode(500, new { message = "An error occurred while updating logo" });
            }
        }

        /// <summary>
        /// Update background image URL (owner only)
        /// </summary>
        [HttpPut("background")]
        [Authorize]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateBackground([FromBody] string backgroundImageUrl)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);

                if (!isAuthorized)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(clinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.SetBackgroundImageUrl(backgroundImageUrl);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic background image updated for clinic: {ClinicId}", clinicId);

                return Ok(MapToDto(customization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic background image");
                return StatusCode(500, new { message = "An error occurred while updating background image" });
            }
        }

        /// <summary>
        /// Get clinic ID - from token for Users with ClinicOwner role, or from owner link for Owners
        /// </summary>
        private async Task<(Guid clinicId, bool isAuthorized)> GetClinicIdForOwnerAsync(Guid userId, string tenantId)
        {
            // Try to get clinic ID from token first (for Users with ClinicOwner role)
            var clinicId = GetClinicIdFromToken();
            
            if (clinicId != Guid.Empty)
            {
                return (clinicId, true);
            }
            
            // If no clinic ID in token, try to get from owner link (for Owner entities)
            var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
            var ownerLink = ownerLinks.FirstOrDefault();
            
            if (ownerLink == null)
            {
                return (Guid.Empty, false);
            }
            
            return (ownerLink.ClinicId, true);
        }

        /// <summary>
        /// Get clinic ID from JWT token
        /// </summary>
        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
        }

        private static ClinicCustomizationDto MapToDto(ClinicCustomization customization)
        {
            return new ClinicCustomizationDto
            {
                Id = customization.Id,
                ClinicId = customization.ClinicId,
                LogoUrl = customization.LogoUrl,
                BackgroundImageUrl = customization.BackgroundImageUrl,
                PrimaryColor = customization.PrimaryColor,
                SecondaryColor = customization.SecondaryColor,
                FontColor = customization.FontColor,
                CreatedAt = customization.CreatedAt,
                UpdatedAt = customization.UpdatedAt ?? customization.CreatedAt
            };
        }
    }
}
