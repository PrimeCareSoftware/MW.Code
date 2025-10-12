using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Application.Services;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for clinic registration and subscription management
    /// </summary>
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        /// <summary>
        /// Register a new clinic with owner user and subscription
        /// </summary>
        /// <param name="request">Registration data</param>
        /// <returns>Registration result with clinic and user IDs</returns>
        [HttpPost]
        [ProducesResponseType(typeof(RegistrationResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<RegistrationResponseDto>> Register([FromBody] RegistrationRequestDto request)
        {
            try
            {
                var (success, message, clinicId, ownerId) = await _registrationService.RegisterClinicWithOwnerAsync(request);

                if (!success)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = message
                    });
                }

                return Ok(new RegistrationResponseDto
                {
                    Success = true,
                    Message = "Registration successful! Welcome to MedicWarehouse. You can now login with your credentials.",
                    ClinicId = clinicId,
                    UserId = ownerId
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new RegistrationResponseDto
                {
                    Success = false,
                    Message = $"Registration failed: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Check if CNPJ is already registered
        /// </summary>
        /// <param name="cnpj">CNPJ to check</param>
        /// <returns>True if CNPJ exists</returns>
        [HttpGet("check-cnpj/{cnpj}")]
        [ProducesResponseType(typeof(CheckCNPJResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckCNPJResponseDto>> CheckCNPJ(string cnpj)
        {
            var exists = await _registrationService.CheckCNPJExistsAsync(cnpj);
            return Ok(new CheckCNPJResponseDto { Exists = exists });
        }

        /// <summary>
        /// Check if username is available
        /// </summary>
        /// <param name="username">Username to check</param>
        /// <returns>True if username is available</returns>
        [HttpGet("check-username/{username}")]
        [ProducesResponseType(typeof(CheckUsernameResponseDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CheckUsernameResponseDto>> CheckUsername(string username)
        {
            // Use default tenant for registration check
            var available = await _registrationService.CheckUsernameAvailableAsync(username, "default-tenant");
            return Ok(new CheckUsernameResponseDto { Available = available });
        }
    }
}
