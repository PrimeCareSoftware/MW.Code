using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing cash flow entries
    /// </summary>
    [ApiController]
    [Route("api/cash-flow")]
    [Authorize]
    public class CashFlowController : BaseController
    {
        private readonly ICashFlowEntryRepository _repository;

        public CashFlowController(
            ICashFlowEntryRepository repository,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all cash flow entries
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(IEnumerable<CashFlowEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CashFlowEntryDto>>> GetAll()
        {
            var entries = await _repository.GetAllAsync(GetTenantId());
            var dtos = entries.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get cash flow entry by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CashFlowEntryDto>> GetById(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id, GetTenantId());
            if (entry == null)
                return NotFound(new { message = "Entrada de fluxo de caixa não encontrada." });

            return Ok(MapToDto(entry));
        }

        /// <summary>
        /// Get cash flow entries by date range
        /// </summary>
        [HttpGet("by-date-range")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(IEnumerable<CashFlowEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CashFlowEntryDto>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var entries = await _repository.GetByDateRangeAsync(startDate, endDate, GetTenantId());
            var dtos = entries.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get cash flow entries by type (Income or Expense)
        /// </summary>
        [HttpGet("by-type/{type}")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(IEnumerable<CashFlowEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CashFlowEntryDto>>> GetByType(int type)
        {
            if (!Enum.IsDefined(typeof(CashFlowType), type))
                return BadRequest(new { message = "Tipo inválido." });

            var entries = await _repository.GetByTypeAsync((CashFlowType)type, GetTenantId());
            var dtos = entries.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get cash flow entries by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(IEnumerable<CashFlowEntryDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CashFlowEntryDto>>> GetByCategory(int category)
        {
            if (!Enum.IsDefined(typeof(CashFlowCategory), category))
                return BadRequest(new { message = "Categoria inválida." });

            var entries = await _repository.GetByCategoryAsync((CashFlowCategory)category, GetTenantId());
            var dtos = entries.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get cash flow summary for a date range
        /// </summary>
        [HttpGet("summary")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(CashFlowSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<CashFlowSummaryDto>> GetSummary(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var entries = await _repository.GetByDateRangeAsync(startDate, endDate, GetTenantId());
            
            var totalIncome = entries
                .Where(e => e.Type == CashFlowType.Income)
                .Sum(e => e.Amount);
            
            var totalExpense = entries
                .Where(e => e.Type == CashFlowType.Expense)
                .Sum(e => e.Amount);

            var summary = new CashFlowSummaryDto
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalIncome = totalIncome,
                TotalExpense = totalExpense,
                NetCashFlow = totalIncome - totalExpense,
                OpeningBalance = 0, // Can be calculated based on previous period if needed
                ClosingBalance = totalIncome - totalExpense
            };

            return Ok(summary);
        }

        /// <summary>
        /// Get total income for date range
        /// </summary>
        [HttpGet("total-income")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalIncome(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var total = await _repository.GetTotalIncomeAsync(startDate, endDate, GetTenantId());
            return Ok(total);
        }

        /// <summary>
        /// Get total expense for date range
        /// </summary>
        [HttpGet("total-expense")]
        [RequirePermissionKey(PermissionKeys.CashFlowView)]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalExpense(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var total = await _repository.GetTotalExpenseAsync(startDate, endDate, GetTenantId());
            return Ok(total);
        }

        /// <summary>
        /// Create a new cash flow entry
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.CashFlowManage)]
        [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CashFlowEntryDto>> Create([FromBody] CreateCashFlowEntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var entry = new CashFlowEntry(
                    (CashFlowType)dto.Type,
                    (CashFlowCategory)dto.Category,
                    dto.TransactionDate,
                    dto.Amount,
                    dto.Description,
                    GetTenantId(),
                    dto.Reference,
                    dto.PaymentId,
                    dto.ReceivableId,
                    dto.PayableId,
                    dto.AppointmentId
                );

                if (!string.IsNullOrEmpty(dto.BankAccount) || !string.IsNullOrEmpty(dto.PaymentMethod))
                    entry.UpdateBankingInfo(dto.BankAccount, dto.PaymentMethod);

                var created = await _repository.AddAsync(entry);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update cash flow entry
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.CashFlowManage)]
        [ProducesResponseType(typeof(CashFlowEntryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CashFlowEntryDto>> Update(Guid id, [FromBody] UpdateCashFlowEntryDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var entry = await _repository.GetByIdAsync(id, GetTenantId());
            if (entry == null)
                return NotFound(new { message = "Entrada de fluxo de caixa não encontrada." });

            try
            {
                if (!string.IsNullOrEmpty(dto.Description))
                    entry.UpdateDescription(dto.Description);

                if (dto.Notes != null)
                    entry.UpdateNotes(dto.Notes);

                if (!string.IsNullOrEmpty(dto.BankAccount) || !string.IsNullOrEmpty(dto.PaymentMethod))
                    entry.UpdateBankingInfo(dto.BankAccount, dto.PaymentMethod);

                await _repository.UpdateAsync(entry);
                return Ok(MapToDto(entry));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete cash flow entry
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.CashFlowManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var entry = await _repository.GetByIdAsync(id, GetTenantId());
            if (entry == null)
                return NotFound(new { message = "Entrada de fluxo de caixa não encontrada." });

            await _repository.DeleteAsync(id, GetTenantId());
            return NoContent();
        }

        private static CashFlowEntryDto MapToDto(CashFlowEntry entry)
        {
            return new CashFlowEntryDto
            {
                Id = entry.Id,
                Type = entry.Type.ToString(),
                Category = entry.Category.ToString(),
                TransactionDate = entry.TransactionDate,
                Amount = entry.Amount,
                Description = entry.Description,
                Reference = entry.Reference,
                Notes = entry.Notes,
                PaymentId = entry.PaymentId,
                ReceivableId = entry.ReceivableId,
                PayableId = entry.PayableId,
                AppointmentId = entry.AppointmentId,
                BankAccount = entry.BankAccount,
                PaymentMethod = entry.PaymentMethod,
                CreatedAt = entry.CreatedAt,
                UpdatedAt = entry.UpdatedAt
            };
        }
    }
}
