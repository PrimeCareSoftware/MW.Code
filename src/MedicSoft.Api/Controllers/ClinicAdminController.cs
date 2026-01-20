using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Common;
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
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<ClinicAdminController> _logger;

        public ClinicAdminController(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IUserService userService,
            IPasswordHasher passwordHasher,
            ILogger<ClinicAdminController> logger,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _userService = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        /// <summary>
        /// Get clinic information (owner only)
        /// Note: Scoped to the current tenant context from JWT token
        /// </summary>
        [HttpGet("info")]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
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

                // Get clinic ID - works for both Owner entities and Users with ClinicOwner role
                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);
                
                if (!isAuthorized)
                {
                    return Forbid();
                }

                var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
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
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
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

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);
                
                if (!isAuthorized)
                {
                    return Forbid();
                }

                var clinic = await _clinicRepository.GetByIdAsync(clinicId, tenantId);
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
        [RequirePermissionKey(PermissionKeys.UsersView)]
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

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);
                
                if (!isAuthorized)
                {
                    return Forbid();
                }

                var users = await _userRepository.GetByClinicIdAsync(clinicId, tenantId);
                
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
        [RequirePermissionKey(PermissionKeys.ClinicView)]
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

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);
                
                if (!isAuthorized)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
                
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
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
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

                var (clinicId, isAuthorized) = await GetClinicIdForOwnerAsync(userId, tenantId);
                
                if (!isAuthorized)
                {
                    return Forbid();
                }

                var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                if (subscription == null || !subscription.IsActive())
                {
                    return NotFound(new { message = "No active subscription found" });
                }

                subscription.Cancel("Cancelled by clinic owner");
                await _subscriptionRepository.UpdateAsync(subscription);

                _logger.LogInformation("Subscription cancelled for clinic: {ClinicId}", clinicId);

                return Ok(new { message = "Subscription cancelled successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling subscription");
                return StatusCode(500, new { message = "An error occurred while cancelling subscription" });
            }
        }

        /// <summary>
        /// Create a new user in the clinic (owner only)
        /// Note: User is created for the clinic in the current tenant context
        /// </summary>
        [HttpPost("users")]
        [RequirePermissionKey(PermissionKeys.UsersCreate)]
        public async Task<ActionResult<ClinicUserDto>> CreateClinicUser([FromBody] CreateClinicUserRequest request)
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

                // Check subscription limits
                var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
                if (subscription == null || !subscription.IsActive())
                {
                    return BadRequest(new { message = "No active subscription found" });
                }

                var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, "system");
                if (plan == null)
                {
                    return BadRequest(new { message = "Invalid subscription plan" });
                }

                var currentUserCount = await _userService.GetUserCountByClinicIdAsync(clinicId, tenantId);
                if (currentUserCount >= plan.MaxUsers)
                {
                    return BadRequest(new { message = $"User limit reached. Current plan allows {plan.MaxUsers} users. Please upgrade your plan." });
                }

                // Parse role
                if (!Enum.TryParse<UserRole>(request.Role, out var role))
                {
                    return BadRequest(new { message = "Invalid role" });
                }

                var user = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.Name,
                    request.Phone ?? "",
                    role,
                    tenantId,
                    clinicId
                );

                _logger.LogInformation("User created in clinic: {UserId} in {ClinicId}", user.Id, clinicId);

                return Ok(new ClinicUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.FullName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt
                });
            }
            catch (InvalidOperationException ex)
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
        /// Update user information (owner only)
        /// </summary>
        [HttpPut("users/{id}")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> UpdateClinicUser(Guid id, [FromBody] UpdateClinicUserRequest request)
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

                // Verify user belongs to this clinic
                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.ClinicId != clinicId)
                {
                    return Forbid();
                }

                // Update profile
                await _userService.UpdateUserProfileAsync(
                    id,
                    request.Email ?? user.Email,
                    request.Name ?? user.FullName,
                    request.Phone ?? user.Phone,
                    tenantId
                );

                // Handle activation/deactivation
                if (request.IsActive.HasValue)
                {
                    if (request.IsActive.Value && !user.IsActive)
                    {
                        await _userService.ActivateUserAsync(id, tenantId);
                    }
                    else if (!request.IsActive.Value && user.IsActive)
                    {
                        await _userService.DeactivateUserAsync(id, tenantId);
                    }
                }

                _logger.LogInformation("User updated: {UserId}", id);

                return Ok(new { message = "User updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating clinic user");
                return StatusCode(500, new { message = "An error occurred while updating user" });
            }
        }

        /// <summary>
        /// Change user password (owner only)
        /// </summary>
        [HttpPut("users/{id}/password")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> ChangeUserPassword(Guid id, [FromBody] ChangeUserPasswordRequest request)
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

                // Verify user belongs to this clinic
                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.ClinicId != clinicId)
                {
                    return Forbid();
                }

                // Validate password strength
                var minPasswordLength = 8;
                var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(
                    request.NewPassword,
                    minPasswordLength);

                if (!isValid)
                {
                    return BadRequest(new { message = errorMessage });
                }

                // Change password
                await _userService.ChangeUserPasswordAsync(id, request.NewPassword, tenantId);

                _logger.LogInformation("Password changed for user: {UserId} by owner: {OwnerId}", id, userId);

                return Ok(new { message = "Password changed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user password");
                return StatusCode(500, new { message = "An error occurred while changing password" });
            }
        }

        /// <summary>
        /// Deactivate user (owner only)
        /// </summary>
        [HttpPost("users/{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.UsersDelete)]
        public async Task<ActionResult> DeactivateClinicUser(Guid id)
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

                // Verify user belongs to this clinic
                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.ClinicId != clinicId)
                {
                    return Forbid();
                }

                await _userService.DeactivateUserAsync(id, tenantId);

                _logger.LogInformation("User deactivated: {UserId}", id);

                return Ok(new { message = "User deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating user");
                return StatusCode(500, new { message = "An error occurred while deactivating user" });
            }
        }

        /// <summary>
        /// Activate user (owner only)
        /// </summary>
        [HttpPost("users/{id}/activate")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> ActivateClinicUser(Guid id)
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

                // Verify user belongs to this clinic
                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.ClinicId != clinicId)
                {
                    return Forbid();
                }

                await _userService.ActivateUserAsync(id, tenantId);

                _logger.LogInformation("User activated: {UserId}", id);

                return Ok(new { message = "User activated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating user");
                return StatusCode(500, new { message = "An error occurred while activating user" });
            }
        }

        /// <summary>
        /// Change user role (owner only)
        /// </summary>
        [HttpPut("users/{id}/role")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> ChangeClinicUserRole(Guid id, [FromBody] ChangeUserRoleRequest request)
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

                // Verify user belongs to this clinic
                var user = await _userRepository.GetByIdAsync(id, tenantId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                if (user.ClinicId != clinicId)
                {
                    return Forbid();
                }

                // Parse role
                if (!Enum.TryParse<UserRole>(request.NewRole, out var newRole))
                {
                    return BadRequest(new { message = "Invalid role" });
                }

                await _userService.ChangeUserRoleAsync(id, newRole, tenantId);

                _logger.LogInformation("User role changed: {UserId} to {NewRole}", id, newRole);

                return Ok(new { message = "User role changed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing user role");
                return StatusCode(500, new { message = "An error occurred while changing user role" });
            }
        }

        /// <summary>
        /// Get subscription details with plan limits (owner only)
        /// </summary>
        [HttpGet("subscription/details")]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult> GetSubscriptionDetails()
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

                var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
                
                if (subscription == null)
                {
                    return NotFound(new { message = "No subscription found" });
                }

                var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, "system");
                if (plan == null)
                {
                    return NotFound(new { message = "Subscription plan not found" });
                }

                var currentUserCount = await _userService.GetUserCountByClinicIdAsync(clinicId, tenantId);

                return Ok(new
                {
                    subscription.Id,
                    PlanId = subscription.SubscriptionPlanId,
                    PlanName = plan.Name,
                    PlanType = plan.Type.ToString(),
                    subscription.Status,
                    subscription.StartDate,
                    subscription.EndDate,
                    NextBillingDate = subscription.NextPaymentDate,
                    CurrentPrice = subscription.CurrentPrice,
                    IsTrial = subscription.Status == SubscriptionStatus.Trial,
                    IsActive = subscription.IsActive(),
                    Limits = new
                    {
                        MaxUsers = plan.MaxUsers,
                        MaxPatients = plan.MaxPatients,
                        CurrentUsers = currentUserCount
                    },
                    Features = new
                    {
                        HasReports = plan.HasReports,
                        HasWhatsAppIntegration = plan.HasWhatsAppIntegration,
                        HasSMSNotifications = plan.HasSMSNotifications,
                        HasTissExport = plan.HasTissExport
                    },
                    subscription.CreatedAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting subscription details");
                return StatusCode(500, new { message = "An error occurred while retrieving subscription details" });
            }
        }

        /// <summary>
        /// Get all clinics owned by the authenticated owner
        /// </summary>
        [HttpGet("my-clinics")]
        public async Task<ActionResult> GetMyClinics()
        {
            try
            {
                var userId = GetUserId();

                if (userId == Guid.Empty)
                {
                    return Unauthorized();
                }

                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                
                var clinicDetails = new List<object>();
                
                foreach (var link in ownerLinks)
                {
                    if (!link.IsActive) continue;
                    
                    var clinic = await _clinicRepository.GetByIdAsync(link.ClinicId, link.Clinic?.TenantId ?? "");
                    if (clinic == null) continue;

                    var subscription = await _subscriptionRepository.GetByClinicIdAsync(link.ClinicId, clinic.TenantId);
                    
                    clinicDetails.Add(new
                    {
                        ClinicId = clinic.Id,
                        Name = clinic.Name,
                        TradeName = clinic.TradeName,
                        Document = clinic.Document,
                        Subdomain = clinic.Subdomain,
                        TenantId = clinic.TenantId,
                        IsActive = clinic.IsActive,
                        IsPrimaryOwner = link.IsPrimaryOwner,
                        HasActiveSubscription = subscription?.IsActive() ?? false,
                        SubscriptionStatus = subscription?.Status.ToString()
                    });
                }

                return Ok(clinicDetails);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting owner clinics");
                return StatusCode(500, new { message = "An error occurred while retrieving clinics" });
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

    public class ChangeUserPasswordRequest
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangeUserRoleRequest
    {
        public string NewRole { get; set; } = string.Empty;
    }
}
