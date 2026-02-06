using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class ChatParticipant : BaseEntity
    {
        [Required]
        public Guid ConversationId { get; private set; }
        
        [ForeignKey(nameof(ConversationId))]
        public virtual ChatConversation Conversation { get; private set; } = null!;
        
        [Required]
        public Guid UserId { get; private set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; private set; } = null!;
        
        public DateTime JoinedAt { get; private set; }
        public DateTime? LeftAt { get; private set; }
        public bool IsActive { get; private set; }
        
        public int UnreadCount { get; private set; }
        
        public Guid? LastReadMessageId { get; private set; }
        public DateTime? LastReadAt { get; private set; }
        
        public bool IsMuted { get; private set; }
        
        public ParticipantRole Role { get; private set; }
        
        private ChatParticipant()
        {
            // EF Constructor
        }
        
        public ChatParticipant(Guid conversationId, Guid userId, string tenantId, ParticipantRole role = ParticipantRole.Member) 
            : base(tenantId)
        {
            if (conversationId == Guid.Empty)
                throw new ArgumentException("ConversationId cannot be empty", nameof(conversationId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
            ConversationId = conversationId;
            UserId = userId;
            JoinedAt = DateTime.UtcNow;
            IsActive = true;
            UnreadCount = 0;
            IsMuted = false;
            Role = role;
        }
        
        public void Leave()
        {
            if (!IsActive)
                throw new InvalidOperationException("Participant has already left");
            
            IsActive = false;
            LeftAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void MarkAsRead(Guid messageId)
        {
            if (!IsActive)
                throw new InvalidOperationException("Cannot mark as read for inactive participant");
            
            if (messageId == Guid.Empty)
                throw new ArgumentException("MessageId cannot be empty", nameof(messageId));
            
            LastReadMessageId = messageId;
            LastReadAt = DateTime.UtcNow;
            UnreadCount = 0;
            UpdateTimestamp();
        }
        
        public void IncrementUnreadCount()
        {
            if (!IsActive)
                return;
            
            UnreadCount++;
            UpdateTimestamp();
        }
        
        public void ResetUnreadCount()
        {
            UnreadCount = 0;
            UpdateTimestamp();
        }
        
        public void SetMuted(bool muted)
        {
            IsMuted = muted;
            UpdateTimestamp();
        }
        
        public void UpdateRole(ParticipantRole newRole)
        {
            Role = newRole;
            UpdateTimestamp();
        }
    }
    
    public enum ParticipantRole
    {
        Member = 0,
        Admin = 1
    }
}
