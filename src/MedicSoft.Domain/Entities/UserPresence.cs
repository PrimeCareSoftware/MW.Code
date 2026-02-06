using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class UserPresence : BaseEntity
    {
        [Required]
        public Guid UserId { get; private set; }
        
        [ForeignKey(nameof(UserId))]
        public virtual User User { get; private set; } = null!;
        
        public PresenceStatus Status { get; private set; }
        
        public DateTime LastSeenAt { get; private set; }
        
        [MaxLength(450)]
        public string? ConnectionId { get; private set; }
        
        public bool IsOnline { get; private set; }
        
        [MaxLength(200)]
        public string? StatusMessage { get; private set; }
        
        private UserPresence()
        {
            // EF Constructor
        }
        
        public UserPresence(Guid userId, string tenantId) 
            : base(tenantId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId cannot be empty", nameof(tenantId));
            
            UserId = userId;
            Status = PresenceStatus.Offline;
            LastSeenAt = DateTime.UtcNow;
            IsOnline = false;
        }
        
        public void SetOnline(string connectionId)
        {
            if (string.IsNullOrWhiteSpace(connectionId))
                throw new ArgumentException("ConnectionId cannot be empty", nameof(connectionId));
            
            Status = PresenceStatus.Online;
            ConnectionId = connectionId;
            IsOnline = true;
            LastSeenAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void SetOffline()
        {
            Status = PresenceStatus.Offline;
            ConnectionId = null;
            IsOnline = false;
            LastSeenAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void UpdateStatus(PresenceStatus newStatus, string? statusMessage = null)
        {
            Status = newStatus;
            
            if (statusMessage != null)
            {
                if (statusMessage.Length > 200)
                    throw new ArgumentException("Status message cannot exceed 200 characters", nameof(statusMessage));
                
                StatusMessage = statusMessage.Trim();
            }
            
            LastSeenAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
        
        public void ClearStatusMessage()
        {
            StatusMessage = null;
            UpdateTimestamp();
        }
    }
    
    public enum PresenceStatus
    {
        Online = 0,
        Away = 1,
        Busy = 2,
        Offline = 3
    }
}
