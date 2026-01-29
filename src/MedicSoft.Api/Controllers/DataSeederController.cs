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
                    systemAdmin = new
                    {
                        note = "System Owner for accessing system-admin area (NOT the same as the clinic 'admin' user below)",
                        endpoint = "POST /api/auth/owner-login",
                        username = "admin",
                        password = "Admin@123",
                        tenantId = "system",
                        isSystemOwner = true
                    },
                    credentials = new
                    {
                        owner = new { username = "owner.demo", password = "Owner@123", role = "Owner" },
                        users = new[]
                        {
                            new { username = "admin", password = "Admin@123", role = "SystemAdmin", note = "Clinic user (different from system owner above)" },
                            new { username = "dr.silva", password = "Doctor@123", role = "Doctor" },
                            new { username = "recep.maria", password = "Recep@123", role = "Receptionist" }
                        }
                    },
                    summary = new
                    {
                        subscriptionPlans = 5,
                        systemOwner = 1,
                        clinic = 1,
                        clinicSubscription = 1,
                        owner = 1,
                        users = 3,
                        patients = 6,
                        procedures = 8,
                        appointments = 5,
                        payments = 2,
                        medications = 8,
                        medicalRecords = 2,
                        prescriptionItems = 3,
                        prescriptionTemplates = 4,
                        medicalRecordTemplates = 3,
                        notifications = 5,
                        notificationRoutines = 5,
                        expenses = 10,
                        examRequests = 5,
                        digitalPrescriptions = 2,
                        healthInsuranceOperators = 3,
                        healthInsurancePlans = 3,
                        invoices = 2
                    },
                    note = "Use these credentials to login and test the system. Complete database seeded with realistic demo data including CFM/ANVISA compliant digital prescriptions. System owner created for system-admin access."
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
                systemAdmin = new
                {
                    note = "System Owner for accessing system-admin area (NOT the same as the clinic 'admin' user below)",
                    endpoint = "POST /api/auth/owner-login",
                    username = "admin",
                    password = "Admin@123",
                    tenantId = "system",
                    isSystemOwner = true
                },
                clinic = new
                {
                    name = "Clínica Demo PrimeCare Software",
                    tradeName = "Clínica Demo"
                },
                users = new[]
                {
                    new { username = "owner.demo", role = "Owner", email = "owner@clinicademo.com.br", crm = (string?)null, specialty = (string?)null },
                    new { username = "admin", role = "SystemAdmin", email = "admin@clinicademo.com.br", crm = (string?)null, specialty = (string?)null },
                    new { username = "dr.silva", role = "Doctor", email = "joao.silva@clinicademo.com.br", crm = (string?)"CRM-123456", specialty = (string?)"Clínico Geral" },
                    new { username = "recep.maria", role = "Receptionist", email = "maria.santos@clinicademo.com.br", crm = (string?)null, specialty = (string?)null }
                },
                dataSeeded = new
                {
                    subscriptionPlans = 5,
                    systemOwner = 1,
                    clinic = 1,
                    clinicSubscription = 1,
                    owner = 1,
                    users = 3,
                    patients = 6,
                    procedures = 8,
                    appointments = 5,
                    appointmentProcedures = 3,
                    payments = 2,
                    medications = 8,
                    medicalRecords = 2,
                    prescriptionItems = 3,
                    prescriptionTemplates = 4,
                    medicalRecordTemplates = 3,
                    notifications = 5,
                    notificationRoutines = 5,
                    expenses = 10,
                    examRequests = 5,
                    digitalPrescriptions = 2,
                    healthInsuranceOperators = 3,
                    healthInsurancePlans = 3,
                    invoices = 2
                },
                entities = new[]
                {
                    "✅ 1 System Owner (admin) for system-admin access",
                    "✅ 5 Planos de assinatura (Trial, Básico, Standard, Premium, Enterprise)",
                    "✅ 1 Clínica Demo",
                    "✅ 1 Assinatura ativa (Plano Standard)",
                    "✅ 1 Proprietário da clínica (Owner)",
                    "✅ 3 Usuários (Admin, Médico, Recepcionista)",
                    "✅ 6 Pacientes com nome da mãe (CFM 1.821) incluindo 2 crianças com responsável",
                    "✅ 8 Procedimentos diversos (consultas, exames, vacinas, etc.)",
                    "✅ 5 Agendamentos (passados, hoje e futuros)",
                    "✅ 3 Procedimentos vinculados a agendamentos",
                    "✅ 2 Pagamentos processados",
                    "✅ 8 Medicamentos (antibióticos, analgésicos, anti-hipertensivos, etc.)",
                    "✅ 2 Prontuários médicos com consultas finalizadas",
                    "✅ 3 Itens de prescrição vinculados aos prontuários",
                    "✅ 2 Prescrições digitais assinadas (CFM 1.643/2002 e ANVISA 344/1998)",
                    "✅ 4 Templates de prescrição (antibióticos, anti-hipertensivos, analgésicos, diabetes)",
                    "✅ 3 Templates de prontuário (clínica geral, cardiologia, pediatria)",
                    "✅ 5 Notificações (SMS, WhatsApp, Email) em diversos estados",
                    "✅ 5 Rotinas de notificação automatizadas",
                    "✅ 10 Despesas (pagas, pendentes, vencidas e canceladas)",
                    "✅ 5 Solicitações de exames (laboratoriais, imagem, cardiológicos)",
                    "✅ 3 Operadoras de plano de saúde (Unimed, Bradesco, SulAmérica)",
                    "✅ 3 Planos de saúde ativos para pacientes",
                    "✅ 2 Notas fiscais (emitidas e pagas)"
                },
                note = "Use POST /api/data-seeder/seed-demo to create comprehensive demo data for testing all system features with full CFM/ANVISA compliance. System owner created for system-admin access."
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

        /// <summary>
        /// Clear all data from the database
        /// WARNING: This will delete all data from all tables. Use with caution!
        /// This endpoint should only be available in Development environment
        /// </summary>
        [HttpDelete("clear-database")]
        public async Task<ActionResult> ClearDatabase()
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
                await _seederService.ClearDatabaseAsync();
                
                return Ok(new
                {
                    message = "Database cleared successfully",
                    deletedTables = new[]
                    {
                        "PrescriptionItems",
                        "ExamRequests",
                        "Notifications",
                        "NotificationRoutines",
                        "DigitalPrescriptions",
                        "MedicalRecords",
                        "Invoices",
                        "Payments",
                        "AppointmentProcedures",
                        "Appointments",
                        "PatientClinicLinks",
                        "HealthInsurancePlans",
                        "Patients",
                        "PrescriptionTemplates",
                        "MedicalRecordTemplates",
                        "Medications",
                        "ExamCatalogs",
                        "Procedures",
                        "Expenses",
                        "Users",
                        "OwnerClinicLinks",
                        "ClinicSubscriptions",
                        "Owners",
                        "Clinics",
                        "HealthInsuranceOperators",
                        "SubscriptionPlans"
                    },
                    note = "All demo data has been removed from the database. You can now re-seed the database using POST /api/data-seeder/seed-demo"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    error = "An error occurred while clearing the database",
                    details = ex.Message
                });
            }
        }
    }
}
