using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.SystemAdmin.Api.Models;
using MedicSoft.SystemAdmin.Api.Services;
using MedicSoft.Shared.Authentication.Controllers;

namespace MedicSoft.SystemAdmin.Api.Controllers;

[Authorize]
[Route("api/[controller]")]
public class TicketsController : MicroserviceBaseController
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    /// <summary>
    /// Create a new ticket
    /// </summary>
    [HttpPost]
    public async Task<ActionResult> CreateTicket([FromBody] CreateTicketRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var userName = GetUsername() ?? "Unknown";
        var userEmail = GetUserEmail() ?? "";
        var clinicId = GetClinicId();
        var tenantId = GetTenantId();

        // Get clinic name if clinicId exists
        string? clinicName = null;
        if (clinicId.HasValue)
        {
            // TODO: Fetch clinic name from Clinics table
            // For now, leave it null - the frontend will handle display
            clinicName = null;
        }

        var ticketId = await _ticketService.CreateTicketAsync(
            request,
            userId.Value,
            userName,
            userEmail,
            clinicId,
            clinicName,
            tenantId ?? "");

        return Ok(new { message = "Chamado criado com sucesso", ticketId });
    }

    /// <summary>
    /// Get ticket by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult> GetTicket(Guid id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var isSystemOwner = IsSystemOwner();
        var ticket = await _ticketService.GetTicketByIdAsync(id, userId.Value, isSystemOwner);

        if (ticket == null)
            return NotFound(new { message = "Chamado não encontrado" });

        return Ok(ticket);
    }

    /// <summary>
    /// Get current user's tickets
    /// </summary>
    [HttpGet("my-tickets")]
    public async Task<ActionResult> GetMyTickets()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var tenantId = GetTenantId() ?? "";
        var tickets = await _ticketService.GetUserTicketsAsync(userId.Value, tenantId);

        return Ok(tickets);
    }

    /// <summary>
    /// Get tickets for a specific clinic (system owners only)
    /// </summary>
    [HttpGet("clinic/{clinicId}")]
    public async Task<ActionResult> GetClinicTickets(Guid clinicId)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var isSystemOwner = IsSystemOwner();
        var tenantId = GetTenantId() ?? "";

        var tickets = await _ticketService.GetClinicTicketsAsync(clinicId, tenantId, isSystemOwner);

        return Ok(tickets);
    }

    /// <summary>
    /// Get all tickets with filters (system owners only)
    /// </summary>
    [HttpGet]
    public async Task<ActionResult> GetAllTickets(
        [FromQuery] TicketStatus? status = null,
        [FromQuery] TicketType? type = null,
        [FromQuery] Guid? clinicId = null,
        [FromQuery] string? tenantId = null)
    {
        if (!IsSystemOwner())
            return Forbid();

        var tickets = await _ticketService.GetAllTicketsAsync(status, type, clinicId, tenantId);

        return Ok(tickets);
    }

    /// <summary>
    /// Update ticket details
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateTicket(Guid id, [FromBody] UpdateTicketRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var isSystemOwner = IsSystemOwner();
        var result = await _ticketService.UpdateTicketAsync(id, request, userId.Value, isSystemOwner);

        if (!result)
            return NotFound(new { message = "Chamado não encontrado ou sem permissão" });

        return Ok(new { message = "Chamado atualizado com sucesso" });
    }

    /// <summary>
    /// Update ticket status
    /// </summary>
    [HttpPut("{id}/status")]
    public async Task<ActionResult> UpdateTicketStatus(Guid id, [FromBody] UpdateTicketStatusRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var userName = GetUsername() ?? "Unknown";
        var isSystemOwner = IsSystemOwner();

        var result = await _ticketService.UpdateTicketStatusAsync(id, request, userId.Value, userName, isSystemOwner);

        if (!result)
            return NotFound(new { message = "Chamado não encontrado" });

        return Ok(new { message = "Status atualizado com sucesso" });
    }

    /// <summary>
    /// Assign ticket to system owner
    /// </summary>
    [HttpPut("{id}/assign")]
    public async Task<ActionResult> AssignTicket(Guid id, [FromBody] AssignTicketRequest request)
    {
        if (!IsSystemOwner())
            return Forbid();

        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var userName = GetUsername() ?? "Unknown";
        var result = await _ticketService.AssignTicketAsync(id, request, userId.Value, userName);

        if (!result)
            return NotFound(new { message = "Chamado não encontrado" });

        return Ok(new { message = "Chamado atribuído com sucesso" });
    }

    /// <summary>
    /// Add comment to ticket
    /// </summary>
    [HttpPost("{id}/comments")]
    public async Task<ActionResult> AddComment(Guid id, [FromBody] AddTicketCommentRequest request)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var userName = GetUsername() ?? "Unknown";
        var isSystemOwner = IsSystemOwner();

        var commentId = await _ticketService.AddCommentAsync(id, request, userId.Value, userName, isSystemOwner);

        return Ok(new { message = "Comentário adicionado com sucesso", commentId });
    }

    /// <summary>
    /// Upload attachment to ticket
    /// </summary>
    [HttpPost("{id}/attachments")]
    public async Task<ActionResult> UploadAttachment(Guid id, [FromBody] UploadAttachmentRequest request)
    {
        var attachmentId = await _ticketService.AddAttachmentAsync(id, request);

        return Ok(new { message = "Anexo enviado com sucesso", attachmentId });
    }

    /// <summary>
    /// Get unread updates count for current user
    /// </summary>
    [HttpGet("unread-count")]
    public async Task<ActionResult> GetUnreadCount()
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        var tenantId = GetTenantId() ?? "";
        var count = await _ticketService.GetUnreadUpdatesCountAsync(userId.Value, tenantId);

        return Ok(new { count });
    }

    /// <summary>
    /// Mark ticket as read
    /// </summary>
    [HttpPost("{id}/mark-read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        var userId = GetUserId();
        if (!userId.HasValue)
            return Unauthorized();

        await _ticketService.MarkTicketAsReadAsync(id, userId.Value);

        return Ok(new { message = "Marcado como lido" });
    }

    /// <summary>
    /// Get ticket statistics (system owners only)
    /// </summary>
    [HttpGet("statistics")]
    public async Task<ActionResult> GetStatistics([FromQuery] Guid? clinicId = null, [FromQuery] string? tenantId = null)
    {
        if (!IsSystemOwner())
            return Forbid();

        var stats = await _ticketService.GetTicketStatisticsAsync(clinicId, tenantId);

        return Ok(stats);
    }
}
