using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Application.DTOs.Auth;
using PatientPortal.Application.Interfaces;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>
    /// Login with email or CPF
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var response = await _authService.LoginAsync(request, ipAddress);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid credentials or account is locked" });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login for {EmailOrCPF}", request.EmailOrCPF);
            return StatusCode(500, new { message = "An error occurred during login" });
        }
    }

    /// <summary>
    /// Register a new patient user
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var response = await _authService.RegisterAsync(request, ipAddress);
            
            if (response == null)
            {
                return BadRequest(new { message = "Email or CPF already registered" });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration for {Email}", request.Email);
            return StatusCode(500, new { message = "An error occurred during registration" });
        }
    }

    /// <summary>
    /// Refresh access token using refresh token
    /// </summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            var response = await _authService.RefreshTokenAsync(request.RefreshToken, ipAddress);
            
            if (response == null)
            {
                return Unauthorized(new { message = "Invalid or expired refresh token" });
            }

            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during token refresh");
            return StatusCode(500, new { message = "An error occurred during token refresh" });
        }
    }

    /// <summary>
    /// Logout and revoke refresh token
    /// </summary>
    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequestDto request)
    {
        try
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
            await _authService.RevokeTokenAsync(request.RefreshToken, ipAddress);
            return Ok(new { message = "Logged out successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during logout");
            return StatusCode(500, new { message = "An error occurred during logout" });
        }
    }

    /// <summary>
    /// Change password for authenticated user
    /// </summary>
    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto request)
    {
        try
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized();
            }

            var result = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
            
            if (!result)
            {
                return BadRequest(new { message = "Current password is incorrect" });
            }

            return Ok(new { message = "Password changed successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during password change");
            return StatusCode(500, new { message = "An error occurred during password change" });
        }
    }
}

public class ChangePasswordDto
{
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
