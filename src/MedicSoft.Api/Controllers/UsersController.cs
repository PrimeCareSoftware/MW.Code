using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.CrossCutting.Security;
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
        private readonly IUserRepository _userRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UsersController(
            ITenantContext tenantContext,
            IUserRepository userRepository,
            IClinicRepository clinicRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IPasswordHasher passwordHasher) : base(tenantContext)
        {
            _userRepository = userRepository;
            _clinicRepository = clinicRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Get all users for the clinic
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
        {
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            var users = await _userRepository.GetByClinicIdAsync(clinicId, tenantId);

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
            var user = await _userRepository.GetByIdAsync(id, tenantId);

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
            var tenantId = GetTenantId();
            var clinicId = GetClinicIdFromToken();

            // Check if username already exists
            var existingUser = await _userRepository.GetUserByUsernameAsync(request.Username, tenantId);
            if (existingUser != null)
                return BadRequest(new { message = "Username already exists" });

            // Validate subscription limits
            var subscription = await _subscriptionRepository.GetByClinicIdAsync(clinicId, tenantId);
            if (subscription == null)
                return BadRequest(new { message = "No active subscription found" });

            var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, tenantId);
            if (plan == null)
                return BadRequest(new { message = "Invalid subscription plan" });

            var currentUserCount = await _userRepository.GetUserCountByClinicIdAsync(clinicId, tenantId);
            if (currentUserCount >= plan.MaxUsers)
                return BadRequest(new { message = $"User limit reached. Current plan allows {plan.MaxUsers} users. Please upgrade your plan." });

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Parse role
            if (!Enum.TryParse<UserRole>(request.Role, out var role))
                return BadRequest(new { message = "Invalid role" });

            var user = new User(
                request.Username,
                request.Email,
                passwordHash,
                request.FullName,
                request.Phone,
                role,
                tenantId,
                clinicId,
                request.ProfessionalId,
                request.Specialty
            );

            await _userRepository.AddAsync(user);

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

        /// <summary>
        /// Update user profile
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest request)
        {
            var tenantId = GetTenantId();
            var user = await _userRepository.GetByIdAsync(id, tenantId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.UpdateProfile(
                request.Email,
                request.FullName,
                request.Phone,
                request.ProfessionalId,
                request.Specialty
            );

            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "User updated successfully" });
        }

        /// <summary>
        /// Change user role (requires ClinicOwner or SystemAdmin)
        /// Only ClinicOwner and SystemAdmin can change user roles
        /// </summary>
        [HttpPut("{id}/role")]
        [RequirePermission(Permission.ManageUsers)]
        public async Task<ActionResult> ChangeRole(Guid id, [FromBody] ChangeRoleRequest request)
        {
            var tenantId = GetTenantId();
            var user = await _userRepository.GetByIdAsync(id, tenantId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            if (!Enum.TryParse<UserRole>(request.NewRole, out var newRole))
                return BadRequest(new { message = "Invalid role" });

            user.ChangeRole(newRole);
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "Role changed successfully" });
        }

        /// <summary>
        /// Deactivate user (requires ManageUsers permission)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermission(Permission.ManageUsers)]
        public async Task<ActionResult> DeactivateUser(Guid id)
        {
            var tenantId = GetTenantId();
            var user = await _userRepository.GetByIdAsync(id, tenantId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Deactivate();
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "User deactivated successfully" });
        }

        /// <summary>
        /// Activate user (requires ManageUsers permission)
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermission(Permission.ManageUsers)]
        public async Task<ActionResult> ActivateUser(Guid id)
        {
            var tenantId = GetTenantId();
            var user = await _userRepository.GetByIdAsync(id, tenantId);

            if (user == null)
                return NotFound(new { message = "User not found" });

            user.Activate();
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "User activated successfully" });
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
