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
    /// Get current user profile
    /// </summary>
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
    /// Update user profile
    /// </summary>
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
