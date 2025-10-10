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

        public AuthController(IConfiguration configuration, IPasswordHasher passwordHasher)
        {
            _configuration = configuration;
            _passwordHasher = passwordHasher;
        }

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
        {
            // Sanitize inputs
            var username = InputSanitizer.TrimAndLimit(request.Username, 100);
            var tenantId = InputSanitizer.TrimAndLimit(request.TenantId, 100);

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(request.Password))
            {
                return Unauthorized(new { message = "Invalid credentials" });
            }

            // In a real application, you would:
            // 1. Query user from database by username
            // 2. Verify password hash using _passwordHasher.VerifyPassword()
            // 3. Check if account is locked
            // 4. Update last login timestamp
            // 5. Log the login attempt

            // Simplified validation for demo
            if (IsValidUser(username, request.Password))
            {
                var token = GenerateJwtToken(username, tenantId);
                return Ok(new AuthResponse
                {
                    Token = token,
                    Username = username,
                    TenantId = tenantId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }

            // Important: Don't reveal whether username or password was incorrect
            return Unauthorized(new { message = "Invalid credentials" });
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

        private bool IsValidUser(string username, string password)
        {
            // Demo validation - in real app, validate against database with hashed passwords
            // Example:
            // var user = await _userRepository.GetByUsernameAsync(username);
            // if (user == null) return false;
            // return _passwordHasher.VerifyPassword(password, user.PasswordHash);
            
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }

        private string GenerateJwtToken(string username, string tenantId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey) || secretKey.Length < 32)
            {
                throw new InvalidOperationException("JWT SecretKey must be at least 32 characters long");
            }

            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("tenant_id", tenantId),
                    new Claim("user_id", Guid.NewGuid().ToString())
                }),
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