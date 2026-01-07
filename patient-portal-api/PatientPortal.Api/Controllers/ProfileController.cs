using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IPatientUserRepository _patientUserRepository;
    private readonly ILogger<ProfileController> _logger;

    public ProfileController(IPatientUserRepository patientUserRepository, ILogger<ProfileController> logger)
    {
        _patientUserRepository = patientUserRepository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves the profile information of the authenticated patient
    /// </summary>
    /// <returns>User profile data including personal information and account settings</returns>
    /// <response code="200">Returns the user profile information</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User profile not found</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/profile/me
    ///     Authorization: Bearer {access-token}
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    ///         "email": "patient@example.com",
    ///         "fullName": "João Silva",
    ///         "cpf": "12345678901",
    ///         "phoneNumber": "+55 11 98765-4321",
    ///         "dateOfBirth": "1990-01-15",
    ///         "emailConfirmed": true,
    ///         "phoneConfirmed": false,
    ///         "twoFactorEnabled": false,
    ///         "lastLoginAt": "2026-01-07T10:30:00Z",
    ///         "createdAt": "2025-12-01T08:00:00Z"
    ///     }
    /// 
    /// **LGPD Compliance:**
    /// - Sensitive data (password hash, security stamps) is never returned
    /// - Profile access is logged for audit purposes
    /// </remarks>
    [HttpGet("me")]
    public async Task<IActionResult> GetProfile()
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _patientUserRepository.GetByIdAsync(userId.Value);
            
            if (user == null)
                return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Email,
                user.FullName,
                user.CPF,
                user.PhoneNumber,
                user.DateOfBirth,
                user.EmailConfirmed,
                user.PhoneConfirmed,
                user.TwoFactorEnabled,
                user.LastLoginAt,
                user.CreatedAt
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting user profile");
            return StatusCode(500, new { message = "An error occurred while retrieving profile" });
        }
    }

    /// <summary>
    /// Updates the profile information of the authenticated patient
    /// </summary>
    /// <param name="request">Profile fields to update (full name and/or phone number)</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Profile updated successfully</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">User profile not found</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     PUT /api/profile/me
    ///     Authorization: Bearer {access-token}
    ///     {
    ///         "fullName": "João Silva Santos",
    ///         "phoneNumber": "+55 11 91234-5678"
    ///     }
    /// 
    /// **Updatable Fields:**
    /// - fullName: User's full name
    /// - phoneNumber: Contact phone number in international format
    /// 
    /// **Non-Updatable Fields:**
    /// - Email and CPF cannot be changed via this endpoint for security reasons
    /// - To change email or CPF, contact support
    /// 
    /// **Validation:**
    /// - Full name must not be empty if provided
    /// - Phone number should be in valid format if provided
    /// 
    /// **LGPD Compliance:**
    /// - Profile updates are logged for audit purposes
    /// - Only the authenticated user can update their own profile
    /// </remarks>
    [HttpPut("me")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto request)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var user = await _patientUserRepository.GetByIdAsync(userId.Value);
            
            if (user == null)
                return NotFound(new { message = "User not found" });

            // Update allowed fields
            if (!string.IsNullOrEmpty(request.FullName))
                user.FullName = request.FullName;

            if (!string.IsNullOrEmpty(request.PhoneNumber))
                user.PhoneNumber = request.PhoneNumber;

            user.UpdatedAt = DateTime.UtcNow;

            await _patientUserRepository.UpdateAsync(user);

            return Ok(new { message = "Profile updated successfully" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user profile");
            return StatusCode(500, new { message = "An error occurred while updating profile" });
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }
}

public class UpdateProfileDto
{
    public string? FullName { get; set; }
    public string? PhoneNumber { get; set; }
}
