using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing TISS guides
    /// </summary>
    [ApiController]
    [Route("api/tiss-guides")]
    [Authorize]
    public class TissGuidesController : BaseController
    {
        private readonly ITissGuideService _tissGuideService;

        public TissGuidesController(
            ITissGuideService tissGuideService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _tissGuideService = tissGuideService;
        }

        /// <summary>
        /// Get all TISS guides
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGuideDto>>> GetAll()
        {
            var guides = await _tissGuideService.GetAllAsync(GetTenantId());
            return Ok(guides);
        }

        /// <summary>
        /// Get TISS guide by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissGuideDto>> GetById(Guid id)
        {
            var guide = await _tissGuideService.GetByIdAsync(id, GetTenantId());
            if (guide == null)
                return NotFound(new { message = $"Guia TISS não encontrada." });

            return Ok(guide);
        }

        /// <summary>
        /// Get TISS guides by batch ID
        /// </summary>
        [HttpGet("by-batch/{batchId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGuideDto>>> GetByBatchId(Guid batchId)
        {
            var guides = await _tissGuideService.GetByBatchIdAsync(batchId, GetTenantId());
            return Ok(guides);
        }

        /// <summary>
        /// Get TISS guides by appointment ID
        /// </summary>
        [HttpGet("by-appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGuideDto>>> GetByAppointmentId(Guid appointmentId)
        {
            var guides = await _tissGuideService.GetByAppointmentIdAsync(appointmentId, GetTenantId());
            return Ok(guides);
        }

        /// <summary>
        /// Get TISS guides by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissGuideDto>>> GetByStatus(string status)
        {
            var guides = await _tissGuideService.GetByStatusAsync(status, GetTenantId());
            return Ok(guides);
        }

        /// <summary>
        /// Get TISS guide by guide number
        /// </summary>
        [HttpGet("by-number/{guideNumber}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissGuideDto>> GetByGuideNumber(string guideNumber)
        {
            var guide = await _tissGuideService.GetByGuideNumberAsync(guideNumber, GetTenantId());
            if (guide == null)
                return NotFound(new { message = $"Guia {guideNumber} não encontrada." });

            return Ok(guide);
        }

        /// <summary>
        /// Create a new TISS guide
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TissCreate)]
        public async Task<ActionResult<TissGuideDto>> Create([FromBody] CreateTissGuideDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var guide = await _tissGuideService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = guide.Id }, guide);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add a procedure to a TISS guide
        /// </summary>
        [HttpPost("{id}/procedures")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGuideDto>> AddProcedure(Guid id, [FromBody] AddProcedureToGuideDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var guide = await _tissGuideService.AddProcedureAsync(id, dto, GetTenantId());
                return Ok(guide);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove a procedure from a TISS guide
        /// </summary>
        [HttpDelete("{id}/procedures/{procedureId}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGuideDto>> RemoveProcedure(Guid id, Guid procedureId)
        {
            try
            {
                var guide = await _tissGuideService.RemoveProcedureAsync(id, procedureId, GetTenantId());
                return Ok(guide);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Finalize a TISS guide (mark as ready to send)
        /// </summary>
        [HttpPost("{id}/finalize")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGuideDto>> Finalize(Guid id)
        {
            try
            {
                var guide = await _tissGuideService.FinalizeAsync(id, GetTenantId());
                return Ok(guide);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Process operator response for a guide
        /// </summary>
        [HttpPost("{id}/process-response")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGuideDto>> ProcessResponse(Guid id, [FromBody] ProcessGuideResponseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var guide = await _tissGuideService.ProcessResponseAsync(id, dto, GetTenantId());
                return Ok(guide);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark a guide as paid
        /// </summary>
        [HttpPost("{id}/mark-paid")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissGuideDto>> MarkAsPaid(Guid id)
        {
            try
            {
                var guide = await _tissGuideService.MarkAsPaidAsync(id, GetTenantId());
                return Ok(guide);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
