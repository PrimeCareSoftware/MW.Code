using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class MessageReadReceipt : BaseEntity
    {
        [Required]
        public Guid MessageId { get; private set; }
        
        [ForeignKey(nameof(MessageId))]
        public virtual ChatMessage Message { get; private set; } = null!;
        
        [Required]
        public Guid UserId { get; private set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; private set; } = null!;
        
        public DateTime ReadAt { get; private set; }
        
        private MessageReadReceipt()
        {
            // EF Constructor
        }
        
        public MessageReadReceipt(Guid messageId, Guid userId, string tenantId) 
            : base(tenantId)
        {
            if (messageId == Guid.Empty)
                throw new ArgumentException("MessageId cannot be empty", nameof(messageId));
            
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
            MessageId = messageId;
            UserId = userId;
            ReadAt = DateTime.UtcNow;
        }
    }
}
