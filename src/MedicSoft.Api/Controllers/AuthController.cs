using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Authentication controller for user and owner login
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthController(IAuthService authService, IJwtTokenService jwtTokenService)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
        }

        /// <summary>
        /// Login endpoint for regular users (doctors, secretaries, etc.)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.TenantId))
            {
                return BadRequest(new { message = "Username, password, and tenantId are required" });
            }

            var user = await _authService.AuthenticateUserAsync(
                request.Username, 
                request.Password, 
                request.TenantId
            );

            if (user == null)
            {
                return Unauthorized(new { message = "Invalid credentials or user not found" });
            }

            // Record login
            await _authService.RecordUserLoginAsync(user.Id, request.TenantId);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(
                username: user.Username,
                userId: user.Id.ToString(),
                tenantId: request.TenantId,
                role: user.Role.ToString(),
                clinicId: user.ClinicId?.ToString()
            );

            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                TenantId = request.TenantId,
                Role = user.Role.ToString(),
                ClinicId = user.ClinicId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
            });
        }

        /// <summary>
        /// Login endpoint for owners (clinic owners and system owners)
        /// </summary>
        [HttpPost("owner-login")]
        public async Task<ActionResult<LoginResponse>> OwnerLogin([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || 
                string.IsNullOrWhiteSpace(request.Password) ||
                string.IsNullOrWhiteSpace(request.TenantId))
            {
                return BadRequest(new { message = "Username, password, and tenantId are required" });
            }

            var owner = await _authService.AuthenticateOwnerAsync(
                request.Username, 
                request.Password, 
                request.TenantId
            );

            if (owner == null)
            {
                return Unauthorized(new { message = "Invalid credentials or owner not found" });
            }

            // Record login
            await _authService.RecordOwnerLoginAsync(owner.Id, request.TenantId);

            // Generate JWT token
            var token = _jwtTokenService.GenerateToken(
                username: owner.Username,
                userId: owner.Id.ToString(),
                tenantId: request.TenantId,
                role: "Owner",
                clinicId: owner.ClinicId?.ToString(),
                isSystemOwner: owner.IsSystemOwner
            );

            return Ok(new LoginResponse
            {
                Token = token,
                Username = owner.Username,
                TenantId = request.TenantId,
                Role = "Owner",
                ClinicId = owner.ClinicId,
                IsSystemOwner = owner.IsSystemOwner,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
            });
        }

        /// <summary>
        /// Validate if a token is still valid
        /// </summary>
        [HttpPost("validate")]
        public ActionResult<TokenValidationResponse> ValidateToken([FromBody] TokenValidationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var principal = _jwtTokenService.ValidateToken(request.Token);

            if (principal == null)
            {
                return Ok(new TokenValidationResponse { IsValid = false });
            }

            return Ok(new TokenValidationResponse 
            { 
                IsValid = true,
                Username = principal.Identity?.Name,
                Role = principal.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value,
                TenantId = principal.FindFirst("tenant_id")?.Value
            });
        }
    }

    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public bool IsSystemOwner { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class TokenValidationRequest
    {
        public string Token { get; set; } = string.Empty;
    }

    public class TokenValidationResponse
    {
        public bool IsValid { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? TenantId { get; set; }
    }
}
