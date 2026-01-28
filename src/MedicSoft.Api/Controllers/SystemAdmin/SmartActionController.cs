using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services.SystemAdmin;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    [ApiController]
    [Route("api/system-admin/smart-actions")]
    [Authorize(Roles = "SystemAdmin")]
    public class SmartActionController : ControllerBase
    {
        private readonly ISmartActionService _smartActionService;
        private readonly ILogger<SmartActionController> _logger;

        public SmartActionController(
            ISmartActionService smartActionService,
            ILogger<SmartActionController> logger)
        {
            _smartActionService = smartActionService;
            _logger = logger;
        }

        [HttpPost("impersonate")]
        public async Task<ActionResult<ImpersonationResult>> ImpersonateClinic([FromBody] ImpersonateRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var token = await _smartActionService.ImpersonateClinicAsync(request.ClinicId, adminUserId);

                return Ok(new ImpersonationResult
                {
                    Token = token,
                    ExpiresIn = 7200 // 2 hours
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error impersonating clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("grant-credit")]
        public async Task<IActionResult> GrantCredit([FromBody] GrantCreditRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smartActionService.GrantCreditAsync(
                    request.ClinicId,
                    request.Days,
                    request.Reason,
                    adminUserId);

                return Ok(new { success = true, message = $"{request.Days} days granted" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error granting credit to clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("apply-discount")]
        public async Task<IActionResult> ApplyDiscount([FromBody] ApplyDiscountRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smartActionService.ApplyDiscountAsync(
                    request.ClinicId,
                    request.Percentage,
                    request.Months,
                    adminUserId);

                return Ok(new { success = true, message = $"{request.Percentage}% discount applied" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error applying discount to clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("suspend")]
        public async Task<IActionResult> SuspendTemporarily([FromBody] SuspendRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smartActionService.SuspendTemporarilyAsync(
                    request.ClinicId,
                    request.ReactivationDate,
                    request.Reason,
                    adminUserId);

                return Ok(new { success = true, message = "Clinic suspended" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error suspending clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("export-data")]
        public async Task<IActionResult> ExportClinicData([FromBody] ExportDataRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var data = await _smartActionService.ExportClinicDataAsync(request.ClinicId, adminUserId);

                return File(data, "application/json", $"clinic-{request.ClinicId}-data.json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error exporting data for clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("migrate-plan")]
        public async Task<IActionResult> MigratePlan([FromBody] MigratePlanRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smartActionService.MigratePlanAsync(
                    request.ClinicId,
                    request.NewPlanId,
                    request.ProRata,
                    adminUserId);

                return Ok(new { success = true, message = "Plan migrated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error migrating plan for clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("send-email")]
        public async Task<IActionResult> SendCustomEmail([FromBody] SendCustomEmailRequest request)
        {
            try
            {
                var adminUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                await _smartActionService.SendCustomEmailAsync(
                    request.ClinicId,
                    request.Subject,
                    request.Body,
                    adminUserId);

                return Ok(new { success = true, message = "Email sent" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error sending email to clinic {request.ClinicId}");
                return BadRequest(new { error = ex.Message });
            }
        }
    }

    // DTOs
    public class ImpersonateRequest
    {
        public int ClinicId { get; set; }
    }

    public class ImpersonationResult
    {
        public string Token { get; set; }
        public int ExpiresIn { get; set; }
    }

    public class GrantCreditRequest
    {
        public int ClinicId { get; set; }
        public int Days { get; set; }
        public string Reason { get; set; }
    }

    public class ApplyDiscountRequest
    {
        public int ClinicId { get; set; }
        public decimal Percentage { get; set; }
        public int Months { get; set; }
    }

    public class SuspendRequest
    {
        public int ClinicId { get; set; }
        public DateTime? ReactivationDate { get; set; }
        public string Reason { get; set; }
    }

    public class ExportDataRequest
    {
        public int ClinicId { get; set; }
    }

    public class MigratePlanRequest
    {
        public int ClinicId { get; set; }
        public int NewPlanId { get; set; }
        public bool ProRata { get; set; }
    }

    public class SendCustomEmailRequest
    {
        public int ClinicId { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
