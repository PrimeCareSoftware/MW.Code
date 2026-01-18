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
    /// Controller for managing accounts payable
    /// </summary>
    [ApiController]
    [Route("api/accounts-payable")]
    [Authorize]
    public class AccountsPayableController : BaseController
    {
        private readonly IAccountsPayableRepository _repository;

        public AccountsPayableController(
            IAccountsPayableRepository repository,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all accounts payable
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsPayableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsPayableDto>>> GetAll()
        {
            var payables = await _repository.GetAllAsync(GetTenantId());
            var dtos = payables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts payable by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(AccountsPayableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsPayableDto>> GetById(Guid id)
        {
            var payable = await _repository.GetByIdAsync(id, GetTenantId());
            if (payable == null)
                return NotFound(new { message = "Conta a pagar não encontrada." });

            return Ok(MapToDto(payable));
        }

        /// <summary>
        /// Get accounts payable by supplier ID
        /// </summary>
        [HttpGet("by-supplier/{supplierId}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsPayableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsPayableDto>>> GetBySupplierId(Guid supplierId)
        {
            var payables = await _repository.GetBySupplierIdAsync(supplierId, GetTenantId());
            var dtos = payables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get overdue accounts payable
        /// </summary>
        [HttpGet("overdue")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsPayableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsPayableDto>>> GetOverdue()
        {
            var payables = await _repository.GetOverdueAsync(GetTenantId());
            var dtos = payables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts payable by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsPayableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsPayableDto>>> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(PayableStatus), status))
                return BadRequest(new { message = "Status inválido." });

            var payables = await _repository.GetByStatusAsync((PayableStatus)status, GetTenantId());
            var dtos = payables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts payable by category
        /// </summary>
        [HttpGet("by-category/{category}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsPayableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsPayableDto>>> GetByCategory(int category)
        {
            if (!Enum.IsDefined(typeof(PayableCategory), category))
                return BadRequest(new { message = "Categoria inválida." });

            var payables = await _repository.GetByCategoryAsync((PayableCategory)category, GetTenantId());
            var dtos = payables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get total outstanding amount
        /// </summary>
        [HttpGet("total-outstanding")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalOutstanding()
        {
            var total = await _repository.GetTotalOutstandingAsync(GetTenantId());
            return Ok(total);
        }

        /// <summary>
        /// Get total overdue amount
        /// </summary>
        [HttpGet("total-overdue")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableView)]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalOverdue()
        {
            var total = await _repository.GetTotalOverdueAsync(GetTenantId());
            return Ok(total);
        }

        /// <summary>
        /// Create a new accounts payable
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AccountsPayableManage)]
        [ProducesResponseType(typeof(AccountsPayableDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountsPayableDto>> Create([FromBody] CreateAccountsPayableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payable = new AccountsPayable(
                    dto.DocumentNumber,
                    (PayableCategory)dto.Category,
                    dto.DueDate,
                    dto.TotalAmount,
                    dto.Description,
                    GetTenantId(),
                    dto.SupplierId,
                    dto.InstallmentNumber,
                    dto.TotalInstallments
                );

                if (!string.IsNullOrEmpty(dto.BankName) || !string.IsNullOrEmpty(dto.BankAccount) || !string.IsNullOrEmpty(dto.PixKey))
                    payable.SetBankingInfo(dto.BankName, dto.BankAccount, dto.PixKey);

                var created = await _repository.AddAsync(payable);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update accounts payable
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableManage)]
        [ProducesResponseType(typeof(AccountsPayableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsPayableDto>> Update(Guid id, [FromBody] UpdateAccountsPayableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var payable = await _repository.GetByIdAsync(id, GetTenantId());
            if (payable == null)
                return NotFound(new { message = "Conta a pagar não encontrada." });

            try
            {
                if (dto.Notes != null)
                    payable.UpdateNotes(dto.Notes);

                if (!string.IsNullOrEmpty(dto.BankName) || !string.IsNullOrEmpty(dto.BankAccount) || !string.IsNullOrEmpty(dto.PixKey))
                    payable.SetBankingInfo(dto.BankName, dto.BankAccount, dto.PixKey);

                await _repository.UpdateAsync(payable);
                return Ok(MapToDto(payable));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add payment to accounts payable
        /// </summary>
        [HttpPost("{id}/payments")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableManage)]
        [ProducesResponseType(typeof(AccountsPayableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsPayableDto>> AddPayment(Guid id, [FromBody] AddPayablePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.PayableId)
                return BadRequest(new { message = "ID incompatível." });

            var payable = await _repository.GetByIdAsync(id, GetTenantId());
            if (payable == null)
                return NotFound(new { message = "Conta a pagar não encontrada." });

            try
            {
                payable.AddPayment(dto.Amount, dto.PaymentDate, dto.TransactionId, dto.Notes);
                await _repository.UpdateAsync(payable);
                return Ok(MapToDto(payable));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel accounts payable
        /// </summary>
        [HttpPost("{id}/cancel")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelPayableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.PayableId)
                return BadRequest(new { message = "ID incompatível." });

            var payable = await _repository.GetByIdAsync(id, GetTenantId());
            if (payable == null)
                return NotFound(new { message = "Conta a pagar não encontrada." });

            try
            {
                payable.Cancel(dto.Reason);
                await _repository.UpdateAsync(payable);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete accounts payable
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsPayableManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var payable = await _repository.GetByIdAsync(id, GetTenantId());
            if (payable == null)
                return NotFound(new { message = "Conta a pagar não encontrada." });

            await _repository.DeleteAsync(id, GetTenantId());
            return NoContent();
        }

        private static AccountsPayableDto MapToDto(AccountsPayable payable)
        {
            return new AccountsPayableDto
            {
                Id = payable.Id,
                DocumentNumber = payable.DocumentNumber,
                SupplierId = payable.SupplierId,
                Category = payable.Category.ToString(),
                Status = payable.Status.ToString(),
                IssueDate = payable.IssueDate,
                DueDate = payable.DueDate,
                TotalAmount = payable.TotalAmount,
                PaidAmount = payable.PaidAmount,
                OutstandingAmount = payable.OutstandingAmount,
                Description = payable.Description,
                Notes = payable.Notes,
                PaymentDate = payable.PaymentDate,
                CancellationReason = payable.CancellationReason,
                InstallmentNumber = payable.InstallmentNumber,
                TotalInstallments = payable.TotalInstallments,
                BankName = payable.BankName,
                BankAccount = payable.BankAccount,
                PixKey = payable.PixKey,
                DaysOverdue = payable.GetDaysOverdue(),
                IsOverdue = payable.IsOverdue(),
                Payments = payable.Payments.Select(p => new PayablePaymentDto
                {
                    Id = p.Id,
                    PayableId = p.PayableId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    TransactionId = p.TransactionId,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                CreatedAt = payable.CreatedAt,
                UpdatedAt = payable.UpdatedAt
            };
        }
    }
}
