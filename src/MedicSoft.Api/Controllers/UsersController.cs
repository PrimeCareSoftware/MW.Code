using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for user management - create, update, manage roles
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;

        public UsersController(
            ITenantContext tenantContext,
            IUserService userService,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository) : base(tenantContext)
        {
            _userService = userService;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
        }

        /// <summary>
        /// Get all users for the clinic
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

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
        /// Get user by ID
        /// </summary>
        [HttpGet("{id}")]
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
        /// Create new user (requires ClinicOwner or SystemAdmin role)
        /// ClinicOwner can manage users in their clinic
        /// </summary>
        [HttpPost]
        [RequirePermission(Permission.ManageUsers)]
        public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var clinicId = GetClinicIdFromToken();

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
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
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
        /// Change user role (requires ClinicOwner or SystemAdmin)
        /// Only ClinicOwner and SystemAdmin can change user roles
        /// </summary>
        [HttpPut("{id}/role")]
        [RequirePermission(Permission.ManageUsers)]
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
        /// Deactivate user (requires ManageUsers permission)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermission(Permission.ManageUsers)]
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
        /// Activate user (requires ManageUsers permission)
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermission(Permission.ManageUsers)]
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

        private Guid GetClinicIdFromToken()
        {
            var clinicIdClaim = User.FindFirst("clinic_id")?.Value;
            return Guid.TryParse(clinicIdClaim, out var clinicId) ? clinicId : Guid.Empty;
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
    }
}
