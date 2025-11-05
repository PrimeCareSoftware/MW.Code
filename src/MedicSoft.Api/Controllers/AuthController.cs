using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            IAuthService authService, 
            IJwtTokenService jwtTokenService,
            ILogger<AuthController> logger)
        {
            _authService = authService;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        /// <summary>
        /// Login endpoint for regular users (doctors, secretaries, etc.)
        /// </summary>
        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("User login attempt with null request body");
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("User login attempt with missing credentials. Username: {Username}", 
                        request.Username ?? "null");
                    return BadRequest(new { message = "Username and password are required" });
                }

                // Get tenantId from request or from middleware context
                var tenantId = request.TenantId;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    tenantId = HttpContext.Items["TenantId"] as string;
                    if (string.IsNullOrWhiteSpace(tenantId))
                    {
                        _logger.LogWarning("User login attempt without tenantId and no tenant context found");
                        return BadRequest(new { message = "TenantId is required. Please access via clinic subdomain or provide tenantId." });
                    }
                    _logger.LogInformation("Using tenantId from context: {TenantId}", tenantId);
                }

                _logger.LogInformation("User login attempt for username: {Username}, tenantId: {TenantId}", 
                    request.Username, tenantId);

                var user = await _authService.AuthenticateUserAsync(
                    request.Username, 
                    request.Password, 
                    tenantId
                );

                if (user == null)
                {
                    _logger.LogWarning("Failed user login attempt for username: {Username}, tenantId: {TenantId}", 
                        request.Username, tenantId);
                    return Unauthorized(new { message = "Invalid credentials or user not found" });
                }

                _logger.LogInformation("User authenticated successfully: {UserId}, username: {Username}", 
                    user.Id, user.Username);

                // Record login
                try
                {
                    await _authService.RecordUserLoginAsync(user.Id, tenantId);
                    _logger.LogInformation("User login recorded for: {UserId}", user.Id);
                }
                catch (Exception recordEx)
                {
                    // Log but don't fail the login if recording fails
                    _logger.LogError(recordEx, "Failed to record user login for: {UserId}", user.Id);
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(
                    username: user.Username,
                    userId: user.Id.ToString(),
                    tenantId: tenantId,
                    role: user.Role.ToString(),
                    clinicId: user.ClinicId?.ToString()
                );

                _logger.LogInformation("JWT token generated successfully for user: {UserId}", user.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = user.Username,
                    TenantId = tenantId,
                    Role = user.Role.ToString(),
                    ClinicId = user.ClinicId,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during user login for username: {Username}, tenantId: {TenantId}", 
                    request?.Username ?? "unknown", request?.TenantId ?? "unknown");
                return StatusCode(500, new { message = "An error occurred during login. Please try again later." });
            }
        }

        /// <summary>
        /// Login endpoint for owners (clinic owners and system owners)
        /// </summary>
        [HttpPost("owner-login")]
        public async Task<ActionResult<LoginResponse>> OwnerLogin([FromBody] LoginRequest request)
        {
            try
            {
                // Validate request
                if (request == null)
                {
                    _logger.LogWarning("Owner login attempt with null request body");
                    return BadRequest(new { message = "Request body is required" });
                }

                if (string.IsNullOrWhiteSpace(request.Username) || 
                    string.IsNullOrWhiteSpace(request.Password))
                {
                    _logger.LogWarning("Owner login attempt with missing credentials. Username: {Username}", 
                        request.Username ?? "null");
                    return BadRequest(new { message = "Username and password are required" });
                }

                // Get tenantId from request or from middleware context
                var tenantId = request.TenantId;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    tenantId = HttpContext.Items["TenantId"] as string;
                    if (string.IsNullOrWhiteSpace(tenantId))
                    {
                        _logger.LogWarning("Owner login attempt without tenantId and no tenant context found");
                        return BadRequest(new { message = "TenantId is required. Please access via clinic subdomain or provide tenantId." });
                    }
                    _logger.LogInformation("Using tenantId from context: {TenantId}", tenantId);
                }

                _logger.LogInformation("Owner login attempt for username: {Username}, tenantId: {TenantId}", 
                    request.Username, tenantId);

                // Authenticate owner
                var owner = await _authService.AuthenticateOwnerAsync(
                    request.Username, 
                    request.Password, 
                    tenantId
                );

                if (owner == null)
                {
                    _logger.LogWarning("Failed owner login attempt for username: {Username}, tenantId: {TenantId}", 
                        request.Username, tenantId);
                    return Unauthorized(new { message = "Invalid credentials or owner not found" });
                }

                _logger.LogInformation("Owner authenticated successfully: {OwnerId}, username: {Username}", 
                    owner.Id, owner.Username);

                // Record login
                try
                {
                    await _authService.RecordOwnerLoginAsync(owner.Id, tenantId);
                    _logger.LogInformation("Owner login recorded for: {OwnerId}", owner.Id);
                }
                catch (Exception recordEx)
                {
                    // Log but don't fail the login if recording fails
                    _logger.LogError(recordEx, "Failed to record owner login for: {OwnerId}", owner.Id);
                }

                // Generate JWT token
                var token = _jwtTokenService.GenerateToken(
                    username: owner.Username,
                    userId: owner.Id.ToString(),
                    tenantId: tenantId,
                    role: "Owner",
                    clinicId: owner.ClinicId?.ToString(),
                    isSystemOwner: owner.IsSystemOwner
                );

                _logger.LogInformation("JWT token generated successfully for owner: {OwnerId}", owner.Id);

                return Ok(new LoginResponse
                {
                    Token = token,
                    Username = owner.Username,
                    TenantId = tenantId,
                    Role = "Owner",
                    ClinicId = owner.ClinicId,
                    IsSystemOwner = owner.IsSystemOwner,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60) // Should match JWT expiry
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during owner login for username: {Username}, tenantId: {TenantId}", 
                    request?.Username ?? "unknown", request?.TenantId ?? "unknown");
                return StatusCode(500, new { message = "An error occurred during login. Please try again later." });
            }
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
        public string? TenantId { get; set; } // Optional - can be resolved from subdomain
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
