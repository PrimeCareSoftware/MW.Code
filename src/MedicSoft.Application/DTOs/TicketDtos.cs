using System;
using System.Collections.Generic;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class TicketDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public string? ClinicName { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public Guid? AssignedToId { get; set; }
        public string? AssignedToName { get; set; }
        public List<TicketAttachmentDto> Attachments { get; set; } = new();
        public List<TicketCommentDto> Comments { get; set; } = new();
        public int UnreadUpdates { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastStatusChangeAt { get; set; }
    }

    public class TicketSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public TicketType Type { get; set; }
        public TicketStatus Status { get; set; }
        public TicketPriority Priority { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? ClinicName { get; set; }
        public int UnreadUpdates { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class CreateTicketRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TicketType Type { get; set; }
        public TicketPriority Priority { get; set; } = TicketPriority.Medium;
    }

    public class UpdateTicketRequest
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public TicketType? Type { get; set; }
        public TicketPriority? Priority { get; set; }
    }

    public class UpdateTicketStatusRequest
    {
        public TicketStatus Status { get; set; }
        public string? Comment { get; set; }
    }

    public class AddTicketCommentRequest
    {
        public string Comment { get; set; } = string.Empty;
        public bool IsInternal { get; set; } = false;
    }

    public class TicketCommentDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string Comment { get; set; } = string.Empty;
        public string AuthorName { get; set; } = string.Empty;
        public bool IsInternal { get; set; }
        public bool IsSystemOwner { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class TicketAttachmentDto
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public DateTime UploadedAt { get; set; }
    }

    public class UploadAttachmentRequest
    {
        public string FileName { get; set; } = string.Empty;
        public string Base64Data { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class TicketStatisticsDto
    {
        public int TotalTickets { get; set; }
        public int OpenTickets { get; set; }
        public int InAnalysisTickets { get; set; }
        public int InProgressTickets { get; set; }
        public int BlockedTickets { get; set; }
        public int CompletedTickets { get; set; }
        public int CancelledTickets { get; set; }
        public Dictionary<string, int> TicketsByType { get; set; } = new();
        public Dictionary<string, int> TicketsByPriority { get; set; } = new();
        public Dictionary<string, int> TicketsByClinic { get; set; } = new();
        public double AverageResolutionTimeHours { get; set; }
    }

    public class AssignTicketRequest
    {
        public Guid? AssignedToId { get; set; }
    }
}
