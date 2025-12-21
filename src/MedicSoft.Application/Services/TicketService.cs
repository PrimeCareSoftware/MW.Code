using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IOwnerRepository _ownerRepository;

        public TicketService(ITicketRepository ticketRepository, IOwnerRepository ownerRepository)
        {
            _ticketRepository = ticketRepository;
            _ownerRepository = ownerRepository;
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
            var ticket = new Ticket(
                request.Title,
                request.Description,
                request.Type,
                request.Priority,
                userId,
                userName,
                userEmail,
                clinicId,
                clinicName,
                tenantId);

            await _ticketRepository.AddAsync(ticket);
            return ticket.Id;
        }

        public async Task<TicketDto?> GetTicketByIdAsync(Guid ticketId, Guid userId, bool isSystemOwner, string tenantId)
        {
            var ticket = await _ticketRepository.GetTicketWithDetailsAsync(ticketId, tenantId);
            if (ticket == null)
                return null;

            // Check permissions
            if (!isSystemOwner && ticket.UserId != userId)
                return null;

            return new TicketDto
            {
                Id = ticket.Id,
                Title = ticket.Title,
                Description = ticket.Description,
                Type = ticket.Type,
                Status = ticket.Status,
                Priority = ticket.Priority,
                UserId = ticket.UserId,
                UserName = ticket.UserName,
                UserEmail = ticket.UserEmail,
                ClinicId = ticket.ClinicId,
                ClinicName = ticket.ClinicName,
                TenantId = ticket.TenantId,
                AssignedToId = ticket.AssignedToId,
                AssignedToName = ticket.AssignedToName,
                Comments = ticket.Comments
                    .Where(c => !c.IsInternal || isSystemOwner)
                    .Select(c => new TicketCommentDto
                    {
                        Id = c.Id,
                        TicketId = c.TicketId,
                        Comment = c.Comment,
                        AuthorName = c.AuthorName,
                        IsInternal = c.IsInternal,
                        IsSystemOwner = c.IsSystemOwner,
                        CreatedAt = c.CreatedAt
                    }).ToList(),
                Attachments = ticket.Attachments.Select(a => new TicketAttachmentDto
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
                UpdatedAt = ticket.UpdatedAt ?? ticket.CreatedAt,
                LastStatusChangeAt = ticket.LastStatusChangeAt
            };
        }

        public async Task<List<TicketSummaryDto>> GetUserTicketsAsync(Guid userId, string tenantId)
        {
            var tickets = await _ticketRepository.GetUserTicketsAsync(userId, tenantId);

            return tickets.Select(t => new TicketSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = t.Type,
                Status = t.Status,
                Priority = t.Priority,
                UserName = t.UserName,
                ClinicName = t.ClinicName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt ?? t.CreatedAt
            }).ToList();
        }

        public async Task<List<TicketSummaryDto>> GetClinicTicketsAsync(Guid clinicId, string tenantId, bool isSystemOwner)
        {
            var tickets = await _ticketRepository.GetClinicTicketsAsync(clinicId, tenantId);

            return tickets.Select(t => new TicketSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = t.Type,
                Status = t.Status,
                Priority = t.Priority,
                UserName = t.UserName,
                ClinicName = t.ClinicName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt ?? t.CreatedAt
            }).ToList();
        }

        public async Task<List<TicketSummaryDto>> GetAllTicketsAsync(
            TicketStatus? status,
            TicketType? type,
            Guid? clinicId,
            string? tenantId)
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync(status, type, clinicId, tenantId);

            return tickets.Select(t => new TicketSummaryDto
            {
                Id = t.Id,
                Title = t.Title,
                Type = t.Type,
                Status = t.Status,
                Priority = t.Priority,
                UserName = t.UserName,
                ClinicName = t.ClinicName,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt ?? t.CreatedAt
            }).ToList();
        }

        public async Task<bool> UpdateTicketAsync(Guid ticketId, UpdateTicketRequest request, Guid userId, bool isSystemOwner, string tenantId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, tenantId);
            if (ticket == null)
                return false;

            // Only ticket owner or system owner can update
            if (!isSystemOwner && ticket.UserId != userId)
                return false;

            ticket.Update(request.Title, request.Description, request.Type, request.Priority);
            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }

        public async Task<bool> UpdateTicketStatusAsync(
            Guid ticketId,
            UpdateTicketStatusRequest request,
            Guid userId,
            string userName,
            bool isSystemOwner,
            string tenantId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, tenantId);
            if (ticket == null)
                return false;

            ticket.UpdateStatus(request.Status, userId, userName, request.Comment);

            // Add comment if provided
            if (!string.IsNullOrEmpty(request.Comment))
            {
                ticket.AddComment(userId, userName, request.Comment, false, isSystemOwner);
            }

            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }

        public async Task<bool> AssignTicketAsync(Guid ticketId, AssignTicketRequest request, Guid systemOwnerId, string systemOwnerName, string tenantId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, tenantId);
            if (ticket == null)
                return false;

            string? assignedToName = null;
            if (request.AssignedToId.HasValue)
            {
                var owner = await _ownerRepository.GetByIdAsync(request.AssignedToId.Value, tenantId);
                assignedToName = owner?.FullName;
            }

            ticket.AssignTo(request.AssignedToId, assignedToName);
            await _ticketRepository.UpdateAsync(ticket);
            return true;
        }

        public async Task<Guid> AddCommentAsync(
            Guid ticketId,
            AddTicketCommentRequest request,
            Guid authorId,
            string authorName,
            bool isSystemOwner,
            string tenantId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, tenantId);
            if (ticket == null)
                throw new InvalidOperationException("Ticket not found");

            ticket.AddComment(authorId, authorName, request.Comment, request.IsInternal, isSystemOwner);
            await _ticketRepository.UpdateAsync(ticket);

            // Return the newly created comment ID
            var comments = await _ticketRepository.GetTicketCommentsAsync(ticketId, tenantId, true);
            var lastComment = comments.OrderByDescending(c => c.CreatedAt).FirstOrDefault();
            return lastComment?.Id ?? Guid.Empty;
        }

        public async Task<Guid> AddAttachmentAsync(Guid ticketId, UploadAttachmentRequest request, string tenantId)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId, tenantId);
            if (ticket == null)
                throw new InvalidOperationException("Ticket not found");

            // Validate file size (max 5MB)
            const int maxFileSizeBytes = 5 * 1024 * 1024;

            // Validate content type
            var allowedContentTypes = new[] { "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp" };
            if (!allowedContentTypes.Contains(request.ContentType.ToLower()))
            {
                throw new InvalidOperationException("Tipo de arquivo não permitido. Apenas imagens são aceitas.");
            }

            // Decode base64 and validate size
            byte[] fileBytes;
            try
            {
                fileBytes = Convert.FromBase64String(request.Base64Data);
            }
            catch (FormatException)
            {
                throw new InvalidOperationException("Dados de arquivo inválidos.");
            }

            if (fileBytes.Length > maxFileSizeBytes)
            {
                throw new InvalidOperationException($"Arquivo muito grande. Tamanho máximo: {maxFileSizeBytes / 1024 / 1024}MB");
            }

            // Validate file extension
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var fileExtension = Path.GetExtension(request.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException("Extensão de arquivo não permitida.");
            }

            // TODO: In production, upload to cloud storage (AWS S3, Azure Blob, etc.)
            var fileUrl = $"/uploads/tickets/{ticketId}/{Guid.NewGuid()}{fileExtension}";

            ticket.AddAttachment(request.FileName, fileUrl, request.ContentType, fileBytes.Length);
            await _ticketRepository.UpdateAsync(ticket);

            // Return the newly created attachment ID
            var attachments = await _ticketRepository.GetTicketAttachmentsAsync(ticketId, tenantId);
            var lastAttachment = attachments.OrderByDescending(a => a.UploadedAt).FirstOrDefault();
            return lastAttachment?.Id ?? Guid.Empty;
        }

        public async Task<int> GetUnreadUpdatesCountAsync(Guid userId, string tenantId)
        {
            // TODO: Implement proper read tracking with a separate table
            // For now, return 0 to avoid confusion
            return await Task.FromResult(0);
        }

        public async Task<bool> MarkTicketAsReadAsync(Guid ticketId, Guid userId, string tenantId)
        {
            // TODO: Implement proper read tracking
            return await Task.FromResult(true);
        }

        public async Task<TicketStatisticsDto> GetTicketStatisticsAsync(Guid? clinicId, string? tenantId)
        {
            var tickets = await _ticketRepository.GetAllTicketsAsync(null, null, clinicId, tenantId);
            var ticketList = tickets.ToList();

            var completedTickets = ticketList.Where(t => t.Status == TicketStatus.Completed).ToList();
            var avgResolutionTime = completedTickets.Any()
                ? completedTickets.Average(t => (t.LastStatusChangeAt ?? t.UpdatedAt ?? t.CreatedAt).Subtract(t.CreatedAt).TotalHours)
                : 0;

            return new TicketStatisticsDto
            {
                TotalTickets = ticketList.Count,
                OpenTickets = ticketList.Count(t => t.Status == TicketStatus.Open),
                InAnalysisTickets = ticketList.Count(t => t.Status == TicketStatus.InAnalysis),
                InProgressTickets = ticketList.Count(t => t.Status == TicketStatus.InProgress),
                BlockedTickets = ticketList.Count(t => t.Status == TicketStatus.Blocked),
                CompletedTickets = ticketList.Count(t => t.Status == TicketStatus.Completed),
                CancelledTickets = ticketList.Count(t => t.Status == TicketStatus.Cancelled),
                TicketsByType = ticketList.GroupBy(t => t.Type.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                TicketsByPriority = ticketList.GroupBy(t => t.Priority.ToString())
                    .ToDictionary(g => g.Key, g => g.Count()),
                TicketsByClinic = ticketList.Where(t => t.ClinicName != null)
                    .GroupBy(t => t.ClinicName!)
                    .ToDictionary(g => g.Key, g => g.Count()),
                AverageResolutionTimeHours = avgResolutionTime
            };
        }
    }
}
