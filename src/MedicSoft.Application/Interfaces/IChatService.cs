using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Interfaces
{
    public interface IChatService
    {
        // Conversations
        Task<ConversationDto> CreateDirectConversationAsync(Guid user1Id, Guid user2Id, string tenantId);
        Task<ConversationDto?> GetConversationAsync(Guid conversationId, string tenantId);
        Task<ConversationListDto> GetUserConversationsAsync(Guid userId, string tenantId);
        Task<ConversationDto?> FindDirectConversationAsync(Guid user1Id, Guid user2Id, string tenantId);
        
        // Messages
        Task<ChatMessageDto> SendMessageAsync(Guid conversationId, Guid senderId, string content, string tenantId, Guid? replyToMessageId = null);
        Task<ChatMessageDto?> GetMessageAsync(Guid messageId, string tenantId);
        Task<MessageListDto> GetConversationMessagesAsync(Guid conversationId, string tenantId, int page = 1, int pageSize = 50);
        Task<MessageListDto> SearchMessagesAsync(Guid conversationId, string query, string tenantId);
        Task<ChatMessageDto> EditMessageAsync(Guid messageId, string newContent, Guid userId, string tenantId);
        Task DeleteMessageAsync(Guid messageId, Guid userId, string tenantId);
        
        // Participants
        Task<List<ChatParticipantDto>> GetConversationParticipantsAsync(Guid conversationId, string tenantId);
        Task<ChatParticipantDto> AddParticipantAsync(Guid conversationId, Guid userId, string tenantId);
        Task RemoveParticipantAsync(Guid conversationId, Guid userId, string tenantId);
        
        // Read Receipts
        Task MarkMessageAsReadAsync(Guid conversationId, Guid messageId, Guid userId, string tenantId);
        Task<int> GetUnreadCountAsync(Guid conversationId, Guid userId, string tenantId);
        
        // Users
        Task<List<UserPresenceDto>> GetClinicUsersAsync(string tenantId);
    }
}
