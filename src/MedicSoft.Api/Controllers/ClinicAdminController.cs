using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ClinicAdminController> _logger;

        public ClinicAdminController(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            IPasswordHasher passwordHasher,
            ILogger<ClinicAdminController> logger,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _subscriptionRepository = subscriptionRepository;
            _passwordHasher = passwordHasher;
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

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                // Verify owner and get clinic
                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
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

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
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

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var users = await _userRepository.GetByClinicIdAsync(ownerLink.ClinicId, tenantId);
                
                return Ok(users.Select(u => new ClinicUserDto
                {
                    Id = u.Id,
                    Username = u.Username,
                    Name = u.Name,
                    Email = u.Email,
                    Role = u.Role,
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
        /// Create clinic user (owner only)
        /// </summary>
        [HttpPost("users")]
        public async Task<ActionResult<ClinicUserDto>> CreateClinicUser([FromBody] CreateClinicUserRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                // Check if username already exists
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username, tenantId);
                if (existingUser != null)
                {
                    return BadRequest(new { message = "Username already exists" });
                }

                // Create new user
                var hashedPassword = _passwordHasher.HashPassword(request.Password);
                var user = new Domain.Entities.User(
                    request.Username,
                    hashedPassword,
                    request.Name,
                    request.Email,
                    request.Role,
                    ownerLink.ClinicId,
                    tenantId
                );

                if (!string.IsNullOrWhiteSpace(request.Phone))
                {
                    user.UpdateContactInfo(request.Phone, request.Email);
                }

                await _userRepository.AddAsync(user);

                _logger.LogInformation("User created: {Username} for clinic: {ClinicId}", request.Username, ownerLink.ClinicId);

                return CreatedAtAction(nameof(GetClinicUsers), new ClinicUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating clinic user");
                return StatusCode(500, new { message = "An error occurred while creating user" });
            }
        }

        /// <summary>
        /// Update clinic user (owner only)
        /// </summary>
        [HttpPut("users/{id}")]
        public async Task<ActionResult<ClinicUserDto>> UpdateClinicUser(Guid id, [FromBody] UpdateClinicUserRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null || user.ClinicId != ownerLink.ClinicId)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (!string.IsNullOrWhiteSpace(request.Name))
                {
                    user.UpdateName(request.Name);
                }

                if (!string.IsNullOrWhiteSpace(request.Phone) || !string.IsNullOrWhiteSpace(request.Email))
                {
                    user.UpdateContactInfo(request.Phone ?? user.Phone, request.Email ?? user.Email);
                }

                if (request.IsActive.HasValue)
                {
                    if (request.IsActive.Value)
                        user.Activate();
                    else
                        user.Deactivate();
                }

                await _userRepository.UpdateAsync(user);

                _logger.LogInformation("User updated: {UserId}", id);

                return Ok(new ClinicUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic user");
                return StatusCode(500, new { message = "An error occurred while updating user" });
            }
        }

        /// <summary>
        /// Delete clinic user (owner only)
        /// </summary>
        [HttpDelete("users/{id}")]
        public async Task<ActionResult> DeleteClinicUser(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null || user.ClinicId != ownerLink.ClinicId)
                {
                    return NotFound(new { message = "User not found" });
                }

                await _userRepository.DeleteAsync(id, tenantId);

                _logger.LogInformation("User deleted: {UserId}", id);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting clinic user");
                return StatusCode(500, new { message = "An error occurred while deleting user" });
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

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetActiveByClinicIdAsync(ownerLink.ClinicId);
                
                if (subscription == null)
                {
                    return NotFound(new { message = "No active subscription found" });
                }

                return Ok(new
                {
                    subscription.Id,
                    subscription.PlanId,
                    subscription.Status,
                    subscription.StartDate,
                    subscription.EndDate,
                    subscription.NextBillingDate,
                    subscription.IsTrial,
                    subscription.IsActive,
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

                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                var ownerLink = await _ownerClinicLinkRepository.GetByOwnerIdAsync(Guid.Parse(userId), tenantId);
                if (ownerLink == null)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetActiveByClinicIdAsync(ownerLink.ClinicId);
                
                if (subscription == null)
                {
                    return NotFound(new { message = "No active subscription found" });
                }

                subscription.Cancel();
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
