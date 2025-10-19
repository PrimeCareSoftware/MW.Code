using Microsoft.AspNetCore.Mvc;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Development utilities controller - ONLY for development/testing
    /// Should be disabled in production
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DevController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public DevController(
            IOwnerRepository ownerRepository,
            IPasswordHasher passwordHasher,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _ownerRepository = ownerRepository;
            _passwordHasher = passwordHasher;
            _environment = environment;
            _configuration = configuration;
        }

        /// <summary>
        /// Create initial system owner for development/testing
        /// Only works in Development environment
        /// </summary>
        [HttpPost("create-system-owner")]
        public async Task<ActionResult> CreateSystemOwner([FromBody] DevCreateSystemOwnerRequest request)
        {
            // Check if development mode is enabled
            var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
            
            if (!_environment.IsDevelopment() && !devModeEnabled)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "This endpoint is only available in Development environment or when Development:EnableDevEndpoints is true"
                });
            }

            // Validate request
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.Email))
            {
                return BadRequest(new { error = "Username, password, and email are required" });
            }

            // Check if system owner with same username already exists
            var existingOwner = await _ownerRepository.GetByUsernameAsync(request.Username, "system");
            if (existingOwner != null)
            {
                return BadRequest(new
                {
                    error = $"System owner with username '{request.Username}' already exists",
                    existingOwner = new
                    {
                        username = existingOwner.Username,
                        email = existingOwner.Email,
                        isSystemOwner = existingOwner.IsSystemOwner
                    }
                });
            }

            // Hash password
            var passwordHash = _passwordHasher.HashPassword(request.Password);

            // Create system owner
            var systemOwner = new Owner(
                username: request.Username,
                email: request.Email,
                passwordHash: passwordHash,
                fullName: request.FullName ?? "System Administrator",
                phone: request.Phone ?? "+5500000000000",
                tenantId: "system"
            );

            await _ownerRepository.AddAsync(systemOwner);

            return Ok(new
            {
                message = "System owner created successfully",
                owner = new
                {
                    id = systemOwner.Id,
                    username = systemOwner.Username,
                    email = systemOwner.Email,
                    fullName = systemOwner.FullName,
                    isSystemOwner = systemOwner.IsSystemOwner,
                    tenantId = systemOwner.TenantId
                },
                loginInstructions = new
                {
                    endpoint = "/api/auth/owner-login",
                    method = "POST",
                    body = new
                    {
                        username = request.Username,
                        password = "<your-password>",
                        tenantId = "system"
                    }
                }
            });
        }

        /// <summary>
        /// Get development environment info
        /// </summary>
        [HttpGet("info")]
        public ActionResult GetDevInfo()
        {
            var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
            var isDevelopment = _environment.IsDevelopment();

            return Ok(new
            {
                environment = _environment.EnvironmentName,
                isDevelopment = isDevelopment,
                devEndpointsEnabled = devModeEnabled || isDevelopment,
                availableEndpoints = new[]
                {
                    "POST /api/dev/create-system-owner - Create a system owner without authentication",
                    "GET /api/dev/info - Get development environment information",
                    "POST /api/data-seeder/seed-system-owner - Create default system owner (admin/Admin@123)",
                    "POST /api/registration - Create a clinic with owner (use this for clinic registration)"
                },
                note = new
                {
                    message = "These endpoints are for DEVELOPMENT/MVP only and should be disabled in production",
                    recommendation = "For creating clinics and users, use the standard /api/registration endpoint or /api/data-seeder/seed-demo for test data"
                }
            });
        }
    }

    public class DevCreateSystemOwnerRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Phone { get; set; }
    }
}
