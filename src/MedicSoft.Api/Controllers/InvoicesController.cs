using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicSoft.Application.Commands.Invoices;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Invoices;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing invoices (nota fiscal)
    /// 
    /// NOTA IMPORTANTE / IMPORTANT NOTE:
    /// =================================
    /// Este sistema de nota fiscal está implementado, mas aguarda decisão sobre:
    /// - Usar um serviço externo para emissão de NF-e/NFS-e (Focus NFe, ENotas, etc)
    /// - Desenvolver integração própria com SEFAZ
    /// 
    /// This invoice system is implemented but awaits decision on:
    /// - Using an external service for NF-e/NFS-e issuance (Focus NFe, ENotas, etc)
    /// - Developing own integration with SEFAZ
    /// 
    /// TODO: Avaliar serviços externos para emissão de nota fiscal eletrônica
    /// TODO: Evaluate external services for electronic invoice issuance
    /// 
    /// Serviços sugeridos / Suggested services:
    /// - Focus NFe (https://focusnfe.com.br/)
    /// - ENotas (https://enotas.com.br/)
    /// - PlugNotas (https://plugnotas.com.br/)
    /// - NFSe.io (https://nfse.io/)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : BaseController
    {
        private readonly IMediator _mediator;

        public InvoicesController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new invoice
        /// </summary>
        /// <param name="createInvoiceDto">Invoice details</param>
        /// <returns>Created invoice</returns>
        [HttpPost]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceDto createInvoiceDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var command = new CreateInvoiceCommand(createInvoiceDto, GetTenantId());
                var invoice = await _mediator.Send(command);
                
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Issue an invoice (change status from Draft to Issued)
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/issue")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Issue(Guid id)
        {
            try
            {
                var command = new IssueInvoiceCommand(id, GetTenantId());
                await _mediator.Send(command);
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel an invoice
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <param name="cancelDto">Cancellation details</param>
        /// <returns>Success status</returns>
        [HttpPut("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelInvoiceDto cancelDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != cancelDto.InvoiceId)
                return BadRequest("Invoice ID mismatch");

            try
            {
                var command = new CancelInvoiceCommand(id, cancelDto.Reason, GetTenantId());
                await _mediator.Send(command);
                
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        /// <param name="id">Invoice ID</param>
        /// <returns>Invoice details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetById(Guid id)
        {
            var query = new GetInvoiceByIdQuery(id, GetTenantId());
            var invoice = await _mediator.Send(query);

            if (invoice == null)
                return NotFound($"Invoice with ID {id} not found");

            return Ok(invoice);
        }

        /// <summary>
        /// Get invoice by payment ID
        /// </summary>
        /// <param name="paymentId">Payment ID</param>
        /// <returns>Invoice details</returns>
        [HttpGet("payment/{paymentId}")]
        [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceDto>> GetByPaymentId(Guid paymentId)
        {
            var query = new GetInvoiceByPaymentIdQuery(paymentId, GetTenantId());
            var invoice = await _mediator.Send(query);

            if (invoice == null)
                return NotFound($"Invoice for payment {paymentId} not found");

            return Ok(invoice);
        }

        /// <summary>
        /// Get all overdue invoices
        /// </summary>
        /// <returns>List of overdue invoices</returns>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<List<InvoiceDto>>> GetOverdue()
        {
            var query = new GetOverdueInvoicesQuery(GetTenantId());
            var invoices = await _mediator.Send(query);

            return Ok(invoices);
        }
    }
}
