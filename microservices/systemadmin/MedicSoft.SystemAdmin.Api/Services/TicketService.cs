using Microsoft.EntityFrameworkCore;
using MedicSoft.SystemAdmin.Api.Data;
using MedicSoft.SystemAdmin.Api.Models;

namespace MedicSoft.SystemAdmin.Api.Services;

public class TicketService : ITicketService
{
    private readonly SystemAdminDbContext _context;

    public TicketService(SystemAdminDbContext context)
    {
        _context = context;
    }

    public async Task<Guid> CreateTicketAsync(
        CreateTicketRequest request,
        Guid userId,
        string userName,
        string userEmail,
        Guid? clinicId,
        string? clinicName,
        string tenantId)
    {
        var ticket = new TicketEntity
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            Type = (int)request.Type,
            Status = (int)TicketStatus.Open,
            Priority = (int)request.Priority,
            UserId = userId,
            UserName = userName,
            UserEmail = userEmail,
            ClinicId = clinicId,
            ClinicName = clinicName,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LastStatusChangeAt = DateTime.UtcNow
        };

        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();

        return ticket.Id;
    }

    public async Task<TicketDto?> GetTicketByIdAsync(Guid ticketId, Guid userId, bool isSystemOwner)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return null;

        // Check permissions
        if (!isSystemOwner && ticket.UserId != userId)
            return null;

        var comments = await _context.TicketComments
            .Where(c => c.TicketId == ticketId && (!c.IsInternal || isSystemOwner))
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        var attachments = await _context.TicketAttachments
            .Where(a => a.TicketId == ticketId)
            .OrderBy(a => a.UploadedAt)
            .ToListAsync();

        return new TicketDto
        {
            Id = ticket.Id,
            Title = ticket.Title,
            Description = ticket.Description,
            Type = (TicketType)ticket.Type,
            Status = (TicketStatus)ticket.Status,
            Priority = (TicketPriority)ticket.Priority,
            UserId = ticket.UserId,
            UserName = ticket.UserName,
            UserEmail = ticket.UserEmail,
            ClinicId = ticket.ClinicId,
            ClinicName = ticket.ClinicName,
            TenantId = ticket.TenantId,
            AssignedToId = ticket.AssignedToId,
            AssignedToName = ticket.AssignedToName,
            Comments = comments.Select(c => new TicketCommentDto
            {
                Id = c.Id,
                TicketId = c.TicketId,
                Comment = c.Comment,
                AuthorName = c.AuthorName,
                IsInternal = c.IsInternal,
                IsSystemOwner = c.IsSystemOwner,
                CreatedAt = c.CreatedAt
            }).ToList(),
            Attachments = attachments.Select(a => new TicketAttachmentDto
            {
                Id = a.Id,
                TicketId = a.TicketId,
                FileName = a.FileName,
                FileUrl = a.FileUrl,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                UploadedAt = a.UploadedAt
            }).ToList(),
            CreatedAt = ticket.CreatedAt,
            UpdatedAt = ticket.UpdatedAt,
            LastStatusChangeAt = ticket.LastStatusChangeAt
        };
    }

    public async Task<List<TicketSummaryDto>> GetUserTicketsAsync(Guid userId, string tenantId)
    {
        var tickets = await _context.Tickets
            .Where(t => t.UserId == userId && t.TenantId == tenantId)
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        return tickets.Select(t => new TicketSummaryDto
        {
            Id = t.Id,
            Title = t.Title,
            Type = (TicketType)t.Type,
            Status = (TicketStatus)t.Status,
            Priority = (TicketPriority)t.Priority,
            UserName = t.UserName,
            ClinicName = t.ClinicName,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<List<TicketSummaryDto>> GetClinicTicketsAsync(Guid clinicId, string tenantId, bool isSystemOwner)
    {
        var query = _context.Tickets.AsQueryable();

        if (isSystemOwner)
        {
            query = query.Where(t => t.ClinicId == clinicId);
        }
        else
        {
            query = query.Where(t => t.ClinicId == clinicId && t.TenantId == tenantId);
        }

        var tickets = await query
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        return tickets.Select(t => new TicketSummaryDto
        {
            Id = t.Id,
            Title = t.Title,
            Type = (TicketType)t.Type,
            Status = (TicketStatus)t.Status,
            Priority = (TicketPriority)t.Priority,
            UserName = t.UserName,
            ClinicName = t.ClinicName,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<List<TicketSummaryDto>> GetAllTicketsAsync(
        TicketStatus? status,
        TicketType? type,
        Guid? clinicId,
        string? tenantId)
    {
        var query = _context.Tickets.AsQueryable();

        if (status.HasValue)
            query = query.Where(t => t.Status == (int)status.Value);

        if (type.HasValue)
            query = query.Where(t => t.Type == (int)type.Value);

        if (clinicId.HasValue)
            query = query.Where(t => t.ClinicId == clinicId.Value);

        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(t => t.TenantId == tenantId);

        var tickets = await query
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        return tickets.Select(t => new TicketSummaryDto
        {
            Id = t.Id,
            Title = t.Title,
            Type = (TicketType)t.Type,
            Status = (TicketStatus)t.Status,
            Priority = (TicketPriority)t.Priority,
            UserName = t.UserName,
            ClinicName = t.ClinicName,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<bool> UpdateTicketAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, bool isSystemOwner)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return false;

        // Only ticket owner or system owner can update
        if (!isSystemOwner && ticket.UserId != userId)
            return false;

        if (!string.IsNullOrEmpty(request.Title))
            ticket.Title = request.Title;

        if (!string.IsNullOrEmpty(request.Description))
            ticket.Description = request.Description;

        if (request.Type.HasValue)
            ticket.Type = (int)request.Type.Value;

        if (request.Priority.HasValue)
            ticket.Priority = (int)request.Priority.Value;

        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateTicketStatusAsync(
        Guid ticketId,
        UpdateTicketStatusRequest request,
        Guid userId,
        string userName,
        bool isSystemOwner)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return false;

        var oldStatus = ticket.Status;
        ticket.Status = (int)request.Status;
        ticket.UpdatedAt = DateTime.UtcNow;
        ticket.LastStatusChangeAt = DateTime.UtcNow;

        // Add history record
        var history = new TicketHistoryEntity
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            OldStatus = oldStatus,
            NewStatus = (int)request.Status,
            ChangedById = userId,
            ChangedByName = userName,
            Comment = request.Comment,
            ChangedAt = DateTime.UtcNow
        };

        _context.TicketHistory.Add(history);

        // Add comment if provided
        if (!string.IsNullOrEmpty(request.Comment))
        {
            var comment = new TicketCommentEntity
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                Comment = request.Comment,
                AuthorId = userId,
                AuthorName = userName,
                IsInternal = false,
                IsSystemOwner = isSystemOwner,
                CreatedAt = DateTime.UtcNow
            };
            _context.TicketComments.Add(comment);
        }

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AssignTicketAsync(Guid ticketId, AssignTicketRequest request, Guid systemOwnerId, string systemOwnerName)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket == null)
            return false;

        ticket.AssignedToId = request.AssignedToId;
        
        if (request.AssignedToId.HasValue)
        {
            var owner = await _context.Owners.FindAsync(request.AssignedToId.Value);
            ticket.AssignedToName = owner?.FullName;
        }
        else
        {
            ticket.AssignedToName = null;
        }

        ticket.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<Guid> AddCommentAsync(
        Guid ticketId,
        AddTicketCommentRequest request,
        Guid authorId,
        string authorName,
        bool isSystemOwner)
    {
        var comment = new TicketCommentEntity
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            Comment = request.Comment,
            AuthorId = authorId,
            AuthorName = authorName,
            IsInternal = request.IsInternal,
            IsSystemOwner = isSystemOwner,
            CreatedAt = DateTime.UtcNow
        };

        _context.TicketComments.Add(comment);

        // Update ticket's UpdatedAt
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return comment.Id;
    }

    public async Task<Guid> AddAttachmentAsync(Guid ticketId, UploadAttachmentRequest request)
    {
        // Decode base64 and save file (simplified - in production, use cloud storage)
        var fileBytes = Convert.FromBase64String(request.Base64Data);
        var fileUrl = $"/uploads/tickets/{ticketId}/{Guid.NewGuid()}_{request.FileName}";

        var attachment = new TicketAttachmentEntity
        {
            Id = Guid.NewGuid(),
            TicketId = ticketId,
            FileName = request.FileName,
            FileUrl = fileUrl,
            ContentType = request.ContentType,
            FileSize = fileBytes.Length,
            UploadedAt = DateTime.UtcNow
        };

        _context.TicketAttachments.Add(attachment);

        // Update ticket's UpdatedAt
        var ticket = await _context.Tickets.FindAsync(ticketId);
        if (ticket != null)
        {
            ticket.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();
        return attachment.Id;
    }

    public async Task<int> GetUnreadUpdatesCountAsync(Guid userId, string tenantId)
    {
        // For now, return count of tickets with recent updates (last 7 days)
        var sevenDaysAgo = DateTime.UtcNow.AddDays(-7);
        
        return await _context.Tickets
            .Where(t => t.UserId == userId && t.TenantId == tenantId && t.UpdatedAt > sevenDaysAgo)
            .CountAsync();
    }

    public async Task<bool> MarkTicketAsReadAsync(Guid ticketId, Guid userId)
    {
        // This is a simplified implementation
        // In production, you'd have a separate table to track read status per user
        return await Task.FromResult(true);
    }

    public async Task<TicketStatisticsDto> GetTicketStatisticsAsync(Guid? clinicId, string? tenantId)
    {
        var query = _context.Tickets.AsQueryable();

        if (clinicId.HasValue)
            query = query.Where(t => t.ClinicId == clinicId.Value);

        if (!string.IsNullOrEmpty(tenantId))
            query = query.Where(t => t.TenantId == tenantId);

        var tickets = await query.ToListAsync();

        var completedTickets = tickets.Where(t => t.Status == (int)TicketStatus.Completed).ToList();
        var avgResolutionTime = completedTickets.Any()
            ? completedTickets.Average(t => (t.LastStatusChangeAt ?? t.UpdatedAt).Subtract(t.CreatedAt).TotalHours)
            : 0;

        return new TicketStatisticsDto
        {
            TotalTickets = tickets.Count,
            OpenTickets = tickets.Count(t => t.Status == (int)TicketStatus.Open),
            InAnalysisTickets = tickets.Count(t => t.Status == (int)TicketStatus.InAnalysis),
            InProgressTickets = tickets.Count(t => t.Status == (int)TicketStatus.InProgress),
            BlockedTickets = tickets.Count(t => t.Status == (int)TicketStatus.Blocked),
            CompletedTickets = tickets.Count(t => t.Status == (int)TicketStatus.Completed),
            CancelledTickets = tickets.Count(t => t.Status == (int)TicketStatus.Cancelled),
            TicketsByType = tickets.GroupBy(t => ((TicketType)t.Type).ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            TicketsByPriority = tickets.GroupBy(t => ((TicketPriority)t.Priority).ToString())
                .ToDictionary(g => g.Key, g => g.Count()),
            TicketsByClinic = tickets.Where(t => t.ClinicName != null)
                .GroupBy(t => t.ClinicName!)
                .ToDictionary(g => g.Key, g => g.Count()),
            AverageResolutionTimeHours = avgResolutionTime
        };
    }
}
