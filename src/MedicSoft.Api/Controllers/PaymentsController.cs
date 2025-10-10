using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicSoft.Application.Commands.Payments;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Payments;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing payments for appointments and subscriptions
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : BaseController
    {
        private readonly IMediator _mediator;

        public PaymentsController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new payment
        /// </summary>
        /// <param name="createPaymentDto">Payment details</param>
        /// <returns>Created payment</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PaymentDto>> Create([FromBody] CreatePaymentDto createPaymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreatePaymentCommand(createPaymentDto, GetTenantId());
                var payment = await _mediator.Send(command);
                
                return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Process a payment (mark as paid)
        /// </summary>
        /// <param name="processPaymentDto">Payment processing details</param>
        /// <returns>Updated payment</returns>
        [HttpPut("process")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> Process([FromBody] ProcessPaymentDto processPaymentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new ProcessPaymentCommand(processPaymentDto, GetTenantId());
                var payment = await _mediator.Send(command);
                
                return Ok(payment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Refund a payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="refundDto">Refund details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/refund")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Refund(Guid id, [FromBody] RefundPaymentDto refundDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != refundDto.PaymentId)
                return BadRequest("Payment ID mismatch");

            try
            {
                var command = new RefundPaymentCommand(id, refundDto.Reason, GetTenantId());
                await _mediator.Send(command);
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel a payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="cancelDto">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelPaymentDto cancelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != cancelDto.PaymentId)
                return BadRequest("Payment ID mismatch");

            try
            {
                var command = new CancelPaymentCommand(id, cancelDto.Reason, GetTenantId());
                await _mediator.Send(command);
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PaymentDto>> GetById(Guid id)
        {
            var query = new GetPaymentByIdQuery(id, GetTenantId());
            var payment = await _mediator.Send(query);

            if (payment == null)
                return NotFound($"Payment with ID {id} not found");

            return Ok(payment);
        }

        /// <summary>
        /// Get all payments for an appointment
        /// </summary>
        /// <param name="appointmentId">Appointment ID</param>
        /// <returns>List of payments</returns>
        [HttpGet("appointment/{appointmentId}")]
        [ProducesResponseType(typeof(List<PaymentDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<PaymentDto>>> GetByAppointmentId(Guid appointmentId)
        {
            var query = new GetAppointmentPaymentsQuery(appointmentId, GetTenantId());
            var payments = await _mediator.Send(query);

            return Ok(payments);
        }
    }
}
