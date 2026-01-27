using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Api.Controllers.CRM
{
    [Authorize]
    [ApiController]
    [Route("api/crm/complaint")]
    [Produces("application/json")]
    public class ComplaintController : BaseController
    {
        private readonly IComplaintService _complaintService;
        private readonly ILogger<ComplaintController> _logger;

        public ComplaintController(
            IComplaintService complaintService,
            ITenantContext tenantContext,
            ILogger<ComplaintController> logger)
            : base(tenantContext)
        {
            _complaintService = complaintService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new complaint
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComplaintDto>> Create([FromBody] CreateComplaintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.CreateAsync(dto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = complaint.Id }, 
                    complaint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating complaint");
                return StatusCode(500, new { message = "Erro ao criar reclamação" });
            }
        }

        /// <summary>
        /// Get all complaints
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetAll()
        {
            try
            {
                var tenantId = GetTenantId();
                var complaints = await _complaintService.GetAllAsync(tenantId);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all complaints");
                return StatusCode(500, new { message = "Erro ao buscar reclamações" });
            }
        }

        /// <summary>
        /// Get a specific complaint by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComplaintDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.GetByIdAsync(id, tenantId);
                
                if (complaint == null)
                    return NotFound(new { message = "Reclamação não encontrada" });

                return Ok(complaint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao buscar reclamação" });
            }
        }

        /// <summary>
        /// Get complaint by protocol number
        /// </summary>
        [HttpGet("protocol/{protocolNumber}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ComplaintDto>> GetByProtocol(string protocolNumber)
        {
            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.GetByProtocolNumberAsync(protocolNumber, tenantId);
                
                if (complaint == null)
                    return NotFound(new { message = "Reclamação não encontrada" });

                return Ok(complaint);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint by protocol {ProtocolNumber}", protocolNumber);
                return StatusCode(500, new { message = "Erro ao buscar reclamação" });
            }
        }

        /// <summary>
        /// Update complaint
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComplaintDto>> Update(
            Guid id, 
            [FromBody] UpdateComplaintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.UpdateAsync(id, dto, tenantId);
                return Ok(complaint);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Reclamação não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao atualizar reclamação" });
            }
        }

        /// <summary>
        /// Delete a complaint
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deleted = await _complaintService.DeleteAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound(new { message = "Reclamação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao deletar reclamação" });
            }
        }

        /// <summary>
        /// Add interaction to a complaint
        /// </summary>
        [HttpPost("{id}/interact")]
        [ProducesResponseType(typeof(ComplaintInteractionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComplaintInteractionDto>> AddInteraction(
            Guid id, 
            [FromBody] AddComplaintInteractionDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();
                var userName = GetUserName();
                
                var interaction = await _complaintService.AddInteractionAsync(id, dto, userId, userName, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = id }, 
                    interaction);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Reclamação não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding interaction to complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao adicionar interação" });
            }
        }

        /// <summary>
        /// Update complaint status
        /// </summary>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComplaintDto>> UpdateStatus(
            Guid id, 
            [FromBody] UpdateComplaintStatusDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.UpdateStatusAsync(id, dto.Status, tenantId);
                return Ok(complaint);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Reclamação não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status of complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao atualizar status da reclamação" });
            }
        }

        /// <summary>
        /// Assign complaint to user
        /// </summary>
        [HttpPut("{id}/assign")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ComplaintDto>> Assign(
            Guid id, 
            [FromBody] AssignComplaintDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var complaint = await _complaintService.AssignToUserAsync(id, dto.UserId, dto.UserName, tenantId);
                return Ok(complaint);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Reclamação não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning complaint {ComplaintId}", id);
                return StatusCode(500, new { message = "Erro ao atribuir reclamação" });
            }
        }

        /// <summary>
        /// Get dashboard metrics
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ComplaintDashboardDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<ComplaintDashboardDto>> GetDashboard()
        {
            try
            {
                var tenantId = GetTenantId();
                var dashboard = await _complaintService.GetDashboardMetricsAsync(tenantId);
                return Ok(dashboard);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaint dashboard");
                return StatusCode(500, new { message = "Erro ao buscar métricas do dashboard" });
            }
        }

        /// <summary>
        /// Get complaints by category
        /// </summary>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetByCategory(ComplaintCategory category)
        {
            try
            {
                var tenantId = GetTenantId();
                var complaints = await _complaintService.GetByCategoryAsync(category, tenantId);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints by category {Category}", category);
                return StatusCode(500, new { message = "Erro ao buscar reclamações por categoria" });
            }
        }

        /// <summary>
        /// Get complaints by status
        /// </summary>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetByStatus(ComplaintStatus status)
        {
            try
            {
                var tenantId = GetTenantId();
                var complaints = await _complaintService.GetByStatusAsync(status, tenantId);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints by status {Status}", status);
                return StatusCode(500, new { message = "Erro ao buscar reclamações por status" });
            }
        }

        /// <summary>
        /// Get complaints by priority
        /// </summary>
        [HttpGet("priority/{priority}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ComplaintDto>>> GetByPriority(ComplaintPriority priority)
        {
            try
            {
                var tenantId = GetTenantId();
                var complaints = await _complaintService.GetByPriorityAsync(priority, tenantId);
                return Ok(complaints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting complaints by priority {Priority}", priority);
                return StatusCode(500, new { message = "Erro ao buscar reclamações por prioridade" });
            }
        }
    }
}
