using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Complaint Controller
    /// 
    /// Manages patient complaints and service issues with comprehensive tracking and resolution workflows.
    /// Provides endpoints for complaint submission, tracking, status updates, and resolution management
    /// with full audit trail and escalation capabilities.
    /// </summary>
    /// <remarks>
    /// This controller enables:
    /// - Complaint registration and documentation
    /// - Multi-status workflow management (open, in-progress, resolved, closed)
    /// - Interaction/activity tracking on complaints
    /// - Assignment to responsible personnel
    /// - Category and priority-based filtering and routing
    /// - Dashboard metrics and KPI reporting
    /// - Complete audit trail for compliance and quality assurance
    /// 
    /// Complaints are critical for identifying service quality issues and driving organizational improvements.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/crm/complaint")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "CRM - Complaint Management")]
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
        /// <remarks>
        /// Registers a new patient complaint in the system. The complaint is assigned a unique protocol
        /// number for reference and tracking. Complaints are created with an initial status and can be
        /// assigned to personnel for resolution.
        /// 
        /// Business Logic:
        /// - Generates unique protocol number for complaint tracking
        /// - Validates patient and complaint category information
        /// - Initializes complaint with default status (Open/New)
        /// - Records creation timestamp and submitter information
        /// - Triggers notification workflows to assigned personnel
        /// - Maintains complete audit trail
        /// </remarks>
        /// <param name="dto">The complaint creation request with patient, description, category, and priority information.</param>
        /// <returns>The newly created complaint with assigned protocol number and metadata.</returns>
        /// <response code="201">Successfully created the complaint; Location header contains resource URL</response>
        /// <response code="400">Invalid request data or validation error (missing required fields, invalid category, etc.)</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves a complete list of all complaints in the system. Results are filtered by the
        /// current tenant context. Use status, category, or priority filters for more targeted queries.
        /// </remarks>
        /// <returns>A collection of all complaints with basic information.</returns>
        /// <response code="200">Successfully retrieved all complaints</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves detailed information about a single complaint including all interactions,
        /// status history, assignments, and attachments. Provides complete context for complaint
        /// resolution and follow-up activities.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint to retrieve.</param>
        /// <returns>The complete complaint information with full history and interactions.</returns>
        /// <response code="200">Successfully retrieved the complaint</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves a complaint using its unique protocol number. This is the preferred lookup method
        /// for patient-facing interfaces and correspondence, as protocol numbers are more memorable
        /// and user-friendly than GUID identifiers.
        /// </remarks>
        /// <param name="protocolNumber">The unique protocol number assigned to the complaint (e.g., "COMP-2024-001234").</param>
        /// <returns>The complete complaint information matching the protocol number.</returns>
        /// <response code="200">Successfully retrieved the complaint</response>
        /// <response code="404">Complaint with specified protocol number not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("protocol/{protocolNumber}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Updates the base complaint information such as description, attached notes, or metadata.
        /// Does not update complaint status or assignments; use specific status and assignment endpoints
        /// for those operations to maintain proper audit trails.
        /// 
        /// Business Logic:
        /// - Validates update does not conflict with current status
        /// - Prevents updates to resolved/closed complaints without special permissions
        /// - Records modification timestamp and user information
        /// - Maintains version history for audit trail
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint to update.</param>
        /// <param name="dto">The complaint update request with modified information.</param>
        /// <returns>The updated complaint object.</returns>
        /// <response code="200">Successfully updated the complaint</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid request data or validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Permanently deletes a complaint from the system. Deleted complaints cannot be recovered.
        /// For compliance and audit purposes, consider marking as archived instead of deletion.
        /// Only new/draft complaints can be deleted without special approval.
        /// 
        /// Business Logic:
        /// - Prevents deletion of complaints with interactions or historical data
        /// - Requires permission for deletion of in-progress or resolved complaints
        /// - Archives audit trail before deletion
        /// - Returns 404 if complaint not found or already deleted
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <response code="204">Successfully deleted the complaint</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Records an interaction or activity on a complaint such as a call, email, visit, or note.
        /// Interactions create the action history that tracks complaint resolution progress and
        /// provides a complete communication timeline.
        /// 
        /// Business Logic:
        /// - Records interaction type (call, email, note, etc.)
        /// - Associates interaction with the current user
        /// - Updates last interaction timestamp on complaint
        /// - May trigger escalation or SLA alerts based on interaction content
        /// - Maintains complete audit trail with timestamps
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint to add interaction to.</param>
        /// <param name="dto">The interaction details including type, description, and any attachments.</param>
        /// <returns>The recorded interaction with timestamp and confirmation.</returns>
        /// <response code="201">Successfully recorded the interaction</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid request data or validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{id}/interact")]
        [ProducesResponseType(typeof(ComplaintInteractionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Updates the complaint status through the resolution workflow. Valid status transitions are
        /// enforced to maintain process integrity (Open → In Progress → Resolved → Closed).
        /// Status changes trigger notifications and may impact SLA tracking.
        /// 
        /// Business Logic:
        /// - Validates status transition is allowed per workflow
        /// - Records status change timestamp and responsible user
        /// - May require notes or justification for certain transitions
        /// - Triggers notifications to interested parties
        /// - Updates SLA metrics and escalation status
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint whose status will be updated.</param>
        /// <param name="dto">The status update request containing the new status value.</param>
        /// <returns>The updated complaint with new status and metadata.</returns>
        /// <response code="200">Successfully updated the complaint status</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid status transition or validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPut("{id}/status")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Assigns a complaint to a specific user for investigation and resolution. Reassignment is
        /// allowed at any time and automatically notifies the newly assigned user. Previous assignments
        /// are retained in the audit trail.
        /// 
        /// Business Logic:
        /// - Updates assigned user and timestamp
        /// - Sends notification to newly assigned user
        /// - Records assignment change in audit trail
        /// - Updates workload and dashboard visibility
        /// - Validates user exists and is active
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the complaint to assign.</param>
        /// <param name="dto">The assignment request with user ID and name.</param>
        /// <returns>The updated complaint with new assignment information.</returns>
        /// <response code="200">Successfully assigned the complaint</response>
        /// <response code="404">Complaint not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid request data or validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPut("{id}/assign")]
        [ProducesResponseType(typeof(ComplaintDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves comprehensive KPI metrics for complaint management dashboard. Provides high-level
        /// overview of complaint volume, status distribution, resolution times, and escalation status
        /// for the current organization/tenant.
        /// 
        /// Dashboard Metrics Include:
        /// - Total complaints and breakdown by status
        /// - Average resolution time and SLA compliance rate
        /// - Open/unresolved complaint count
        /// - Escalated complaints requiring immediate attention
        /// - Top complaint categories and common issues
        /// - Performance trends and historical comparison
        /// </remarks>
        /// <returns>The dashboard metrics object with comprehensive complaint KPIs.</returns>
        /// <response code="200">Successfully retrieved dashboard metrics</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(ComplaintDashboardDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves all complaints grouped by their assigned category. Useful for identifying
        /// common complaint patterns and areas needing improvement.
        /// </remarks>
        /// <param name="category">The complaint category to filter by (e.g., Quality, Billing, Staff, Facility, etc.).</param>
        /// <returns>A collection of complaints matching the specified category.</returns>
        /// <response code="200">Successfully retrieved complaints by category</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("category/{category}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves complaints filtered by their current workflow status. Useful for workload planning,
        /// reporting on resolution progress, and identifying bottlenecks in the complaint process.
        /// </remarks>
        /// <param name="status">The complaint status to filter by (Open, InProgress, Resolved, Closed).</param>
        /// <returns>A collection of complaints with the specified status.</returns>
        /// <response code="200">Successfully retrieved complaints by status</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("status/{status}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves complaints filtered by their urgency/priority level. Allows management to focus
        /// on critical and high-priority issues that require immediate attention and resolution.
        /// </remarks>
        /// <param name="priority">The complaint priority level to filter by (Critical, High, Medium, Low).</param>
        /// <returns>A collection of complaints with the specified priority level.</returns>
        /// <response code="200">Successfully retrieved complaints by priority</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("priority/{priority}")]
        [ProducesResponseType(typeof(IEnumerable<ComplaintDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
