using System;
using System.Linq;
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
    /// Controller for managing financial closures
    /// </summary>
    [ApiController]
    [Route("api/financial-closures")]
    [Authorize]
    public class FinancialClosureController : BaseController
    {
        private readonly IFinancialClosureRepository _repository;

        public FinancialClosureController(
            IFinancialClosureRepository repository,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _repository = repository;
        }

        /// <summary>
        /// Get all financial closures
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(IEnumerable<FinancialClosureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FinancialClosureDto>>> GetAll()
        {
            var closures = await _repository.GetAllAsync(GetTenantId());
            var dtos = closures.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get financial closure by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> GetById(Guid id)
        {
            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            return Ok(MapToDto(closure));
        }

        /// <summary>
        /// Get financial closure by appointment ID
        /// </summary>
        [HttpGet("by-appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> GetByAppointmentId(Guid appointmentId)
        {
            var closure = await _repository.GetByAppointmentIdAsync(appointmentId, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado para este agendamento." });

            return Ok(MapToDto(closure));
        }

        /// <summary>
        /// Get financial closures by patient ID
        /// </summary>
        [HttpGet("by-patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(IEnumerable<FinancialClosureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FinancialClosureDto>>> GetByPatientId(Guid patientId)
        {
            var closures = await _repository.GetByPatientIdAsync(patientId, GetTenantId());
            var dtos = closures.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get financial closures by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(IEnumerable<FinancialClosureDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<FinancialClosureDto>>> GetByStatus(int status)
        {
            if (!Enum.IsDefined(typeof(FinancialClosureStatus), status))
                return BadRequest(new { message = "Status inválido." });

            var closures = await _repository.GetByStatusAsync((FinancialClosureStatus)status, GetTenantId());
            var dtos = closures.Select(MapToDto);
            return Ok(dtos);
        }

        /// <summary>
        /// Get financial closure by closure number
        /// </summary>
        [HttpGet("by-number/{closureNumber}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureView)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> GetByClosureNumber(string closureNumber)
        {
            var closure = await _repository.GetByClosureNumberAsync(closureNumber, GetTenantId());
            if (closure == null)
                return NotFound(new { message = $"Fechamento {closureNumber} não encontrado." });

            return Ok(MapToDto(closure));
        }

        /// <summary>
        /// Create a new financial closure
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<FinancialClosureDto>> Create([FromBody] CreateFinancialClosureDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var closure = new FinancialClosure(
                    dto.AppointmentId,
                    dto.PatientId,
                    dto.ClosureNumber,
                    (ClosurePaymentType)dto.PaymentType,
                    GetTenantId(),
                    dto.HealthInsuranceOperatorId
                );

                var created = await _repository.AddAsync(closure);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, MapToDto(created));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add item to financial closure
        /// </summary>
        [HttpPost("{id}/items")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> AddItem(Guid id, [FromBody] AddClosureItemDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (id != dto.ClosureId)
                return BadRequest(new { message = "ID incompatível." });

            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.AddItem(dto.Description, dto.Quantity, dto.UnitPrice, dto.CoverByInsurance);
                await _repository.UpdateAsync(closure);
                return Ok(MapToDto(closure));
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
        /// Remove item from financial closure
        /// </summary>
        [HttpDelete("{id}/items/{itemId}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> RemoveItem(Guid id, Guid itemId)
        {
            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.RemoveItem(itemId);
                await _repository.UpdateAsync(closure);
                return Ok(MapToDto(closure));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Apply discount to financial closure
        /// </summary>
        [HttpPost("{id}/apply-discount")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> ApplyDiscount(Guid id, [FromBody] ApplyClosureDiscountDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (id != dto.ClosureId)
                return BadRequest(new { message = "ID incompatível." });

            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.ApplyDiscount(dto.DiscountAmount, dto.Reason);
                await _repository.UpdateAsync(closure);
                return Ok(MapToDto(closure));
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
        /// Record payment for financial closure
        /// </summary>
        [HttpPost("{id}/record-payment")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> RecordPayment(Guid id, [FromBody] RecordClosurePaymentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (id != dto.ClosureId)
                return BadRequest(new { message = "ID incompatível." });

            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.RecordPayment(dto.Amount);
                await _repository.UpdateAsync(closure);
                return Ok(MapToDto(closure));
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
        /// Mark closure as pending payment
        /// </summary>
        [HttpPost("{id}/mark-pending")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(typeof(FinancialClosureDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<FinancialClosureDto>> MarkAsPendingPayment(Guid id)
        {
            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.MarkAsPendingPayment();
                await _repository.UpdateAsync(closure);
                return Ok(MapToDto(closure));
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel financial closure
        /// </summary>
        [HttpPost("{id}/cancel")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelClosureDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            if (id != dto.ClosureId)
                return BadRequest(new { message = "ID incompatível." });

            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            try
            {
                closure.Cancel(dto.Reason);
                await _repository.UpdateAsync(closure);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete financial closure
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.FinancialClosureManage)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var closure = await _repository.GetByIdAsync(id, GetTenantId());
            if (closure == null)
                return NotFound(new { message = "Fechamento financeiro não encontrado." });

            await _repository.DeleteAsync(id, GetTenantId());
            return NoContent();
        }

        private static FinancialClosureDto MapToDto(FinancialClosure closure)
        {
            return new FinancialClosureDto
            {
                Id = closure.Id,
                AppointmentId = closure.AppointmentId,
                PatientId = closure.PatientId,
                HealthInsuranceOperatorId = closure.HealthInsuranceOperatorId,
                ClosureNumber = closure.ClosureNumber,
                Status = closure.Status.ToString(),
                PaymentType = closure.PaymentType.ToString(),
                ClosureDate = closure.ClosureDate,
                TotalAmount = closure.TotalAmount,
                PatientAmount = closure.PatientAmount,
                InsuranceAmount = closure.InsuranceAmount,
                PaidAmount = closure.PaidAmount,
                OutstandingAmount = closure.OutstandingAmount,
                Notes = closure.Notes,
                SettlementDate = closure.SettlementDate,
                CancellationReason = closure.CancellationReason,
                DiscountAmount = closure.DiscountAmount,
                DiscountReason = closure.DiscountReason,
                Items = closure.Items.Select(i => new FinancialClosureItemDto
                {
                    Id = i.Id,
                    ClosureId = i.ClosureId,
                    Description = i.Description,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    TotalPrice = i.TotalPrice,
                    CoverByInsurance = i.CoverByInsurance,
                    CreatedAt = i.CreatedAt
                }).ToList(),
                CreatedAt = closure.CreatedAt,
                UpdatedAt = closure.UpdatedAt
            };
        }
    }
}
