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
                    new { username = "admin", role = "SystemAdmin", email = "admin@clinicademo.com.br" },
                    new { username = "dr.silva", role = "Doctor", email = "joao.silva@clinicademo.com.br" },
                    new { username = "recep.maria", role = "Receptionist", email = "maria.santos@clinicademo.com.br" }
                },
                patientsCount = 6,
                proceduresCount = 8,
                appointmentsCount = 5,
                note = "Use POST /api/data-seeder/seed-demo to create this demo data"
            });
        }
    }
}
