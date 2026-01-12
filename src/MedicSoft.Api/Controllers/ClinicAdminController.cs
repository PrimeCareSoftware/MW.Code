using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for clinic owner administration
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClinicAdminController : BaseController
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly IOwnerClinicLinkRepository _ownerClinicLinkRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ILogger<ClinicAdminController> _logger;

        public ClinicAdminController(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            ILogger<ClinicAdminController> logger,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _subscriptionRepository = subscriptionRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get clinic information (owner only)
        /// </summary>
        [HttpGet("info")]
        public async Task<ActionResult<ClinicAdminInfoDto>> GetClinicInfo()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                // Verify owner and get clinic
                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var ownerLink = ownerLinks.FirstOrDefault();
                
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var clinic = await _clinicRepository.GetByIdAsync(ownerLink.ClinicId, tenantId);
                if (clinic == null)
                {
                    return NotFound(new { message = "Clinic not found" });
                }

                return Ok(MapClinicToDto(clinic));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinic information");
                return StatusCode(500, new { message = "An error occurred while retrieving clinic information" });
            }
        }

        /// <summary>
        /// Update clinic information (owner only)
        /// </summary>
        [HttpPut("info")]
        public async Task<ActionResult<ClinicAdminInfoDto>> UpdateClinicInfo([FromBody] UpdateClinicInfoRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var ownerLink = ownerLinks.FirstOrDefault();
                
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var clinic = await _clinicRepository.GetByIdAsync(ownerLink.ClinicId, tenantId);
                if (clinic == null)
                {
                    return NotFound(new { message = "Clinic not found" });
                }

                // Update clinic info - keeping name and document unchanged
                if (!string.IsNullOrWhiteSpace(request.Phone) || 
                    !string.IsNullOrWhiteSpace(request.Email) || 
                    !string.IsNullOrWhiteSpace(request.Address))
                {
                    clinic.UpdateInfo(
                        clinic.Name,
                        clinic.TradeName,
                        request.Phone ?? clinic.Phone,
                        request.Email ?? clinic.Email,
                        request.Address ?? clinic.Address
                    );
                }

                // Update schedule settings if provided
                if (request.OpeningTime.HasValue || request.ClosingTime.HasValue || 
                    request.AppointmentDurationMinutes.HasValue || request.AllowEmergencySlots.HasValue)
                {
                    clinic.UpdateScheduleSettings(
                        request.OpeningTime ?? clinic.OpeningTime,
                        request.ClosingTime ?? clinic.ClosingTime,
                        request.AppointmentDurationMinutes ?? clinic.AppointmentDurationMinutes,
                        request.AllowEmergencySlots ?? clinic.AllowEmergencySlots
                    );
                }

                await _clinicRepository.UpdateAsync(clinic);

                _logger.LogInformation("Clinic information updated: {ClinicId}", clinic.Id);

                return Ok(MapClinicToDto(clinic));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic information");
                return StatusCode(500, new { message = "An error occurred while updating clinic information" });
            }
        }

        /// <summary>
        /// Get clinic users (owner only)
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<IEnumerable<ClinicUserDto>>> GetClinicUsers()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var ownerLink = ownerLinks.FirstOrDefault();
                
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var users = await _userRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                return Ok(users.Select(u => new ClinicUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Name = u.FullName,
                    Email = u.Email,
                    Role = u.Role.ToString(),
                    IsActive = u.IsActive,
                    CreatedAt = u.CreatedAt
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinic users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        /// <summary>
        /// Get clinic subscription information (owner only)
        /// </summary>
        [HttpGet("subscription")]
        public async Task<ActionResult> GetSubscription()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var ownerLink = ownerLinks.FirstOrDefault();
                
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                if (subscription == null || !subscription.IsActive())
                {
                    return NotFound(new { message = "No active subscription found" });
                }

                return Ok(new
                {
                    subscription.Id,
                    PlanId = subscription.SubscriptionPlanId,
                    subscription.Status,
                    subscription.StartDate,
                    subscription.EndDate,
                    NextBillingDate = subscription.NextPaymentDate,
                    IsTrial = subscription.Status == SubscriptionStatus.Trial,
                    IsActive = subscription.IsActive(),
                    subscription.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription");
                return StatusCode(500, new { message = "An error occurred while retrieving subscription" });
            }
        }

        /// <summary>
        /// Cancel clinic subscription (owner only)
        /// </summary>
        [HttpPut("subscription/cancel")]
        public async Task<ActionResult> CancelSubscription()
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var ownerLink = ownerLinks.FirstOrDefault();
                
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                if (subscription == null || !subscription.IsActive())
                {
                    return NotFound(new { message = "No active subscription found" });
                }

                subscription.Cancel("Cancelled by clinic owner");
                await _subscriptionRepository.UpdateAsync(subscription);

                _logger.LogInformation("Subscription cancelled for clinic: {ClinicId}", ownerLink.ClinicId);

                return Ok(new { message = "Subscription cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription");
                return StatusCode(500, new { message = "An error occurred while cancelling subscription" });
            }
        }

        private static ClinicAdminInfoDto MapClinicToDto(Domain.Entities.Clinic clinic)
        {
            return new ClinicAdminInfoDto
            {
                ClinicId = clinic.Id,
                Name = clinic.Name,
                TradeName = clinic.TradeName,
                Document = clinic.Document,
                Phone = clinic.Phone,
                Email = clinic.Email,
                Address = clinic.Address,
                Subdomain = clinic.Subdomain,
                OpeningTime = clinic.OpeningTime,
                ClosingTime = clinic.ClosingTime,
                AppointmentDurationMinutes = clinic.AppointmentDurationMinutes,
                AllowEmergencySlots = clinic.AllowEmergencySlots,
                IsActive = clinic.IsActive
            };
        }
    }
}
