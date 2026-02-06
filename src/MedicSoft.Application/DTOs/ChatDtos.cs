using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class ChatMessageDto
    {
        public Guid Id { get; set; }
        public Guid ConversationId { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Type { get; set; } = "Text";
        public DateTime SentAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsDeleted { get; set; }
        public List<Guid> ReadBy { get; set; } = new();
        public Guid? ReplyToMessageId { get; set; }
    }

    public class ConversationDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "Direct";
        public DateTime CreatedAt { get; set; }
        public DateTime? LastMessageAt { get; set; }
        public ChatMessageDto? LastMessage { get; set; }
        public List<ChatParticipantDto> Participants { get; set; } = new();
        public int UnreadCount { get; set; }
    }

    public class SendMessageDto
    {
        public Guid ConversationId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid? ReplyToMessageId { get; set; }
    }

    public class CreateDirectConversationDto
    {
        public Guid OtherUserId { get; set; }
    }

    public class ChatParticipantDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
        public int UnreadCount { get; set; }
        public bool IsMuted { get; set; }
        public string Role { get; set; } = "Member";
    }

    public class UserPresenceDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = "Offline";
        public DateTime LastSeenAt { get; set; }
        public bool IsOnline { get; set; }
        public string? StatusMessage { get; set; }
    }

    public class MarkAsReadDto
    {
        public Guid ConversationId { get; set; }
        public Guid MessageId { get; set; }
    }

    public class SearchMessagesDto
    {
        public Guid ConversationId { get; set; }
        public string Query { get; set; } = string.Empty;
    }

    public class ConversationListDto
    {
        public List<ConversationDto> Conversations { get; set; } = new();
        public int TotalCount { get; set; }
    }

    public class MessageListDto
    {
        public List<ChatMessageDto> Messages { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public bool HasMore { get; set; }
    }
}
