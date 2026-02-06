using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Services
{
    public class ChatService : IChatService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<ChatService> _logger;

        public ChatService(MedicSoftDbContext context, ILogger<ChatService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ConversationDto> CreateDirectConversationAsync(Guid user1Id, Guid user2Id, string tenantId)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));

            // Check if conversation already exists
            var existingConversation = await FindDirectConversationAsync(user1Id, user2Id, tenantId);
            if (existingConversation != null)
                return existingConversation;

            // Get users
            var user1 = await _context.Users.FirstOrDefaultAsync(u => u.Id == user1Id && u.TenantId == tenantId);
            var user2 = await _context.Users.FirstOrDefaultAsync(u => u.Id == user2Id && u.TenantId == tenantId);

            if (user1 == null || user2 == null)
                throw new InvalidOperationException("One or both users not found in the tenant");

            // Create conversation
            var title = $"{user1.FullName} - {user2.FullName}";
            var conversation = new ChatConversation(title, ConversationType.Direct, tenantId, user1Id);
            _context.ChatConversations.Add(conversation);

            // Add participants
            var participant1 = new ChatParticipant(conversation.Id, user1Id, tenantId);
            var participant2 = new ChatParticipant(conversation.Id, user2Id, tenantId);

            conversation.AddParticipant(participant1);
            conversation.AddParticipant(participant2);

            _context.ChatParticipants.Add(participant1);
            _context.ChatParticipants.Add(participant2);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created direct conversation {ConversationId} between users {User1Id} and {User2Id} in tenant {TenantId}", 
                conversation.Id, user1Id, user2Id, tenantId);

            return await GetConversationAsync(conversation.Id, tenantId) 
                ?? throw new InvalidOperationException("Failed to retrieve created conversation");
        }

        public async Task<ConversationDto?> FindDirectConversationAsync(Guid user1Id, Guid user2Id, string tenantId)
        {
            var conversation = await _context.ChatConversations
                .Where(c => c.TenantId == tenantId && c.Type == ConversationType.Direct)
                .Where(c => c.Participants.Count(p => p.IsActive && (p.UserId == user1Id || p.UserId == user2Id)) == 2)
                .Include(c => c.Participants.Where(p => p.IsActive))
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync();

            return conversation != null ? MapToConversationDto(conversation, user1Id) : null;
        }

        public async Task<ConversationDto?> GetConversationAsync(Guid conversationId, string tenantId)
        {
            var conversation = await _context.ChatConversations
                .Where(c => c.Id == conversationId && c.TenantId == tenantId)
                .Include(c => c.Participants.Where(p => p.IsActive))
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m.Sender)
                .FirstOrDefaultAsync();

            return conversation != null ? MapToConversationDto(conversation, Guid.Empty) : null;
        }

        public async Task<ConversationListDto> GetUserConversationsAsync(Guid userId, string tenantId)
        {
            var conversations = await _context.ChatParticipants
                .Where(p => p.UserId == userId && p.TenantId == tenantId && p.IsActive)
                .Select(p => p.Conversation)
                .Include(c => c.Participants.Where(p => p.IsActive))
                    .ThenInclude(p => p.User)
                .Include(c => c.LastMessage)
                    .ThenInclude(m => m.Sender)
                .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
                .ToListAsync();

            return new ConversationListDto
            {
                Conversations = conversations.Select(c => MapToConversationDto(c, userId)).ToList(),
                TotalCount = conversations.Count
            };
        }

        public async Task<ChatMessageDto> SendMessageAsync(Guid conversationId, Guid senderId, string content, string tenantId, Guid? replyToMessageId = null)
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));

            // Validate conversation and participant
            var conversation = await _context.ChatConversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.TenantId == tenantId);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found");

            var participant = conversation.Participants.FirstOrDefault(p => p.UserId == senderId && p.IsActive);
            if (participant == null)
                throw new InvalidOperationException("User is not a participant of this conversation");

            // Create message
            var message = new ChatMessage(conversationId, senderId, content, tenantId, MessageType.Text, replyToMessageId);
            _context.ChatMessages.Add(message);

            // Update conversation
            conversation.AddMessage(message);

            // Update unread counts for other participants
            var otherParticipants = conversation.Participants.Where(p => p.UserId != senderId && p.IsActive);
            foreach (var p in otherParticipants)
            {
                p.IncrementUnreadCount();
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} sent message {MessageId} in conversation {ConversationId}", 
                senderId, message.Id, conversationId);

            return await GetMessageDtoAsync(message.Id, tenantId) 
                ?? throw new InvalidOperationException("Failed to retrieve sent message");
        }

        public async Task<ChatMessageDto?> GetMessageAsync(Guid messageId, string tenantId)
        {
            return await GetMessageDtoAsync(messageId, tenantId);
        }

        public async Task<MessageListDto> GetConversationMessagesAsync(Guid conversationId, string tenantId, int page = 1, int pageSize = 50)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 50;

            var query = _context.ChatMessages
                .Where(m => m.ConversationId == conversationId && m.TenantId == tenantId && !m.IsDeleted)
                .Include(m => m.Sender)
                .Include(m => m.ReadReceipts)
                .OrderByDescending(m => m.SentAt);

            var totalCount = await query.CountAsync();
            var messages = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new MessageListDto
            {
                Messages = messages.Select(MapToMessageDto).Reverse().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                HasMore = totalCount > (page * pageSize)
            };
        }

        public async Task<MessageListDto> SearchMessagesAsync(Guid conversationId, string query, string tenantId)
        {
            var messages = await _context.ChatMessages
                .Where(m => m.ConversationId == conversationId && 
                           m.TenantId == tenantId && 
                           !m.IsDeleted &&
                           m.Content.Contains(query))
                .Include(m => m.Sender)
                .Include(m => m.ReadReceipts)
                .OrderByDescending(m => m.SentAt)
                .Take(100)
                .ToListAsync();

            return new MessageListDto
            {
                Messages = messages.Select(MapToMessageDto).ToList(),
                TotalCount = messages.Count,
                Page = 1,
                PageSize = 100,
                HasMore = false
            };
        }

        public async Task<ChatMessageDto> EditMessageAsync(Guid messageId, string newContent, Guid userId, string tenantId)
        {
            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.TenantId == tenantId);

            if (message == null)
                throw new InvalidOperationException("Message not found");

            if (message.SenderId != userId)
                throw new InvalidOperationException("Only the sender can edit the message");

            message.EditContent(newContent);
            await _context.SaveChangesAsync();

            return await GetMessageDtoAsync(messageId, tenantId) 
                ?? throw new InvalidOperationException("Failed to retrieve edited message");
        }

        public async Task DeleteMessageAsync(Guid messageId, Guid userId, string tenantId)
        {
            var message = await _context.ChatMessages
                .FirstOrDefaultAsync(m => m.Id == messageId && m.TenantId == tenantId);

            if (message == null)
                throw new InvalidOperationException("Message not found");

            if (message.SenderId != userId)
                throw new InvalidOperationException("Only the sender can delete the message");

            message.Delete();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Message {MessageId} deleted by user {UserId}", messageId, userId);
        }

        public async Task<List<ChatParticipantDto>> GetConversationParticipantsAsync(Guid conversationId, string tenantId)
        {
            var participants = await _context.ChatParticipants
                .Where(p => p.ConversationId == conversationId && p.TenantId == tenantId && p.IsActive)
                .Include(p => p.User)
                .ToListAsync();

            return participants.Select(MapToParticipantDto).ToList();
        }

        public async Task<ChatParticipantDto> AddParticipantAsync(Guid conversationId, Guid userId, string tenantId)
        {
            var conversation = await _context.ChatConversations
                .Include(c => c.Participants)
                .FirstOrDefaultAsync(c => c.Id == conversationId && c.TenantId == tenantId);

            if (conversation == null)
                throw new InvalidOperationException("Conversation not found");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId && u.TenantId == tenantId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var participant = new ChatParticipant(conversationId, userId, tenantId);
            conversation.AddParticipant(participant);
            _context.ChatParticipants.Add(participant);
            await _context.SaveChangesAsync();

            return MapToParticipantDto(participant, user);
        }

        public async Task RemoveParticipantAsync(Guid conversationId, Guid userId, string tenantId)
        {
            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && 
                                         p.UserId == userId && 
                                         p.TenantId == tenantId &&
                                         p.IsActive);

            if (participant == null)
                throw new InvalidOperationException("Participant not found");

            participant.Leave();
            await _context.SaveChangesAsync();

            _logger.LogInformation("User {UserId} left conversation {ConversationId}", userId, conversationId);
        }

        public async Task MarkMessageAsReadAsync(Guid conversationId, Guid messageId, Guid userId, string tenantId)
        {
            var message = await _context.ChatMessages
                .Include(m => m.ReadReceipts)
                .FirstOrDefaultAsync(m => m.Id == messageId && 
                                         m.ConversationId == conversationId && 
                                         m.TenantId == tenantId);

            if (message == null)
                throw new InvalidOperationException("Message not found");

            // Don't mark own messages as read
            if (message.SenderId == userId)
                return;

            // Check if already read
            if (message.HasBeenReadBy(userId))
                return;

            // Create read receipt
            var readReceipt = new MessageReadReceipt(messageId, userId, tenantId);
            message.AddReadReceipt(readReceipt);
            _context.MessageReadReceipts.Add(readReceipt);

            // Update participant
            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && 
                                         p.UserId == userId && 
                                         p.TenantId == tenantId &&
                                         p.IsActive);

            if (participant != null)
            {
                participant.MarkAsRead(messageId);
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, string tenantId)
        {
            var participant = await _context.ChatParticipants
                .FirstOrDefaultAsync(p => p.ConversationId == conversationId && 
                                         p.UserId == userId && 
                                         p.TenantId == tenantId &&
                                         p.IsActive);

            return participant?.UnreadCount ?? 0;
        }

        public async Task<List<UserPresenceDto>> GetClinicUsersAsync(string tenantId)
        {
            var users = await _context.Users
                .Where(u => u.TenantId == tenantId && u.IsActive)
                .OrderBy(u => u.FullName)
                .ToListAsync();

            var userPresences = await _context.UserPresences
                .Where(up => up.TenantId == tenantId)
                .ToListAsync();

            return users.Select(u =>
            {
                var presence = userPresences.FirstOrDefault(up => up.UserId == u.Id);
                return new UserPresenceDto
                {
                    UserId = u.Id,
                    UserName = u.Username,
                    FullName = u.FullName,
                    Status = presence?.Status.ToString() ?? "Offline",
                    LastSeenAt = presence?.LastSeenAt ?? u.LastLoginAt ?? DateTime.UtcNow,
                    IsOnline = presence?.IsOnline ?? false,
                    StatusMessage = presence?.StatusMessage
                };
            }).ToList();
        }

        // Helper methods
        private async Task<ChatMessageDto?> GetMessageDtoAsync(Guid messageId, string tenantId)
        {
            var message = await _context.ChatMessages
                .Where(m => m.Id == messageId && m.TenantId == tenantId)
                .Include(m => m.Sender)
                .Include(m => m.ReadReceipts)
                .FirstOrDefaultAsync();

            return message != null ? MapToMessageDto(message) : null;
        }

        private ConversationDto MapToConversationDto(ChatConversation conversation, Guid currentUserId)
        {
            var participants = conversation.Participants
                .Where(p => p.IsActive)
                .Select(p => MapToParticipantDto(p))
                .ToList();

            var currentParticipant = conversation.Participants.FirstOrDefault(p => p.UserId == currentUserId && p.IsActive);

            return new ConversationDto
            {
                Id = conversation.Id,
                Title = conversation.Title,
                Type = conversation.Type.ToString(),
                CreatedAt = conversation.CreatedAt,
                LastMessageAt = conversation.LastMessageAt,
                LastMessage = conversation.LastMessage != null ? MapToMessageDto(conversation.LastMessage) : null,
                Participants = participants,
                UnreadCount = currentParticipant?.UnreadCount ?? 0
            };
        }

        private ChatMessageDto MapToMessageDto(ChatMessage message)
        {
            return new ChatMessageDto
            {
                Id = message.Id,
                ConversationId = message.ConversationId,
                SenderId = message.SenderId,
                SenderName = message.Sender?.FullName ?? "Unknown",
                Content = message.IsDeleted ? "[Mensagem deletada]" : message.Content,
                Type = message.Type.ToString(),
                SentAt = message.SentAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsDeleted = message.IsDeleted,
                ReadBy = message.ReadReceipts.Select(r => r.UserId).ToList(),
                ReplyToMessageId = message.ReplyToMessageId
            };
        }

        private ChatParticipantDto MapToParticipantDto(ChatParticipant participant)
        {
            return MapToParticipantDto(participant, participant.User);
        }

        private ChatParticipantDto MapToParticipantDto(ChatParticipant participant, User user)
        {
            return new ChatParticipantDto
            {
                Id = participant.Id,
                UserId = participant.UserId,
                UserName = user.Username,
                FullName = user.FullName,
                JoinedAt = participant.JoinedAt,
                IsActive = participant.IsActive,
                UnreadCount = participant.UnreadCount,
                IsMuted = participant.IsMuted,
                Role = participant.Role.ToString()
            };
        }
    }
}
