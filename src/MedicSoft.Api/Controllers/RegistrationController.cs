using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

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
        private readonly IClinicRepository _clinicRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IClinicSubscriptionRepository _clinicSubscriptionRepository;
        private readonly IPasswordHasher _passwordHasher;

        public RegistrationController(
            IClinicRepository clinicRepository,
            IUserRepository userRepository,
            ISubscriptionPlanRepository subscriptionPlanRepository,
            IClinicSubscriptionRepository clinicSubscriptionRepository,
            IPasswordHasher passwordHasher)
        {
            _clinicRepository = clinicRepository;
            _userRepository = userRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _clinicSubscriptionRepository = clinicSubscriptionRepository;
            _passwordHasher = passwordHasher;
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
                // Validate required fields
                if (!request.AcceptTerms)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = "You must accept the terms and conditions"
                    });
                }

                // Validate password strength
                var (isValidPassword, passwordError) = _passwordHasher.ValidatePasswordStrength(request.Password, 8);
                if (!isValidPassword)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = $"Password validation failed: {passwordError}"
                    });
                }

                // Check if CNPJ already exists
                var existingClinic = await _clinicRepository.GetByCNPJAsync(request.ClinicCNPJ);
                if (existingClinic != null)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = "CNPJ already registered"
                    });
                }

                // Check if username already exists
                var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = "Username already taken"
                    });
                }

                // Get subscription plan
                var tenantId = Guid.NewGuid().ToString();
                var plan = await _subscriptionPlanRepository.GetByIdAsync(Guid.Parse(request.PlanId), tenantId);
                if (plan == null)
                {
                    return BadRequest(new RegistrationResponseDto
                    {
                        Success = false,
                        Message = "Invalid subscription plan"
                    });
                }

                // Build full address
                var fullAddress = $"{request.Street}, {request.Number} {request.Complement}, {request.Neighborhood} - {request.City}/{request.State} - CEP: {request.ZipCode}";

                // Create clinic with default schedule (8AM to 6PM)
                var clinic = new Clinic(
                    request.ClinicName,
                    request.ClinicName, // TradeName same as Name
                    request.ClinicCNPJ,
                    request.ClinicPhone,
                    request.ClinicEmail,
                    fullAddress,
                    new TimeSpan(8, 0, 0), // 8 AM
                    new TimeSpan(18, 0, 0), // 6 PM
                    tenantId,
                    30 // 30 minute appointments
                );

                await _clinicRepository.AddAsync(clinic);

                // Hash the password
                var passwordHash = _passwordHasher.HashPassword(request.Password);

                // Create owner user
                var user = new User(
                    request.Username,
                    request.OwnerEmail,
                    passwordHash,
                    request.OwnerName,
                    request.OwnerPhone,
                    UserRole.ClinicOwner,
                    tenantId,
                    clinic.Id
                );

                await _userRepository.AddAsync(user);

                // Create subscription
                var trialDays = request.UseTrial ? plan.TrialDays : 0;
                var subscription = new ClinicSubscription(
                    clinic.Id,
                    plan.Id,
                    DateTime.UtcNow,
                    trialDays,
                    plan.MonthlyPrice,
                    tenantId
                );

                await _clinicSubscriptionRepository.AddAsync(subscription);

                var trialEndDate = request.UseTrial ? DateTime.UtcNow.AddDays(trialDays) : (DateTime?)null;

                return Ok(new RegistrationResponseDto
                {
                    Success = true,
                    Message = "Registration successful! Welcome to MedicWarehouse. You can now login with your credentials.",
                    ClinicId = clinic.Id,
                    UserId = user.Id,
                    TrialEndDate = trialEndDate
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
            var exists = await _clinicRepository.GetByCNPJAsync(cnpj);
            return Ok(new CheckCNPJResponseDto { Exists = exists != null });
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
            var exists = await _userRepository.GetByUsernameAsync(username);
            return Ok(new CheckUsernameResponseDto { Available = exists == null });
        }
    }
}
