using MedicSoft.SystemAdmin.Api.Models;

namespace MedicSoft.SystemAdmin.Api.Services;

public interface ITicketService
{
    Task<Guid> CreateTicketAsync(CreateTicketRequest request, Guid userId, string userName, string userEmail, Guid? clinicId, string? clinicName, string tenantId);
    Task<TicketDto?> GetTicketByIdAsync(Guid ticketId, Guid userId, bool isSystemOwner);
    Task<List<TicketSummaryDto>> GetUserTicketsAsync(Guid userId, string tenantId);
    Task<List<TicketSummaryDto>> GetClinicTicketsAsync(Guid clinicId, string tenantId, bool isSystemOwner);
    Task<List<TicketSummaryDto>> GetAllTicketsAsync(TicketStatus? status, TicketType? type, Guid? clinicId, string? tenantId);
    Task<bool> UpdateTicketAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, bool isSystemOwner);
    Task<bool> UpdateTicketStatusAsync(Guid ticketId, UpdateTicketStatusRequest request, Guid userId, string userName, bool isSystemOwner);
    Task<bool> AssignTicketAsync(Guid ticketId, AssignTicketRequest request, Guid systemOwnerId, string systemOwnerName);
    Task<Guid> AddCommentAsync(Guid ticketId, AddTicketCommentRequest request, Guid authorId, string authorName, bool isSystemOwner);
    Task<Guid> AddAttachmentAsync(Guid ticketId, UploadAttachmentRequest request);
    Task<int> GetUnreadUpdatesCountAsync(Guid userId, string tenantId);
    Task<bool> MarkTicketAsReadAsync(Guid ticketId, Guid userId);
    Task<TicketStatisticsDto> GetTicketStatisticsAsync(Guid? clinicId, string? tenantId);
}
