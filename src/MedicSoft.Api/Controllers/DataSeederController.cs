using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataSeederController : BaseController
    {
        private readonly DataSeederService _seederService;

        public DataSeederController(DataSeederService seederService, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _seederService = seederService;
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
    }
}
