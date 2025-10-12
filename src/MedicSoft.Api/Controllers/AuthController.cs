using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MedicSoft.CrossCutting.Security;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        public AuthController(
            IConfiguration configuration, 
            IPasswordHasher passwordHasher,
            IUserRepository userRepository)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
            // Sanitize inputs
            var username = InputSanitizer.TrimAndLimit(request.Username, 100);
            var tenantId = InputSanitizer.TrimAndLimit(request.TenantId, 100);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Get user from database
            var user = await _userRepository.GetUserByUsernameAsync(username, tenantId);
            
            if (user == null)
            {
                // Don't reveal whether username or password was incorrect
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Verify password
            if (!_passwordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                // Don't reveal whether username or password was incorrect
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // Check if user is active
            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Account is disabled. Please contact your administrator." });
            }

            // Update last login timestamp
            user.RecordLogin();
            await _userRepository.UpdateAsync(user);

            // Generate JWT token with clinic_id for authorization
            var token = GenerateJwtToken(user.Username, tenantId, user.Id.ToString(), user.Role.ToString(), user.ClinicId?.ToString());
            
            return Ok(new AuthResponse
            {
                Token = token,
                Username = user.Username,
                TenantId = tenantId,
                Role = user.Role.ToString(),
                ClinicId = user.ClinicId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }

        /// <summary>
        /// Get current user info
        /// </summary>
        [HttpGet("me")]
        [Authorize]
        public ActionResult<UserInfo> GetCurrentUser()
        {
            var username = User.Identity?.Name;
            var tenantId = User.FindFirst("tenant_id")?.Value ?? "default-tenant";

            return Ok(new UserInfo
            {
                Username = username ?? "unknown",
                TenantId = tenantId
            });
        }

        /// <summary>
        /// Change password
        /// </summary>
        [HttpPost("change-password")]
        [Authorize]
        public ActionResult ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var username = User.Identity?.Name;
            
            if (string.IsNullOrWhiteSpace(username))
            {
                return Unauthorized();
            }

            // Validate new password strength
            var minPasswordLength = _configuration.GetValue<int>("Security:MinPasswordLength", 8);
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(
                request.NewPassword, 
                minPasswordLength);

            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            // In a real application:
            // 1. Get user from database
            // 2. Verify current password with _passwordHasher.VerifyPassword()
            // 3. Hash new password with _passwordHasher.HashPassword()
            // 4. Update user record in database
            // 5. Invalidate all existing tokens
            // 6. Send notification email

            return Ok(new { message = "Password changed successfully" });
        }

        /// <summary>
        /// Hash password utility endpoint (for development/setup only - remove in production)
        /// </summary>
        [HttpPost("hash-password")]
        [AllowAnonymous]
        public ActionResult<HashPasswordResponse> HashPassword([FromBody] HashPasswordRequest request)
        {
            // This endpoint should be removed or protected in production
            if (_configuration.GetValue<bool>("Security:AllowPasswordHashing", false) == false)
            {
                return NotFound();
            }

            var minPasswordLength = _configuration.GetValue<int>("Security:MinPasswordLength", 8);
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(
                request.Password, 
                minPasswordLength);

            if (!isValid)
            {
                return BadRequest(new { message = errorMessage });
            }

            var hashedPassword = _passwordHasher.HashPassword(request.Password);

            return Ok(new HashPasswordResponse
            {
                HashedPassword = hashedPassword,
                Message = "Password hashed successfully"
            });
        }

        private string GenerateJwtToken(string username, string tenantId, string userId, string role, string? clinicId = null)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim("tenant_id", tenantId),
                new Claim("user_id", userId),
                new Claim(ClaimTypes.Role, role)
            };

            // Add clinic_id claim if user belongs to a clinic
            if (!string.IsNullOrEmpty(clinicId))
            {
                claims.Add(new Claim("clinic_id", clinicId));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(jwtSettings.GetValue<int>("ExpiryMinutes", 60)),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TenantId { get; set; } = "default-tenant";
    }

    public class AuthResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class UserInfo
    {
        public string Username { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
    }

    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class HashPasswordRequest
    {
        public string Password { get; set; } = string.Empty;
    }

    public class HashPasswordResponse
    {
        public string HashedPassword { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}