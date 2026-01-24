using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ControlledMedicationController : BaseController
    {
        private readonly IControlledMedicationRegistryService _registryService;
        private readonly IMonthlyBalanceService _balanceService;

        public ControlledMedicationController(
            IControlledMedicationRegistryService registryService,
            IMonthlyBalanceService balanceService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _registryService = registryService;
            _balanceService = balanceService;
        }

        /// <summary>
        /// Register manual stock entry of controlled medication
        /// </summary>
        [HttpPost("register-stock-entry")]
        public async Task<ActionResult<ControlledMedicationRegistry>> RegisterStockEntry([FromBody] StockEntryDto dto)
        {
            try
            {
                if (dto == null)
                    return BadRequest("Stock entry data is required");

                var registry = await _registryService.RegisterStockEntryAsync(
                    dto,
                    GetTenantId(),
                    GetUserId());

                return CreatedAtAction(nameof(GetBalance), new { medicationName = registry.MedicationName }, registry);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get registry entries for a period
        /// </summary>
        [HttpGet("registry")]
        public async Task<ActionResult<IEnumerable<ControlledMedicationRegistry>>> GetRegistry(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? medicationName)
        {
            try
            {
                IEnumerable<ControlledMedicationRegistry> entries;

                if (!string.IsNullOrWhiteSpace(medicationName))
                {
                    entries = await _registryService.GetRegistryByMedicationAsync(
                        medicationName,
                        GetTenantId());
                }
                else
                {
                    var end = endDate ?? DateTime.UtcNow;
                    entries = await _registryService.GetRegistryByPeriodAsync(
                        startDate,
                        end,
                        GetTenantId());
                }

                return Ok(entries);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get current balance for a specific medication
        /// </summary>
        [HttpGet("balance/{medicationName}")]
        public async Task<ActionResult<BalanceResponse>> GetBalance(string medicationName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(medicationName))
                    return BadRequest("Medication name is required");

                var balance = await _registryService.GetCurrentBalanceAsync(
                    medicationName,
                    GetTenantId());

                return Ok(new BalanceResponse
                {
                    MedicationName = medicationName,
                    Balance = balance
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get list of all controlled medications
        /// </summary>
        [HttpGet("medications")]
        public async Task<ActionResult<IEnumerable<string>>> GetMedications()
        {
            try
            {
                var medications = await _registryService.GetRegistryByPeriodAsync(
                    DateTime.UtcNow.AddYears(-10),
                    DateTime.UtcNow,
                    GetTenantId());

                var medicationNames = medications
                    .Select(m => m.MedicationName)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToList();

                return Ok(medicationNames);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get monthly balances for a specific month/year
        /// </summary>
        [HttpGet("balances/monthly")]
        public async Task<ActionResult<IEnumerable<MonthlyControlledBalance>>> GetMonthlyBalances(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            try
            {
                if (year < 2000 || year > DateTime.UtcNow.Year)
                    return BadRequest("Invalid year");

                if (month < 1 || month > 12)
                    return BadRequest("Month must be between 1 and 12");

                var balances = await _balanceService.CalculateMonthlyBalancesAsync(
                    year,
                    month,
                    GetTenantId());

                return Ok(balances);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Record physical inventory count for a monthly balance
        /// </summary>
        [HttpPost("balances/{id}/physical-inventory")]
        public async Task<ActionResult<MonthlyControlledBalance>> RecordPhysicalInventory(
            Guid id,
            [FromBody] PhysicalInventoryRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Physical inventory data is required");

                if (request.PhysicalCount < 0)
                    return BadRequest("Physical count cannot be negative");

                var balance = await _balanceService.RecordPhysicalInventoryAsync(
                    id,
                    request.PhysicalCount,
                    request.Reason,
                    GetTenantId(),
                    GetUserId());

                return Ok(balance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Close a monthly balance (lock it from further modifications)
        /// </summary>
        [HttpPost("balances/{id}/close")]
        public async Task<ActionResult<MonthlyControlledBalance>> CloseBalance(Guid id)
        {
            try
            {
                var balance = await _balanceService.CloseBalanceAsync(
                    id,
                    GetTenantId(),
                    GetUserId());

                return Ok(balance);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Calculate monthly balances for all controlled medications
        /// </summary>
        [HttpPost("balances/calculate")]
        public async Task<ActionResult<IEnumerable<MonthlyControlledBalance>>> CalculateMonthlyBalances(
            [FromQuery] int year,
            [FromQuery] int month)
        {
            try
            {
                if (year < 2000 || year > DateTime.UtcNow.Year)
                    return BadRequest("Invalid year");

                if (month < 1 || month > 12)
                    return BadRequest("Month must be between 1 and 12");

                var balances = await _balanceService.CalculateMonthlyBalancesAsync(
                    year,
                    month,
                    GetTenantId());

                return Ok(balances);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get overdue monthly balances (not closed past deadline)
        /// </summary>
        [HttpGet("balances/overdue")]
        public async Task<ActionResult<IEnumerable<MonthlyControlledBalance>>> GetOverdueBalances()
        {
            try
            {
                var balances = await _balanceService.GetOverdueBalancesAsync(GetTenantId());
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Get monthly balances with discrepancies between physical and calculated counts
        /// </summary>
        [HttpGet("balances/discrepancies")]
        public async Task<ActionResult<IEnumerable<MonthlyControlledBalance>>> GetBalancesWithDiscrepancies()
        {
            try
            {
                var balances = await _balanceService.GetBalancesWithDiscrepanciesAsync(GetTenantId());
                return Ok(balances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Request model for recording physical inventory
    /// </summary>
    public class PhysicalInventoryRequest
    {
        public decimal PhysicalCount { get; set; }
        public string? Reason { get; set; }
    }

    /// <summary>
    /// Response model for balance information
    /// </summary>
    public class BalanceResponse
    {
        public string MedicationName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
