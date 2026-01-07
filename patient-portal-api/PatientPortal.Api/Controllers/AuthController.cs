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
    /// Authenticates a patient user with email or CPF
    /// </summary>
    /// <param name="request">Login credentials containing email/CPF and password</param>
    /// <returns>JWT access token and refresh token if authentication succeeds</returns>
    /// <response code="200">Login successful - returns access token, refresh token, and user information</response>
    /// <response code="401">Invalid credentials or account is locked after 5 failed attempts</response>
    /// <response code="500">Internal server error occurred during authentication</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "emailOrCPF": "patient@example.com",
    ///         "password": "Password123!"
    ///     }
    /// 
    /// OR using CPF:
    /// 
    ///     POST /api/auth/login
    ///     {
    ///         "emailOrCPF": "12345678901",
    ///         "password": "Password123!"
    ///     }
    /// 
    /// **Security Notes:**
    /// - Account will be locked for 15 minutes after 5 failed login attempts
    /// - Access tokens expire after 15 minutes
    /// - Refresh tokens expire after 7 days
    /// - All login attempts are logged with IP address
    /// </remarks>
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
    /// Registers a new patient user account
    /// </summary>
    /// <param name="request">Registration information including email, CPF, password, and personal details</param>
    /// <returns>JWT tokens and user information if registration succeeds</returns>
    /// <response code="200">Registration successful - returns access token and refresh token</response>
    /// <response code="400">Email or CPF already registered, or invalid data provided</response>
    /// <response code="500">Internal server error occurred during registration</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/register
    ///     {
    ///         "email": "patient@example.com",
    ///         "cpf": "12345678901",
    ///         "fullName": "João Silva",
    ///         "password": "Password123!",
    ///         "phoneNumber": "+55 11 98765-4321",
    ///         "dateOfBirth": "1990-01-15",
    ///         "patientId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "clinicId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    ///     }
    /// 
    /// **Requirements:**
    /// - Email must be unique and in valid format
    /// - CPF must be unique and contain 11 digits
    /// - Password must be at least 8 characters
    /// - Full name is required
    /// - Phone number should be in international format
    /// - Patient ID must match an existing patient in the main system
    /// - Clinic ID identifies which clinic the patient belongs to
    /// 
    /// **LGPD Compliance:**
    /// - Password is hashed using PBKDF2 with 100,000 iterations
    /// - Personal data is encrypted at rest
    /// - Registration is logged for audit purposes
    /// </remarks>
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
    /// Refreshes an expired access token using a valid refresh token
    /// </summary>
    /// <param name="request">Refresh token request containing the refresh token</param>
    /// <returns>New access token and refresh token pair</returns>
    /// <response code="200">Token refresh successful - returns new token pair</response>
    /// <response code="401">Refresh token is invalid, expired, or has been revoked</response>
    /// <response code="500">Internal server error occurred during token refresh</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/refresh
    ///     {
    ///         "refreshToken": "your-refresh-token-here"
    ///     }
    /// 
    /// **Token Lifecycle:**
    /// - Access tokens expire after 15 minutes
    /// - Refresh tokens expire after 7 days
    /// - Each refresh generates a new token pair
    /// - Old refresh tokens are automatically revoked
    /// - Tokens can only be used once (one-time use)
    /// 
    /// **Security:**
    /// - Refresh tokens are stored hashed in the database
    /// - Used tokens are marked as revoked
    /// - All refresh attempts are logged with IP address
    /// </remarks>
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
    /// Logs out the authenticated user by revoking the refresh token
    /// </summary>
    /// <param name="request">Refresh token to revoke</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Logout successful - refresh token has been revoked</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error occurred during logout</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/logout
    ///     Authorization: Bearer {access-token}
    ///     {
    ///         "refreshToken": "your-refresh-token-here"
    ///     }
    /// 
    /// **Notes:**
    /// - Requires valid access token in Authorization header
    /// - Revokes the specified refresh token
    /// - Access token remains valid until expiry (15 minutes)
    /// - Client should discard both tokens after logout
    /// - Logout event is logged for security audit
    /// </remarks>
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
    /// Changes the password for the authenticated user
    /// </summary>
    /// <param name="request">Current password and new password</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Password changed successfully</response>
    /// <response code="400">Current password is incorrect</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error occurred during password change</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/auth/change-password
    ///     Authorization: Bearer {access-token}
    ///     {
    ///         "currentPassword": "OldPassword123!",
    ///         "newPassword": "NewPassword456!"
    ///     }
    /// 
    /// **Password Requirements:**
    /// - Minimum 8 characters
    /// - Should contain uppercase and lowercase letters
    /// - Should contain numbers
    /// - Should contain special characters
    /// 
    /// **Security:**
    /// - Current password is verified before change
    /// - New password is hashed with PBKDF2 (100,000 iterations)
    /// - Password change event is logged
    /// - All active refresh tokens remain valid
    /// </remarks>
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
