using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing expenses (accounts payable)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : BaseController
    {
        private readonly IExpenseService _expenseService;

        public ExpensesController(IExpenseService expenseService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _expenseService = expenseService;
        }

        /// <summary>
        /// Get all expenses for the clinic
        /// </summary>
        /// <param name="clinicId">Optional clinic ID filter</param>
        /// <param name="status">Optional status filter</param>
        /// <param name="category">Optional category filter</param>
        /// <returns>List of expenses</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ExpenseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ExpenseDto>>> GetAll(
            [FromQuery] Guid? clinicId = null,
            [FromQuery] string? status = null,
            [FromQuery] string? category = null)
        {
            var tenantId = GetTenantId();
            var expenses = await _expenseService.GetAllExpensesAsync(tenantId, clinicId, status, category);
            return Ok(expenses);
        }

        /// <summary>
        /// Get expense by ID
        /// </summary>
        /// <param name="id">Expense ID</param>
        /// <returns>Expense details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ExpenseDto>> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var expense = await _expenseService.GetExpenseByIdAsync(id, tenantId);

            if (expense == null)
                return NotFound("Expense not found");

            return Ok(expense);
        }

        /// <summary>
        /// Create a new expense
        /// </summary>
        /// <param name="dto">Expense details</param>
        /// <returns>Created expense</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ExpenseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ExpenseDto>> Create([FromBody] CreateExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tenantId = GetTenantId();
                var result = await _expenseService.CreateExpenseAsync(dto, tenantId);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an expense
        /// </summary>
        /// <param name="id">Expense ID</param>
        /// <param name="dto">Updated expense details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tenantId = GetTenantId();
                await _expenseService.UpdateExpenseAsync(id, dto, tenantId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mark expense as paid
        /// </summary>
        /// <param name="id">Expense ID</param>
        /// <param name="dto">Payment details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/pay")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Pay(Guid id, [FromBody] PayExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tenantId = GetTenantId();
                await _expenseService.MarkExpenseAsPaidAsync(id, dto, tenantId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel an expense
        /// </summary>
        /// <param name="id">Expense ID</param>
        /// <param name="dto">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelExpenseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tenantId = GetTenantId();
                await _expenseService.CancelExpenseAsync(id, dto, tenantId);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete an expense
        /// </summary>
        /// <param name="id">Expense ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _expenseService.DeleteExpenseAsync(id, tenantId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Expense not found");
            }
        }
    }
}
