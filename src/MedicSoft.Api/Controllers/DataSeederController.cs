using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSeederController : BaseController
    {
        private readonly DataSeederService _seederService;
        private readonly IOwnerRepository _ownerRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IWebHostEnvironment _environment;
        private readonly IConfiguration _configuration;

        public DataSeederController(
            DataSeederService seederService, 
            ITenantContext tenantContext,
            IOwnerRepository ownerRepository,
            IPasswordHasher passwordHasher,
            IWebHostEnvironment environment,
            IConfiguration configuration)
            : base(tenantContext)
        {
            _seederService = seederService;
            _ownerRepository = ownerRepository;
            _passwordHasher = passwordHasher;
            _environment = environment;
            _configuration = configuration;
        }

        /// <summary>
        /// Seed demo data for testing purposes
        /// WARNING: This endpoint should be disabled in production
        /// </summary>
        [HttpPost("seed-demo")]
        public async Task<ActionResult> SeedDemoData()
        {
            try
            {
                await _seederService.SeedDemoDataAsync();
                return Ok(new
                {
                    message = "Demo data seeded successfully",
                    tenantId = "demo-clinic-001",
                    users = new[]
                    {
                        new { username = "admin", password = "Admin@123", role = "SystemAdmin" },
                        new { username = "dr.silva", password = "Doctor@123", role = "Doctor" },
                        new { username = "recep.maria", password = "Recep@123", role = "Receptionist" }
                    },
                    note = "Use these credentials to login and test the system"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while seeding data", details = ex.Message });
            }
        }

        /// <summary>
        /// Get information about demo data
        /// </summary>
        [HttpGet("demo-info")]
        public ActionResult GetDemoInfo()
        {
            return Ok(new
            {
                tenantId = "demo-clinic-001",
                clinic = new
                {
                    name = "Clínica Demo MedicWarehouse",
                    tradeName = "Clínica Demo"
                },
                users = new[]
                {
                    new { username = "admin", role = "SystemAdmin", email = "admin@clinicademo.com.br", crm = (string?)null, specialty = (string?)null },
                    new { username = "dr.silva", role = "Doctor", email = "joao.silva@clinicademo.com.br", crm = (string?)"CRM-123456", specialty = (string?)"Clínico Geral" },
                    new { username = "recep.maria", role = "Receptionist", email = "maria.santos@clinicademo.com.br", crm = (string?)null, specialty = (string?)null }
                },
                dataSeeded = new
                {
                    patients = 6,
                    procedures = 8,
                    appointments = 5,
                    payments = 2,
                    medications = 8,
                    medicalRecords = 2,
                    prescriptionItems = 3,
                    prescriptionTemplates = 4,
                    medicalRecordTemplates = 3,
                    notifications = 5
                },
                entities = new[]
                {
                    "✅ 1 Clínica Demo",
                    "✅ 3 Usuários (Admin, Médico, Recepcionista)",
                    "✅ 6 Pacientes (incluindo 2 crianças com responsável)",
                    "✅ 8 Procedimentos diversos (consultas, exames, vacinas, etc.)",
                    "✅ 5 Agendamentos (passados, hoje e futuros)",
                    "✅ 2 Pagamentos processados",
                    "✅ 8 Medicamentos (antibióticos, analgésicos, anti-hipertensivos, etc.)",
                    "✅ 2 Prontuários médicos com consultas finalizadas",
                    "✅ 3 Itens de prescrição vinculados aos prontuários",
                    "✅ 4 Templates de prescrição (antibióticos, anti-hipertensivos, analgésicos, diabetes)",
                    "✅ 3 Templates de prontuário (clínica geral, cardiologia, pediatria)",
                    "✅ 5 Notificações (SMS, WhatsApp, Email) em diversos estados"
                },
                note = "Use POST /api/data-seeder/seed-demo to create comprehensive demo data for testing all system features"
            });
        }

        /// <summary>
        /// Seed initial system owner for development/testing
        /// Creates a default system owner if none exists
        /// WARNING: This endpoint should be disabled in production
        /// </summary>
        [HttpPost("seed-system-owner")]
        public async Task<ActionResult> SeedSystemOwner()
        {
            // Check if development mode is enabled
            var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
            
            if (!_environment.IsDevelopment() && !devModeEnabled)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new
                {
                    error = "This endpoint is only available in Development environment or when Development:EnableDevEndpoints is true"
                });
            }

            try
            {
                // Check if a system owner already exists
                var existingOwner = await _ownerRepository.GetByUsernameAsync("admin", "system");
                if (existingOwner != null)
                {
                    return Ok(new
                    {
                        message = "System owner already exists",
                        owner = new
                        {
                            username = existingOwner.Username,
                            email = existingOwner.Email,
                            isSystemOwner = existingOwner.IsSystemOwner
                        },
                        note = "Use these credentials to login",
                        loginInfo = new
                        {
                            endpoint = "POST /api/auth/owner-login",
                            username = "admin",
                            password = "Admin@123",
                            tenantId = "system"
                        }
                    });
                }

                // Create default system owner
                var passwordHash = _passwordHasher.HashPassword("Admin@123");
                var systemOwner = new Domain.Entities.Owner(
                    username: "admin",
                    email: "admin@medicwarehouse.com",
                    passwordHash: passwordHash,
                    fullName: "System Administrator",
                    phone: "+5511999999999",
                    tenantId: "system"
                );

                await _ownerRepository.AddAsync(systemOwner);

                return Ok(new
                {
                    message = "System owner created successfully",
                    owner = new
                    {
                        username = "admin",
                        email = "admin@medicwarehouse.com",
                        password = "Admin@123",
                        isSystemOwner = true,
                        tenantId = "system"
                    },
                    loginInfo = new
                    {
                        endpoint = "POST /api/auth/owner-login",
                        body = new
                        {
                            username = "admin",
                            password = "Admin@123",
                            tenantId = "system"
                        }
                    },
                    note = "Use these credentials to login and manage the system. Change the password after first login!"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while creating system owner",
                    details = ex.Message
                });
            }
        }
    }
}
