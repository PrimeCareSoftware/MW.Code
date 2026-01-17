using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Registration;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Local development controller for quick registration without authentication
    /// ONLY for local development - disabled in production
    /// </summary>
    [ApiController]
    [Route("api/local-dev")]
    [AllowAnonymous]
    public class LocalDevController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;
        private readonly IUserRepository _userRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public LocalDevController(
            IRegistrationService registrationService,
            IUserRepository userRepository,
            IClinicRepository clinicRepository,
            IPasswordHasher passwordHasher,
            IWebHostEnvironment environment,
            IConfiguration configuration)
        {
            _registrationService = registrationService;
            _userRepository = userRepository;
            _clinicRepository = clinicRepository;
            _passwordHasher = passwordHasher;
            _environment = environment;
            _configuration = configuration;
        }

        /// <summary>
        /// Quick registration: Creates Clinic + Owner + System Admin User
        /// Only works in Development environment
        /// </summary>
        [HttpPost("quick-register")]
        public async Task<ActionResult> QuickRegister([FromBody] LocalDevQuickRegisterRequest request)
        {
            // Check if development mode is enabled
            var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
            
            if (!_environment.IsDevelopment() && !devModeEnabled)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "This endpoint is only available in Development environment"
                });
            }

            try
            {
                // Use default values if not provided
                var clinicName = string.IsNullOrWhiteSpace(request.ClinicName) ? "Clínica Teste Local" : request.ClinicName;
                var ownerUsername = string.IsNullOrWhiteSpace(request.OwnerUsername) ? "owner" : request.OwnerUsername;
                var ownerPassword = string.IsNullOrWhiteSpace(request.OwnerPassword) ? "Owner@123" : request.OwnerPassword;
                var ownerEmail = string.IsNullOrWhiteSpace(request.OwnerEmail) ? "owner@teste.local" : request.OwnerEmail;
                var adminUsername = string.IsNullOrWhiteSpace(request.AdminUsername) ? "admin" : request.AdminUsername;
                var adminPassword = string.IsNullOrWhiteSpace(request.AdminPassword) ? "Admin@123" : request.AdminPassword;
                var adminEmail = string.IsNullOrWhiteSpace(request.AdminEmail) ? "admin@teste.local" : request.AdminEmail;

                // Get a subscription plan (use Trial plan if exists)
                var planRepository = HttpContext.RequestServices.GetRequiredService<ISubscriptionPlanRepository>();
                var plans = await planRepository.GetActiveInPlansAsync();
                var trialPlan = plans.FirstOrDefault(p => p.Type == SubscriptionPlanType.Trial) 
                    ?? plans.FirstOrDefault();
                
                if (trialPlan == null)
                {
                    return BadRequest(new { error = "No subscription plans available. Please seed data first." });
                }

                // Create registration request
                var registrationRequest = new RegistrationRequestDto
                {
                    ClinicName = clinicName,
                    ClinicCNPJ = GenerateRandomCNPJ(),
                    ClinicPhone = "+5511999999999",
                    ClinicEmail = $"contato@{clinicName.ToLower().Replace(" ", "")}.local",
                    Street = "Rua Teste",
                    Number = "123",
                    Neighborhood = "Centro",
                    City = "São Paulo",
                    State = "SP",
                    ZipCode = "01000-000",
                    OwnerName = request.OwnerName ?? "Proprietário Teste",
                    OwnerCPF = GenerateRandomCPF(),
                    OwnerPhone = "+5511988888888",
                    OwnerEmail = ownerEmail,
                    Username = ownerUsername,
                    Password = ownerPassword,
                    PlanId = trialPlan.Id.ToString(),
                    AcceptTerms = true,
                    UseTrial = true
                };

                // Register clinic with owner
                var result = await _registrationService.RegisterClinicWithOwnerAsync(registrationRequest);

                if (!result.Success)
                {
                    return BadRequest(new { error = result.Message });
                }

                // Create system admin user for the clinic
                var adminPasswordHash = _passwordHasher.HashPassword(adminPassword);
                var clinic = await _clinicRepository.GetByIdAsync(result.ClinicId!.Value, result.TenantId!);
                
                var adminUser = new User(
                    username: adminUsername,
                    email: adminEmail,
                    passwordHash: adminPasswordHash,
                    fullName: request.AdminName ?? "Administrador Teste",
                    phone: "+5511977777777",
                    role: UserRole.SystemAdmin,
                    tenantId: result.TenantId!,
                    clinicId: clinic!.Id
                );

                await _userRepository.AddAsync(adminUser);

                return Ok(new
                {
                    message = "Quick registration completed successfully!",
                    clinic = new
                    {
                        id = result.ClinicId,
                        name = result.ClinicName,
                        tenantId = result.TenantId
                    },
                    owner = new
                    {
                        id = result.OwnerId,
                        username = ownerUsername,
                        password = ownerPassword,
                        email = ownerEmail,
                        loginEndpoint = "/api/auth/owner-login"
                    },
                    systemAdmin = new
                    {
                        id = adminUser.Id,
                        username = adminUsername,
                        password = adminPassword,
                        email = adminEmail,
                        loginEndpoint = "/api/auth/login"
                    },
                    note = "Use the credentials above to login and test the system"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred during quick registration",
                    details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get local dev info and instructions
        /// </summary>
        [HttpGet("info")]
        public ActionResult GetInfo()
        {
            var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
            var isDevelopment = _environment.IsDevelopment();

            if (!isDevelopment && !devModeEnabled)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "This endpoint is only available in Development environment"
                });
            }

            return Ok(new
            {
                title = "Local Development Quick Registration",
                description = "This endpoint allows you to quickly create a complete registration (Clinic + Owner + System Admin) for testing purposes",
                endpoints = new
                {
                    quickRegister = new
                    {
                        method = "POST",
                        path = "/api/local-dev/quick-register",
                        description = "Create a complete registration with default or custom values",
                        exampleBody = new
                        {
                            clinicName = "Minha Clínica",
                            ownerUsername = "owner",
                            ownerPassword = "Owner@123",
                            ownerEmail = "owner@teste.local",
                            ownerName = "Dr. João Silva",
                            adminUsername = "admin",
                            adminPassword = "Admin@123",
                            adminEmail = "admin@teste.local",
                            adminName = "Administrador"
                        },
                        note = "All fields are optional. If not provided, default values will be used."
                    },
                    webInterface = new
                    {
                        path = "/local-dev-registration.html",
                        description = "Simple web interface for quick registration"
                    }
                },
                warning = "⚠️ This endpoint is for LOCAL DEVELOPMENT ONLY and will not work in production"
            });
        }

        private static string GenerateRandomCNPJ()
        {
            // Generate the first 12 digits randomly
            var cnpj = "";
            for (int i = 0; i < 12; i++)
            {
                cnpj += Random.Shared.Next(0, 10).ToString();
            }

            // Calculate the first check digit
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += (cnpj[i] - '0') * multiplier1[i];
            }
            var remainder = sum % 11;
            var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;
            cnpj += firstCheckDigit.ToString();

            // Calculate the second check digit
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            for (int i = 0; i < 13; i++)
            {
                sum += (cnpj[i] - '0') * multiplier2[i];
            }
            remainder = sum % 11;
            var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;
            cnpj += secondCheckDigit.ToString();

            return cnpj;
        }

        private static string GenerateRandomCPF()
        {
            // Generate the first 9 digits randomly
            var cpf = "";
            for (int i = 0; i < 9; i++)
            {
                cpf += Random.Shared.Next(0, 10).ToString();
            }

            // Calculate the first check digit
            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += (cpf[i] - '0') * (10 - i);
            }
            var remainder = sum % 11;
            var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;
            cpf += firstCheckDigit.ToString();

            // Calculate the second check digit
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += (cpf[i] - '0') * (11 - i);
            }
            remainder = sum % 11;
            var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;
            cpf += secondCheckDigit.ToString();

            return cpf;
        }
    }

    public class LocalDevQuickRegisterRequest
    {
        public string? ClinicName { get; set; }
        public string? OwnerUsername { get; set; }
        public string? OwnerPassword { get; set; }
        public string? OwnerEmail { get; set; }
        public string? OwnerName { get; set; }
        public string? AdminUsername { get; set; }
        public string? AdminPassword { get; set; }
        public string? AdminEmail { get; set; }
        public string? AdminName { get; set; }
    }
}
