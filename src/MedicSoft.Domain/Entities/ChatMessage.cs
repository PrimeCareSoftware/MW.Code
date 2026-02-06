using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        [Required]
        public Guid ConversationId { get; private set; }
        
        [ForeignKey(nameof(ConversationId))]
        public virtual ChatConversation Conversation { get; private set; } = null!;
        
        [Required]
        public Guid SenderId { get; private set; }
        
        [ForeignKey(nameof(SenderId))]
        public virtual User Sender { get; private set; } = null!;
        
        [Required]
        [MaxLength(5000)]
        public string Content { get; private set; }
        
        public MessageType Type { get; private set; }
        
        public DateTime SentAt { get; private set; }
        
        public bool IsEdited { get; private set; }
        public DateTime? EditedAt { get; private set; }
        
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedAt { get; private set; }
        
        // Opcional: Reply
        public Guid? ReplyToMessageId { get; private set; }
        
        [ForeignKey(nameof(ReplyToMessageId))]
        public virtual ChatMessage? ReplyToMessage { get; private set; }
        
        // Navigation properties
        private readonly List<MessageReadReceipt> _readReceipts = new();
        public IReadOnlyCollection<MessageReadReceipt> ReadReceipts => _readReceipts.AsReadOnly();
        
        private ChatMessage()
        {
            // EF Constructor
            Content = null!;
        }
        
        public ChatMessage(Guid conversationId, Guid senderId, string content, string tenantId, MessageType type = MessageType.Text, Guid? replyToMessageId = null) 
            : base(tenantId)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("ConversationId cannot be empty", nameof(conversationId));
            
            if (senderId == Guid.Empty)
                throw new ArgumentException("SenderId cannot be empty", nameof(senderId));
            
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            
            if (content.Length > 5000)
                throw new ArgumentException("Content cannot exceed 5000 characters", nameof(content));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
            ConversationId = conversationId;
            SenderId = senderId;
            Content = content.Trim();
            Type = type;
            SentAt = DateTime.UtcNow;
            ReplyToMessageId = replyToMessageId;
            IsEdited = false;
            IsDeleted = false;
        }
        
        public void EditContent(string newContent)
        {
            if (IsDeleted)
                throw new InvalidOperationException("Cannot edit a deleted message");
            
            if (string.IsNullOrWhiteSpace(newContent))
                throw new ArgumentException("Content cannot be empty", nameof(newContent));
            
            if (newContent.Length > 5000)
                throw new ArgumentException("Content cannot exceed 5000 characters", nameof(newContent));
            
            Content = newContent.Trim();
            IsEdited = true;
            EditedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void Delete()
        {
            if (IsDeleted)
                throw new InvalidOperationException("Message is already deleted");
            
            IsDeleted = true;
            DeletedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void AddReadReceipt(MessageReadReceipt readReceipt)
        {
            if (readReceipt == null)
                throw new ArgumentNullException(nameof(readReceipt));
            
            if (readReceipt.TenantId != TenantId)
                throw new InvalidOperationException("Read receipt must belong to the same tenant");
            
            if (_readReceipts.Any(r => r.UserId == readReceipt.UserId))
                throw new InvalidOperationException("User has already read this message");
            
            _readReceipts.Add(readReceipt);
            UpdateTimestamp();
        }
        
        public bool HasBeenReadBy(Guid userId)
        {
            return _readReceipts.Any(r => r.UserId == userId);
        }
        
        public IEnumerable<Guid> GetReadByUserIds()
        {
            return _readReceipts.Select(r => r.UserId);
        }
    }
    
    public enum MessageType
    {
        Text = 0,
        Image = 1,
        File = 2,
        System = 3
    }
}
