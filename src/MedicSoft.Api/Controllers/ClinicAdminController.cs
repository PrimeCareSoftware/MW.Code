using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Application.Helpers;
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
        private readonly BusinessConfigurationService _businessConfigService;
        private readonly IAccessProfileService _accessProfileService;

        public ClinicAdminController(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IUserService userService,
            IPasswordHasher passwordHasher,
            ILogger<ClinicAdminController> logger,
            ITenantContext tenantContext,
            BusinessConfigurationService businessConfigService,
            IAccessProfileService accessProfileService)
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
            _businessConfigService = businessConfigService;
            _accessProfileService = accessProfileService;
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

                // Update number of rooms if provided
                if (request.NumberOfRooms.HasValue)
                {
                    clinic.UpdateNumberOfRooms(request.NumberOfRooms.Value);
                }

                // Update notification setting if provided
                if (request.NotifyPrimaryDoctorOnOtherDoctorAppointment.HasValue)
                {
                    clinic.UpdateNotifyPrimaryDoctorSetting(request.NotifyPrimaryDoctorOnOtherDoctorAppointment.Value);
                }

                // Update online appointment scheduling setting if provided
                if (request.EnableOnlineAppointmentScheduling.HasValue)
                {
                    clinic.UpdateOnlineSchedulingSetting(request.EnableOnlineAppointmentScheduling.Value);
                }

                await _clinicRepository.UpdateAsync(clinic);
                
                // Sync relevant properties with BusinessConfiguration
                try
                {
                    await _businessConfigService.SyncClinicPropertiesToBusinessConfigAsync(clinicId, tenantId);
                }
                catch (Exception ex)
                {
                    // Log but don't fail the clinic update if business config sync fails
                    _logger.LogWarning(ex, "Failed to sync clinic properties to business configuration for clinic {ClinicId}", clinicId);
                }

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
        /// Supports both role-based (legacy) and profile-based (new) user creation
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

                // Subscription plans are system-wide entities with tenantId="system"
                // Use GetByIdWithoutTenantAsync to retrieve them
                var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
                if (plan == null)
                {
                    return BadRequest(new { message = "Invalid subscription plan" });
                }

                var currentUserCount = await _userService.GetUserCountByClinicIdAsync(clinicId, tenantId);
                if (currentUserCount >= plan.MaxUsers)
                {
                    return BadRequest(new { message = $"User limit reached. Current plan allows {plan.MaxUsers} users. Please upgrade your plan." });
                }

                // Determine if using profile-based or role-based creation
                UserRole role;
                Guid? profileIdToAssign = null;

                if (request.ProfileId.HasValue)
                {
                    // Profile-based creation (new system)
                    var profile = await _accessProfileService.GetByIdAsync(request.ProfileId.Value, tenantId);
                    if (profile == null)
                    {
                        return BadRequest(new { message = "Invalid profile ID" });
                    }

                    // Verify profile belongs to this clinic
                    if (profile.ClinicId != clinicId)
                    {
                        return BadRequest(new { message = "Profile does not belong to this clinic" });
                    }

                    // Map profile name to a UserRole for backward compatibility
                    // This ensures the user has a valid role enum value
                    role = ProfileMappingHelper.MapProfileNameToRole(profile.Name, _logger);
                    profileIdToAssign = request.ProfileId.Value;

                    _logger.LogInformation("Creating user with profile-based system. Profile: {ProfileId}, MappedRole: {Role}", 
                        request.ProfileId.Value, role);
                }
                else
                {
                    // Role-based creation (legacy system)
                    if (!Enum.TryParse<UserRole>(request.Role, true, out role))
                    {
                        var allowedRoles = ProfileMappingHelper.GetAllowedRolesForCreation();
                        return BadRequest(new { message = $"Invalid role: {request.Role}. Valid roles are: {string.Join(", ", allowedRoles)}" });
                    }

                    // Prevent creation of SystemAdmin through this endpoint
                    if (role == UserRole.SystemAdmin)
                    {
                        return BadRequest(new { message = "SystemAdmin users cannot be created through this endpoint" });
                    }

                    _logger.LogInformation("Creating user with role-based system. Role: {Role}", role);
                }

                // Create the user with the determined role
                var user = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.Name,
                    request.Phone ?? "",
                    role,
                    tenantId,
                    clinicId,
                    request.ProfessionalId,
                    request.Specialty
                );

                // If using profile-based creation, assign the profile to the user
                if (profileIdToAssign.HasValue)
                {
                    await _accessProfileService.AssignProfileToUserAsync(user.Id, profileIdToAssign.Value, tenantId);
                    _logger.LogInformation("Profile {ProfileId} assigned to user {UserId}", profileIdToAssign.Value, user.Id);
                }

                _logger.LogInformation("User created in clinic: {UserId} in {ClinicId}", user.Id, clinicId);

                return Ok(new ClinicUserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Name = user.FullName,
                    Email = user.Email,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    CreatedAt = user.CreatedAt,
                    ProfessionalId = user.ProfessionalId,
                    Specialty = user.Specialty,
                    ProfileId = profileIdToAssign
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

                // Subscription plans are system-wide entities with tenantId="system"
                // Use GetByIdWithoutTenantAsync to retrieve them
                var plan = await _planRepository.GetByIdWithoutTenantAsync(subscription.SubscriptionPlanId);
                if (plan == null)
                {
                    return NotFound(new { message = "Subscription plan not found" });
                }

                var currentUserCount = await _userService.GetUserCountByClinicIdAsync(clinicId, tenantId);
                
                // Get current clinic count for the owner
                var ownerLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(userId);
                var currentClinicsCount = ownerLinks.Count(link => link.IsActive);

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
                        MaxClinics = plan.MaxClinics,
                        CurrentUsers = currentUserCount,
                        CurrentClinics = currentClinicsCount
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
        /// Get public display settings for the clinic
        /// </summary>
        [HttpGet("public-display-settings")]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult> GetPublicDisplaySettings()
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

                return Ok(new
                {
                    ShowOnPublicSite = clinic.ShowOnPublicSite,
                    ClinicType = clinic.ClinicType.ToString(),
                    WhatsAppNumber = clinic.WhatsAppNumber
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting public display settings");
                return StatusCode(500, new { message = "An error occurred while retrieving settings" });
            }
        }

        /// <summary>
        /// Update public display settings for the clinic
        /// </summary>
        [HttpPut("public-display-settings")]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult> UpdatePublicDisplaySettings([FromBody] UpdatePublicDisplaySettingsRequest request)
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

                // Parse clinic type
                if (!Enum.TryParse<Domain.Enums.ClinicType>(request.ClinicType, true, out var clinicType))
                {
                    return BadRequest(new { message = "Invalid clinic type" });
                }

                // Update public display settings
                clinic.UpdatePublicSiteSettings(request.ShowOnPublicSite, clinicType, request.WhatsAppNumber);
                await _clinicRepository.UpdateAsync(clinic);

                _logger.LogInformation("Public display settings updated for clinic: {ClinicId}", clinicId);

                return Ok(new
                {
                    message = "Settings updated successfully",
                    ShowOnPublicSite = clinic.ShowOnPublicSite,
                    ClinicType = clinic.ClinicType.ToString(),
                    WhatsAppNumber = clinic.WhatsAppNumber
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating public display settings");
                return StatusCode(500, new { message = "An error occurred while updating settings" });
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

        /// <summary>
        /// Update payment receiver configuration (owner only)
        /// </summary>
        [HttpPut("payment-receiver")]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult> UpdatePaymentReceiver([FromBody] UpdatePaymentReceiverRequest request)
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

                // Parse and update payment receiver type
                if (!Enum.TryParse<MedicSoft.Domain.Enums.PaymentReceiverType>(request.PaymentReceiverType, out var receiverType))
                {
                    return BadRequest(new { message = $"Invalid payment receiver type: {request.PaymentReceiverType}" });
                }

                clinic.UpdatePaymentReceiverType(receiverType);
                await _clinicRepository.UpdateAsync(clinic);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating payment receiver configuration");
                return StatusCode(500, new { message = "An error occurred while updating payment receiver configuration" });
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
                IsActive = clinic.IsActive,
                ShowOnPublicSite = clinic.ShowOnPublicSite,
                ClinicType = clinic.ClinicType.ToString(),
                WhatsAppNumber = clinic.WhatsAppNumber,
                DefaultPaymentReceiverType = clinic.DefaultPaymentReceiverType.ToString(),
                NumberOfRooms = clinic.NumberOfRooms,
                NotifyPrimaryDoctorOnOtherDoctorAppointment = clinic.NotifyPrimaryDoctorOnOtherDoctorAppointment,
                EnableOnlineAppointmentScheduling = clinic.EnableOnlineAppointmentScheduling
            };
        }

        /// <summary>
        /// Get doctor fields configuration (owner only)
        /// </summary>
        [HttpGet("doctor-fields-config")]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult<DoctorFieldsConfigDto>> GetDoctorFieldsConfiguration()
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

                var config = await _userService.GetDoctorFieldsConfigurationAsync(clinicId, tenantId);

                return Ok(new DoctorFieldsConfigDto
                {
                    ProfessionalIdRequired = config.ProfessionalIdRequired,
                    SpecialtyRequired = config.SpecialtyRequired
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor fields configuration");
                return StatusCode(500, new { message = "An error occurred while retrieving doctor fields configuration" });
            }
        }

        /// <summary>
        /// Update doctor fields configuration (owner only)
        /// </summary>
        [HttpPut("doctor-fields-config")]
        [RequirePermissionKey(PermissionKeys.ClinicManage)]
        public async Task<ActionResult> UpdateDoctorFieldsConfiguration([FromBody] DoctorFieldsConfigDto request)
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

                var config = new DoctorFieldsConfiguration(request.ProfessionalIdRequired, request.SpecialtyRequired);
                await _userService.UpdateDoctorFieldsConfigurationAsync(clinicId, tenantId, config);

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor fields configuration");
                return StatusCode(500, new { message = "An error occurred while updating doctor fields configuration" });
            }
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

    public class UpdatePublicDisplaySettingsRequest
    {
        public bool ShowOnPublicSite { get; set; }
        public string ClinicType { get; set; } = "Medical";
        public string? WhatsAppNumber { get; set; }
    }

    public class UpdatePaymentReceiverRequest
    {
        public string PaymentReceiverType { get; set; } = "Secretary"; // Doctor, Secretary, Other
    }

    public class DoctorFieldsConfigDto
    {
        public bool ProfessionalIdRequired { get; set; }
        public bool SpecialtyRequired { get; set; }
    }
}
