using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for user management - create, update, manage roles
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IClinicSelectionService _clinicSelectionService;

        public UsersController(
            ITenantContext tenantContext,
            IUserService userService,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IClinicSelectionService clinicSelectionService) : base(tenantContext)
        {
            _userService = userService;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _clinicSelectionService = clinicSelectionService;
        }

        /// <summary>
        /// Get all users for the clinic (requires users.view permission)
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.UsersView)]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicId() ?? Guid.Empty;

            var users = await _userService.GetUsersByClinicIdAsync(clinicId, tenantId);

            return Ok(users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                Email = u.Email,
                FullName = u.FullName,
                Phone = u.Phone,
                Role = u.Role.ToString(),
                IsActive = u.IsActive,
                LastLoginAt = u.LastLoginAt,
                ProfessionalId = u.ProfessionalId,
                Specialty = u.Specialty
            }));
        }

        /// <summary>
        /// Get user by ID (requires users.view permission)
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.UsersView)]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var tenantId = GetTenantId();
            var user = await _userService.GetUserByIdAsync(id, tenantId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Phone = user.Phone,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                ProfessionalId = user.ProfessionalId,
                Specialty = user.Specialty
            });
        }

        /// <summary>
        /// Create new user (requires users.create permission)
        /// ClinicOwner can manage users in their clinic
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.UsersCreate)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicId() ?? Guid.Empty;

                // Validate subscription limits
                var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
                if (subscription == null)
                    return BadRequest(new { message = "No active subscription found" });

                var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);
                if (plan == null)
                    return BadRequest(new { message = "Invalid subscription plan" });

                var currentUserCount = await _userService.GetUserCountByClinicIdAsync(clinicId, tenantId);
                if (currentUserCount >= plan.MaxUsers)
                    return BadRequest(new { message = $"User limit reached. Current plan allows {plan.MaxUsers} users. Please upgrade your plan." });

                // Parse role
                if (!Enum.TryParse<UserRole>(request.Role, out var role))
                    return BadRequest(new { message = "Invalid role" });

                var user = await _userService.CreateUserAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.FullName,
                    request.Phone,
                    role,
                    tenantId,
                    clinicId,
                    request.ProfessionalId,
                    request.Specialty
                );

                return CreatedAtAction(nameof(GetUser), new { id = user.Id }, new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Phone = user.Phone,
                    Role = user.Role.ToString(),
                    IsActive = user.IsActive,
                    ProfessionalId = user.ProfessionalId,
                    Specialty = user.Specialty
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update user profile (requires users.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                await _userService.UpdateUserProfileAsync(
                    id,
                    request.Email,
                    request.FullName,
                    request.Phone,
                    tenantId,
                    request.ProfessionalId,
                    request.Specialty
                );

                return Ok(new { message = "User updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Change user role (requires users.edit permission)
        /// Only ClinicOwner and SystemAdmin can change user roles
        /// </summary>
        [HttpPut("{id}/role")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            try
            {
                var tenantId = GetTenantId();

                if (!Enum.TryParse<UserRole>(request.NewRole, out var newRole))
                    return BadRequest(new { message = "Invalid role" });

                await _userService.ChangeUserRoleAsync(id, newRole, tenantId);

                return Ok(new { message = "Role changed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate user (requires users.delete permission)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.UsersDelete)]
        public async Task<ActionResult> DeactivateUser(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _userService.DeactivateUserAsync(id, tenantId);
                return Ok(new { message = "User deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate user (requires users.edit permission)
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> ActivateUser(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _userService.ActivateUserAsync(id, tenantId);
                return Ok(new { message = "User activated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get available roles for user creation
        /// </summary>
        [HttpGet("roles")]
        public ActionResult<IEnumerable<string>> GetAvailableRoles()
        {
            var roles = Enum.GetNames(typeof(UserRole))
                .Where(r => r != nameof(UserRole.SystemAdmin)); // Don't allow creating system admins

            return Ok(roles);
        }

        /// <summary>
        /// Get list of clinics the current user can access
        /// </summary>
        [HttpGet("clinics")]
        public async Task<ActionResult<IEnumerable<UserClinicDto>>> GetUserClinics()
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "User ID not found in token" });
            }

            var clinics = await _clinicSelectionService.GetUserClinicsAsync(userId, tenantId);
            return Ok(clinics);
        }

        /// <summary>
        /// Get the current clinic the user is working in
        /// </summary>
        [HttpGet("current-clinic")]
        public async Task<ActionResult<UserClinicDto>> GetCurrentClinic()
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "User ID not found in token" });
            }

            var currentClinic = await _clinicSelectionService.GetCurrentClinicAsync(userId, tenantId);
            if (currentClinic == null)
            {
                return NotFound(new { message = "No current clinic set" });
            }

            return Ok(currentClinic);
        }

        /// <summary>
        /// Switch to a different clinic
        /// </summary>
        [HttpPost("select-clinic/{clinicId}")]
        public async Task<ActionResult<SwitchClinicResponse>> SelectClinic(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var userId = GetUserId();

            if (userId == Guid.Empty)
            {
                return BadRequest(new { message = "User ID not found in token" });
            }

            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, tenantId);
            
            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Link a user to a clinic - allows the user to access and work in that clinic
        /// Requires users.edit permission (typically ClinicOwner or Admin)
        /// </summary>
        [HttpPost("{userId}/clinics")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult<UserClinicLinkDto>> LinkUserToClinic(Guid userId, [FromBody] LinkUserToClinicRequest request)
        {
            try
            {
                var tenantId = GetTenantId();

                // Validate that the clinic belongs to the same tenant
                var link = await _userService.LinkUserToClinicAsync(userId, request.ClinicId, tenantId, request.IsPreferred);

                return Ok(new UserClinicLinkDto
                {
                    UserId = link.UserId,
                    ClinicId = link.ClinicId,
                    LinkedDate = link.LinkedDate,
                    IsActive = link.IsActive,
                    IsPreferredClinic = link.IsPreferredClinic
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove user's access to a clinic
        /// Requires users.edit permission (typically ClinicOwner or Admin)
        /// </summary>
        [HttpDelete("{userId}/clinics/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> RemoveUserClinicLink(Guid userId, Guid clinicId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _userService.RemoveUserClinicLinkAsync(userId, clinicId, tenantId);
                return Ok(new { message = "User clinic link removed successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Set a clinic as the user's preferred (default) clinic
        /// Requires users.edit permission (typically ClinicOwner or Admin)
        /// </summary>
        [HttpPut("{userId}/preferred-clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.UsersEdit)]
        public async Task<ActionResult> SetPreferredClinic(Guid userId, Guid clinicId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _userService.SetPreferredClinicAsync(userId, clinicId, tenantId);
                return Ok(new { message = "Preferred clinic set successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class CreateUserRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
    }

    public class UpdateUserRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
    }

    public class ChangeRoleRequest
    {
        public string NewRole { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
        public Guid? CurrentClinicId { get; set; }
    }

    public class LinkUserToClinicRequest
    {
        public Guid ClinicId { get; set; }
        public bool IsPreferred { get; set; } = false;
    }

    public class UserClinicLinkDto
    {
        public Guid UserId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime LinkedDate { get; set; }
        public bool IsActive { get; set; }
        public bool IsPreferredClinic { get; set; }
    }
}
