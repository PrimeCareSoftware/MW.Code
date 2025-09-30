using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Authenticate user and generate JWT token
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public ActionResult<AuthResponse> Login([FromBody] LoginRequest request)
        {
            // This is a simplified authentication for demo purposes
            // In a real application, you would validate against a user database
            if (IsValidUser(request.Username, request.Password))
            {
                var token = GenerateJwtToken(request.Username, request.TenantId);
                return Ok(new AuthResponse
                {
                    Token = token,
                    Username = request.Username,
                    TenantId = request.TenantId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                });
            }

            return Unauthorized("Invalid credentials");
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

        private bool IsValidUser(string username, string password)
        {
            // Demo validation - in real app, validate against database
            return !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
        }

        private string GenerateJwtToken(string username, string tenantId)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "MedicWarehouse-SuperSecretKey-2024-Development";
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim("tenant_id", tenantId),
                    new Claim("user_id", Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
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
}