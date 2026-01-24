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
    /// Controller for managing TISS batches
    /// </summary>
    [ApiController]
    [Route("api/tiss-batches")]
    [Authorize]
    public class TissBatchesController : BaseController
    {
        private readonly ITissBatchService _tissBatchService;

        public TissBatchesController(
            ITissBatchService tissBatchService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _tissBatchService = tissBatchService;
        }

        /// <summary>
        /// Get all TISS batches
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissBatchDto>>> GetAll()
        {
            var batches = await _tissBatchService.GetAllAsync(GetTenantId());
            return Ok(batches);
        }

        /// <summary>
        /// Get TISS batch by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<TissBatchDto>> GetById(Guid id)
        {
            var batch = await _tissBatchService.GetByIdAsync(id, GetTenantId());
            if (batch == null)
                return NotFound(new { message = $"Lote TISS não encontrado." });

            return Ok(batch);
        }

        /// <summary>
        /// Get TISS batches by clinic ID
        /// </summary>
        [HttpGet("by-clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissBatchDto>>> GetByClinicId(Guid clinicId)
        {
            var batches = await _tissBatchService.GetByClinicIdAsync(clinicId, GetTenantId());
            return Ok(batches);
        }

        /// <summary>
        /// Get TISS batches by operator ID
        /// </summary>
        [HttpGet("by-operator/{operatorId}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissBatchDto>>> GetByOperatorId(Guid operatorId)
        {
            var batches = await _tissBatchService.GetByOperatorIdAsync(operatorId, GetTenantId());
            return Ok(batches);
        }

        /// <summary>
        /// Get TISS batches by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult<IEnumerable<TissBatchDto>>> GetByStatus(string status)
        {
            var batches = await _tissBatchService.GetByStatusAsync(status, GetTenantId());
            return Ok(batches);
        }

        /// <summary>
        /// Create a new TISS batch
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.TissCreate)]
        public async Task<ActionResult<TissBatchDto>> Create([FromBody] CreateTissBatchDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var batch = await _tissBatchService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = batch.Id }, batch);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Add a guide to a batch
        /// </summary>
        [HttpPost("{id}/guides/{guideId}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> AddGuide(Guid id, Guid guideId)
        {
            try
            {
                var batch = await _tissBatchService.AddGuideAsync(id, guideId, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Remove a guide from a batch
        /// </summary>
        [HttpDelete("{id}/guides/{guideId}")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> RemoveGuide(Guid id, Guid guideId)
        {
            try
            {
                var batch = await _tissBatchService.RemoveGuideAsync(id, guideId, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Generate XML for a batch
        /// </summary>
        [HttpPost("{id}/generate-xml")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissXmlGenerationResultDto>> GenerateXml(Guid id)
        {
            try
            {
                var result = await _tissBatchService.GenerateXmlAsync(id, GetTenantId());
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Download the XML file for a batch
        /// </summary>
        [HttpGet("{id}/download-xml")]
        [RequirePermissionKey(PermissionKeys.TissView)]
        public async Task<ActionResult> DownloadXml(Guid id)
        {
            try
            {
                var xmlContent = await _tissBatchService.DownloadXmlAsync(id, GetTenantId());
                if (xmlContent == null)
                    return NotFound(new { message = "XML não encontrado para este lote." });

                return File(xmlContent, "application/xml", $"tiss_batch_{id}.xml");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark batch as ready to send
        /// </summary>
        [HttpPost("{id}/ready-to-send")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> MarkAsReadyToSend(Guid id)
        {
            try
            {
                var batch = await _tissBatchService.MarkAsReadyToSendAsync(id, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Submit a batch to the operator
        /// </summary>
        [HttpPost("{id}/submit")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> Submit(Guid id)
        {
            try
            {
                var batch = await _tissBatchService.SubmitAsync(id, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Process operator response for a batch
        /// </summary>
        [HttpPost("{id}/process-response")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> ProcessResponse(Guid id, [FromBody] ProcessBatchResponseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var batch = await _tissBatchService.ProcessResponseAsync(id, dto, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark a batch as paid
        /// </summary>
        [HttpPost("{id}/mark-paid")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> MarkAsPaid(Guid id)
        {
            try
            {
                var batch = await _tissBatchService.MarkAsPaidAsync(id, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Reject a batch
        /// </summary>
        [HttpPost("{id}/reject")]
        [RequirePermissionKey(PermissionKeys.TissEdit)]
        public async Task<ActionResult<TissBatchDto>> Reject(Guid id)
        {
            try
            {
                var batch = await _tissBatchService.RejectAsync(id, GetTenantId());
                return Ok(batch);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
