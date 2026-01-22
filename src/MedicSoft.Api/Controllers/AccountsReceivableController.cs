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
    /// Controller for managing accounts receivable
    /// </summary>
    [ApiController]
    [Route("api/accounts-receivable")]
    [Authorize]
    public class AccountsReceivableController : BaseController
    {
        private readonly IAccountsReceivableRepository _repository;
        private readonly ILogger<AccountsReceivableController> _logger;

        public AccountsReceivableController(
            IAccountsReceivableRepository repository,
            ILogger<AccountsReceivableController> logger,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _repository = repository;
            _logger = logger;
        }

        /// <summary>
        /// Get all accounts receivable
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsReceivableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsReceivableDto>>> GetAll()
        {
            var receivables = await _repository.GetAllAsync(GetTenantId());
            var dtos = receivables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts receivable by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(AccountsReceivableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsReceivableDto>> GetById(Guid id)
        {
            var receivable = await _repository.GetByIdAsync(id, GetTenantId());
            if (receivable == null)
                return NotFound(new { message = "Conta a receber não encontrada." });

            return Ok(MapToDto(receivable));
        }

        /// <summary>
        /// Get accounts receivable by patient ID
        /// </summary>
        [HttpGet("by-patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsReceivableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsReceivableDto>>> GetByPatientId(Guid patientId)
        {
            var receivables = await _repository.GetByPatientIdAsync(patientId, GetTenantId());
            var dtos = receivables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts receivable by appointment ID
        /// </summary>
        [HttpGet("by-appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsReceivableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsReceivableDto>>> GetByAppointmentId(Guid appointmentId)
        {
            var receivables = await _repository.GetByAppointmentIdAsync(appointmentId, GetTenantId());
            var dtos = receivables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get overdue accounts receivable
        /// </summary>
        [HttpGet("overdue")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsReceivableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsReceivableDto>>> GetOverdue()
        {
            var receivables = await _repository.GetOverdueAsync(GetTenantId());
            var dtos = receivables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get accounts receivable by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(IEnumerable<AccountsReceivableDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountsReceivableDto>>> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(ReceivableStatus), status))
                return BadRequest(new { message = "Status inválido." });

            var receivables = await _repository.GetByStatusAsync((ReceivableStatus)status, GetTenantId());
            var dtos = receivables.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get total outstanding amount
        /// </summary>
        [HttpGet("total-outstanding")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
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
        [RequirePermissionKey(PermissionKeys.AccountsReceivableView)]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        public async Task<ActionResult<decimal>> GetTotalOverdue()
        {
            var total = await _repository.GetTotalOverdueAsync(GetTenantId());
            return Ok(total);
        }

        /// <summary>
        /// Create a new accounts receivable
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableManage)]
        [ProducesResponseType(typeof(AccountsReceivableDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AccountsReceivableDto>> Create([FromBody] CreateAccountsReceivableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var receivable = new AccountsReceivable(
                    dto.DocumentNumber,
                    (ReceivableType)dto.Type,
                    dto.DueDate,
                    dto.TotalAmount,
                    GetTenantId(),
                    dto.AppointmentId,
                    dto.PatientId,
                    dto.HealthInsuranceOperatorId,
                    dto.Description,
                    dto.InstallmentNumber,
                    dto.TotalInstallments
                );

                var created = await _repository.AddAsync(receivable);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update accounts receivable
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableManage)]
        [ProducesResponseType(typeof(AccountsReceivableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsReceivableDto>> Update(Guid id, [FromBody] UpdateAccountsReceivableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var receivable = await _repository.GetByIdAsync(id, GetTenantId());
            if (receivable == null)
                return NotFound(new { message = "Conta a receber não encontrada." });

            try
            {
                if (dto.Notes != null)
                    receivable.UpdateNotes(dto.Notes);

                if (dto.InterestRate.HasValue || dto.FineRate.HasValue || dto.DiscountRate.HasValue)
                    receivable.SetCharges(dto.InterestRate, dto.FineRate, dto.DiscountRate);

                await _repository.UpdateAsync(receivable);
                return Ok(MapToDto(receivable));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add payment to accounts receivable
        /// </summary>
        [HttpPost("{id}/payments")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableManage)]
        [ProducesResponseType(typeof(AccountsReceivableDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountsReceivableDto>> AddPayment(Guid id, [FromBody] AddReceivablePaymentDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for AddPayment: {Errors}", 
                        string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
                    return BadRequest(ModelState);
                }

                if (id != dto.ReceivableId)
                {
                    _logger.LogWarning("Mismatched IDs in AddPayment: route id={RouteId}, dto id={DtoId}", id, dto.ReceivableId);
                    return BadRequest(new { message = "ID incompatível." });
                }

                var tenantId = GetTenantId();
                var receivable = await _repository.GetByIdAsync(id, tenantId);
                
                if (receivable == null)
                {
                    _logger.LogWarning("Receivable not found: id={Id}, tenantId={TenantId}", id, tenantId);
                    return NotFound(new { message = "Conta a receber não encontrada." });
                }

                _logger.LogInformation("Adding payment to receivable {ReceivableId}: amount={Amount}, date={Date}", 
                    id, dto.Amount, dto.PaymentDate);

                receivable.AddPayment(dto.Amount, dto.PaymentDate, dto.TransactionId, dto.Notes);
                await _repository.UpdateAsync(receivable);

                _logger.LogInformation("Payment added successfully to receivable {ReceivableId}", id);
                return Ok(MapToDto(receivable));
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation when adding payment to receivable {ReceivableId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid argument when adding payment to receivable {ReceivableId}", id);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error adding payment to receivable {ReceivableId}", id);
                return StatusCode(500, new { message = "Ocorreu um erro ao processar o pagamento. Por favor, tente novamente." });
            }
        }

        /// <summary>
        /// Cancel accounts receivable
        /// </summary>
        [HttpPost("{id}/cancel")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelReceivableDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.ReceivableId)
                return BadRequest(new { message = "ID incompatível." });

            var receivable = await _repository.GetByIdAsync(id, GetTenantId());
            if (receivable == null)
                return NotFound(new { message = "Conta a receber não encontrada." });

            try
            {
                receivable.Cancel(dto.Reason);
                await _repository.UpdateAsync(receivable);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete accounts receivable
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.AccountsReceivableManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var receivable = await _repository.GetByIdAsync(id, GetTenantId());
            if (receivable == null)
                return NotFound(new { message = "Conta a receber não encontrada." });

            await _repository.DeleteAsync(id, GetTenantId());
            return NoContent();
        }

        private static AccountsReceivableDto MapToDto(AccountsReceivable receivable)
        {
            return new AccountsReceivableDto
            {
                Id = receivable.Id,
                AppointmentId = receivable.AppointmentId,
                PatientId = receivable.PatientId,
                HealthInsuranceOperatorId = receivable.HealthInsuranceOperatorId,
                DocumentNumber = receivable.DocumentNumber,
                Type = receivable.Type.ToString(),
                Status = receivable.Status.ToString(),
                IssueDate = receivable.IssueDate,
                DueDate = receivable.DueDate,
                TotalAmount = receivable.TotalAmount,
                PaidAmount = receivable.PaidAmount,
                OutstandingAmount = receivable.OutstandingAmount,
                Description = receivable.Description,
                Notes = receivable.Notes,
                SettlementDate = receivable.SettlementDate,
                CancellationReason = receivable.CancellationReason,
                InstallmentNumber = receivable.InstallmentNumber,
                TotalInstallments = receivable.TotalInstallments,
                InterestRate = receivable.InterestRate,
                FineRate = receivable.FineRate,
                DiscountRate = receivable.DiscountRate,
                DaysOverdue = receivable.GetDaysOverdue(),
                IsOverdue = receivable.IsOverdue(),
                Payments = receivable.Payments.Select(p => new ReceivablePaymentDto
                {
                    Id = p.Id,
                    ReceivableId = p.ReceivableId,
                    Amount = p.Amount,
                    PaymentDate = p.PaymentDate,
                    TransactionId = p.TransactionId,
                    Notes = p.Notes,
                    CreatedAt = p.CreatedAt
                }).ToList(),
                CreatedAt = receivable.CreatedAt,
                UpdatedAt = receivable.UpdatedAt
            };
        }
    }
}
