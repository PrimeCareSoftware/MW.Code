using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Auth.Api.Models;
using MedicSoft.Auth.Api.Services;
using MedicSoft.Shared.Authentication.Constants;

namespace MedicSoft.Auth.Api.Controllers;

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

            var tenantId = request.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    _logger.LogWarning("User login attempt without tenantId and no tenant context found");
                    return BadRequest(new { message = "TenantId is required. Please access via clinic subdomain or provide tenantId." });
                }
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

            string sessionId;
            try
            {
                sessionId = await _authService.RecordUserLoginAsync(user.Id, tenantId);
                _logger.LogInformation("User login recorded for: {UserId} with session: {SessionId}", user.Id, sessionId);
            }
            catch (Exception recordEx)
            {
                _logger.LogError(recordEx, "Failed to record user login for: {UserId}", user.Id);
                return StatusCode(500, new { message = "Failed to record login session. Please try again." });
            }

            var token = _jwtTokenService.GenerateToken(
                username: user.Username,
                userId: user.Id.ToString(),
                tenantId: tenantId,
                role: GetRoleName(user.Role),
                clinicId: user.ClinicId?.ToString(),
                sessionId: sessionId
            );

            _logger.LogInformation("JWT token generated successfully for user: {UserId}", user.Id);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = user.Username,
                TenantId = tenantId,
                Role = GetRoleName(user.Role),
                ClinicId = user.ClinicId,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
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

            var tenantId = request.TenantId;
            if (string.IsNullOrWhiteSpace(tenantId))
            {
                tenantId = HttpContext.Items["TenantId"] as string;
                if (string.IsNullOrWhiteSpace(tenantId))
                {
                    _logger.LogWarning("Owner login attempt without tenantId and no tenant context found");
                    return BadRequest(new { message = "TenantId is required. Please access via clinic subdomain or provide tenantId." });
                }
            }

            _logger.LogInformation("Owner login attempt for username: {Username}, tenantId: {TenantId}",
                request.Username, tenantId);

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

            string sessionId;
            try
            {
                sessionId = await _authService.RecordOwnerLoginAsync(owner.Id, tenantId);
                _logger.LogInformation("Owner login recorded for: {OwnerId} with session: {SessionId}", owner.Id, sessionId);
            }
            catch (Exception recordEx)
            {
                _logger.LogError(recordEx, "Failed to record owner login for: {OwnerId}", owner.Id);
                return StatusCode(500, new { message = "Failed to record login session. Please try again." });
            }

            var isSystemOwner = owner.ClinicId == null;

            var token = _jwtTokenService.GenerateToken(
                username: owner.Username,
                userId: owner.Id.ToString(),
                tenantId: tenantId,
                role: "Owner",
                clinicId: owner.ClinicId?.ToString(),
                isSystemOwner: isSystemOwner,
                sessionId: sessionId
            );

            _logger.LogInformation("JWT token generated successfully for owner: {OwnerId}", owner.Id);

            return Ok(new LoginResponse
            {
                Token = token,
                Username = owner.Username,
                TenantId = tenantId,
                Role = "Owner",
                ClinicId = owner.ClinicId,
                IsSystemOwner = isSystemOwner,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60)
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
            Role = principal.FindFirst(ClaimTypes.Role)?.Value,
            TenantId = principal.FindFirst(ClaimConstants.TenantId)?.Value
        });
    }

    /// <summary>
    /// Validate if the current session is still active
    /// </summary>
    [HttpPost("validate-session")]
    public async Task<ActionResult<SessionValidationResponse>> ValidateSession([FromBody] SessionValidationRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.Token))
            {
                return BadRequest(new { message = "Token is required" });
            }

            var principal = _jwtTokenService.ValidateToken(request.Token);
            if (principal == null)
            {
                _logger.LogWarning("ValidateSession failed: Token validation returned null");
                return Ok(new SessionValidationResponse
                {
                    IsValid = false,
                    Message = "Token inválido ou expirado"
                });
            }

            var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionIdClaim = principal.FindFirst(ClaimConstants.SessionId)?.Value;
            var tenantIdClaim = principal.FindFirst(ClaimConstants.TenantId)?.Value;
            var roleClaim = principal.FindFirst(ClaimTypes.Role)?.Value;

            if (string.IsNullOrWhiteSpace(userIdClaim) ||
                string.IsNullOrWhiteSpace(sessionIdClaim) ||
                string.IsNullOrWhiteSpace(tenantIdClaim))
            {
                return Ok(new SessionValidationResponse
                {
                    IsValid = false,
                    Message = "Token inválido"
                });
            }

            if (!Guid.TryParse(userIdClaim, out var userId))
            {
                return Ok(new SessionValidationResponse
                {
                    IsValid = false,
                    Message = "Token inválido"
                });
            }

            bool isSessionValid;

            if (roleClaim == "Owner")
            {
                isSessionValid = await _authService.ValidateOwnerSessionAsync(userId, sessionIdClaim, tenantIdClaim);
            }
            else
            {
                isSessionValid = await _authService.ValidateUserSessionAsync(userId, sessionIdClaim, tenantIdClaim);
            }

            if (!isSessionValid)
            {
                _logger.LogWarning("Session validation failed for user {UserId}. Session {SessionId} is no longer valid.",
                    userId, sessionIdClaim);

                return Ok(new SessionValidationResponse
                {
                    IsValid = false,
                    Message = "Sua sessão foi encerrada porque você fez login em outro dispositivo ou navegador."
                });
            }

            return Ok(new SessionValidationResponse
            {
                IsValid = true,
                Message = "Sessão válida"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating session");
            return StatusCode(500, new { message = "Erro ao validar sessão" });
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Auth.Microservice" });
    }

    private static string GetRoleName(int role)
    {
        return role switch
        {
            0 => "SystemAdmin",
            1 => "Doctor",
            2 => "Dentist",
            3 => "Secretary",
            4 => "Receptionist",
            5 => "Nurse",
            6 => "Technician",
            7 => "Manager",
            _ => "Unknown"
        };
    }
}
