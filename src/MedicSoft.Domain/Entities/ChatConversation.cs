using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class ChatConversation : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; private set; }
        
        public ConversationType Type { get; private set; }
        
        public DateTime? LastMessageAt { get; private set; }
        
        public Guid? LastMessageId { get; private set; }
        
        [ForeignKey(nameof(LastMessageId))]
        public virtual ChatMessage? LastMessage { get; private set; }
        
        // Para grupos (opcional)
        public Guid? CreatedByUserId { get; private set; }
        
        [ForeignKey(nameof(CreatedByUserId))]
        public virtual User? CreatedBy { get; private set; }
        
        // Navigation properties
        private readonly List<ChatParticipant> _participants = new();
        public IReadOnlyCollection<ChatParticipant> Participants => _participants.AsReadOnly();
        
        private readonly List<ChatMessage> _messages = new();
        public IReadOnlyCollection<ChatMessage> Messages => _messages.AsReadOnly();
        
        private ChatConversation()
        {
            // EF Constructor
            Title = null!;
        }
        
        public ChatConversation(string title, ConversationType type, string tenantId, Guid? createdByUserId = null) 
            : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty", nameof(title));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
            Title = title.Trim();
            Type = type;
            CreatedByUserId = createdByUserId;
        }
        
        public void AddParticipant(ChatParticipant participant)
        {
            if (participant == null)
                throw new ArgumentNullException(nameof(participant));
            
            if (participant.TenantId != TenantId)
                throw new InvalidOperationException("Participant must belong to the same tenant");
            
            if (_participants.Any(p => p.UserId == participant.UserId && p.IsActive))
                throw new InvalidOperationException("User is already an active participant");
            
            _participants.Add(participant);
            UpdateTimestamp();
        }
        
        public void AddMessage(ChatMessage message)
        {
            if (message == null)
                throw new ArgumentNullException(nameof(message));
            
            if (message.TenantId != TenantId)
                throw new InvalidOperationException("Message must belong to the same tenant");
            
            _messages.Add(message);
            LastMessageAt = message.SentAt;
            LastMessageId = message.Id;
            UpdateTimestamp();
        }
        
        public void UpdateTitle(string newTitle)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title cannot be empty", nameof(newTitle));
            
            Title = newTitle.Trim();
            UpdateTimestamp();
        }
        
        public IEnumerable<ChatParticipant> GetActiveParticipants()
        {
            return _participants.Where(p => p.IsActive);
        }
    }
    
    public enum ConversationType
    {
        Direct = 0,
        Group = 1
    }
}
