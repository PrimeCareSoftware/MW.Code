using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
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
        public async Task<ActionResult<ClinicCustomizationDto>> GetCurrentClinicCustomization()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                // Get owner's clinic
                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(userId, tenantId);
                if (ownerLink == null)
                {
                    return NotFound(new { message = "Clinic not found for owner" });
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                // If no customization exists, create default one
                if (customization == null)
                {
                    customization = new ClinicCustomization(ownerLink.ClinicId, tenantId);
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
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateColors([FromBody] UpdateClinicCustomizationRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                // Verify owner
                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(userId, tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(ownerLink.ClinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.UpdateColors(request.PrimaryColor, request.SecondaryColor, request.FontColor);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic customization colors updated for clinic: {ClinicId}", ownerLink.ClinicId);

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
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateLogo([FromBody] string logoUrl)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(userId, tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(ownerLink.ClinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.SetLogoUrl(logoUrl);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic logo updated for clinic: {ClinicId}", ownerLink.ClinicId);

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
        public async Task<ActionResult<ClinicCustomizationDto>> UpdateBackground([FromBody] string backgroundImageUrl)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(userId, tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var customization = await _customizationRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                if (customization == null)
                {
                    customization = new ClinicCustomization(ownerLink.ClinicId, tenantId);
                    await _customizationRepository.AddAsync(customization);
                }

                customization.SetBackgroundImageUrl(backgroundImageUrl);
                await _customizationRepository.UpdateAsync(customization);

                _logger.LogInformation("Clinic background image updated for clinic: {ClinicId}", ownerLink.ClinicId);

                return Ok(MapToDto(customization));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic background image");
                return StatusCode(500, new { message = "An error occurred while updating background image" });
            }
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
                UpdatedAt = customization.UpdatedAt
            };
        }
    }
}
