using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing expenses (accounts payable)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ExpensesController : BaseController
    {
        private readonly MedicSoftDbContext _context;

        public ExpensesController(MedicSoftDbContext context, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _context = context;
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
            var query = _context.Expenses.AsQueryable();

            if (clinicId.HasValue)
                query = query.Where(e => e.ClinicId == clinicId.Value);

            if (!string.IsNullOrEmpty(status) && Enum.TryParse<ExpenseStatus>(status, out var expenseStatus))
                query = query.Where(e => e.Status == expenseStatus);

            if (!string.IsNullOrEmpty(category) && Enum.TryParse<ExpenseCategory>(category, out var expenseCategory))
                query = query.Where(e => e.Category == expenseCategory);

            var expenses = await query
                .OrderByDescending(e => e.DueDate)
                .Select(e => new ExpenseDto
                {
                    Id = e.Id,
                    ClinicId = e.ClinicId,
                    Description = e.Description,
                    Category = e.Category.ToString(),
                    Amount = e.Amount,
                    DueDate = e.DueDate,
                    PaidDate = e.PaidDate,
                    Status = e.Status.ToString(),
                    PaymentMethod = e.PaymentMethod != null ? e.PaymentMethod.ToString() : null,
                    PaymentReference = e.PaymentReference,
                    SupplierName = e.SupplierName,
                    SupplierDocument = e.SupplierDocument,
                    Notes = e.Notes,
                    CancellationReason = e.CancellationReason,
                    CreatedAt = e.CreatedAt,
                    UpdatedAt = e.UpdatedAt,
                    DaysOverdue = e.IsOverdue() ? e.DaysOverdue() : null
                })
                .ToListAsync();

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
            var expense = await _context.Expenses.FindAsync(id);

            if (expense == null)
                return NotFound("Expense not found");

            return Ok(new ExpenseDto
            {
                Id = expense.Id,
                ClinicId = expense.ClinicId,
                Description = expense.Description,
                Category = expense.Category.ToString(),
                Amount = expense.Amount,
                DueDate = expense.DueDate,
                PaidDate = expense.PaidDate,
                Status = expense.Status.ToString(),
                PaymentMethod = expense.PaymentMethod?.ToString(),
                PaymentReference = expense.PaymentReference,
                SupplierName = expense.SupplierName,
                SupplierDocument = expense.SupplierDocument,
                Notes = expense.Notes,
                CancellationReason = expense.CancellationReason,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt,
                DaysOverdue = expense.IsOverdue() ? expense.DaysOverdue() : null
            });
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

            if (!Enum.TryParse<ExpenseCategory>(dto.Category, out var category))
                return BadRequest("Invalid expense category");

            try
            {
                var expense = new Expense(
                    dto.ClinicId,
                    dto.Description,
                    category,
                    dto.Amount,
                    dto.DueDate,
                    GetTenantId(),
                    dto.SupplierName,
                    dto.SupplierDocument,
                    dto.Notes
                );

                _context.Expenses.Add(expense);
                await _context.SaveChangesAsync();

                var result = new ExpenseDto
                {
                    Id = expense.Id,
                    ClinicId = expense.ClinicId,
                    Description = expense.Description,
                    Category = expense.Category.ToString(),
                    Amount = expense.Amount,
                    DueDate = expense.DueDate,
                    PaidDate = expense.PaidDate,
                    Status = expense.Status.ToString(),
                    SupplierName = expense.SupplierName,
                    SupplierDocument = expense.SupplierDocument,
                    Notes = expense.Notes,
                    CreatedAt = expense.CreatedAt
                };

                return CreatedAtAction(nameof(GetById), new { id = expense.Id }, result);
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

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Expense not found");

            if (!Enum.TryParse<ExpenseCategory>(dto.Category, out var category))
                return BadRequest("Invalid expense category");

            try
            {
                expense.Update(
                    dto.Description,
                    category,
                    dto.Amount,
                    dto.DueDate,
                    dto.SupplierName,
                    dto.SupplierDocument,
                    dto.Notes
                );

                await _context.SaveChangesAsync();
                return NoContent();
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

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Expense not found");

            if (!Enum.TryParse<PaymentMethod>(dto.PaymentMethod, out var paymentMethod))
                return BadRequest("Invalid payment method");

            try
            {
                expense.MarkAsPaid(paymentMethod, dto.PaymentReference);
                await _context.SaveChangesAsync();
                return NoContent();
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

            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Expense not found");

            try
            {
                expense.Cancel(dto.Reason);
                await _context.SaveChangesAsync();
                return NoContent();
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
            var expense = await _context.Expenses.FindAsync(id);
            if (expense == null)
                return NotFound("Expense not found");

            _context.Expenses.Remove(expense);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
