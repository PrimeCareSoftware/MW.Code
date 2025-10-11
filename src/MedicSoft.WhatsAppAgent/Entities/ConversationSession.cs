using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.WhatsAppAgent.Entities
{
    /// <summary>
    /// Represents a WhatsApp conversation session with a user
    /// Tracks conversation history and rate limiting
    /// </summary>
    public class ConversationSession
    {
        public Guid Id { get; private set; }
        
        [Required]
        public Guid ConfigurationId { get; private set; }
        
        [Required]
        [MaxLength(100)]
        public string TenantId { get; private set; }
        
        [Required]
        [MaxLength(20)]
        public string UserPhoneNumber { get; private set; }
        
        [MaxLength(200)]
        public string UserName { get; private set; }
        
        /// <summary>
        /// Conversation context in JSON format
        /// </summary>
        [MaxLength(8000)]
        public string Context { get; private set; }
        
        /// <summary>
        /// Current state of the conversation
        /// </summary>
        [MaxLength(50)]
        public string State { get; private set; }
        
        /// <summary>
        /// Number of messages in current hour
        /// </summary>
        public int MessageCountCurrentHour { get; private set; }
        
        /// <summary>
        /// Last message timestamp for rate limiting
        /// </summary>
        public DateTime LastMessageAt { get; private set; }
        
        /// <summary>
        /// Hour window start for rate limiting
        /// </summary>
        public DateTime CurrentHourStart { get; private set; }
        
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }
        
        /// <summary>
        /// Session expires after inactivity
        /// </summary>
        public DateTime ExpiresAt { get; private set; }
        
        public bool IsActive { get; private set; }

        private ConversationSession() { }

        public ConversationSession(
            Guid configurationId,
            string tenantId,
            string userPhoneNumber,
            string userName = null)
        {
            if (configurationId == Guid.Empty)
                throw new ArgumentException("ConfigurationId is required", nameof(configurationId));
            
            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("TenantId is required", nameof(tenantId));
            
            if (string.IsNullOrWhiteSpace(userPhoneNumber))
                throw new ArgumentException("UserPhoneNumber is required", nameof(userPhoneNumber));

            Id = Guid.NewGuid();
            ConfigurationId = configurationId;
            TenantId = tenantId;
            UserPhoneNumber = userPhoneNumber;
            UserName = userName;
            Context = "{}";
            State = "Initial";
            MessageCountCurrentHour = 0;
            LastMessageAt = DateTime.UtcNow;
            CurrentHourStart = DateTime.UtcNow;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddHours(24);
            IsActive = true;
        }

        public bool CanSendMessage(int maxMessagesPerHour)
        {
            ResetHourlyCountIfNeeded();
            return MessageCountCurrentHour < maxMessagesPerHour;
        }

        public void IncrementMessageCount()
        {
            ResetHourlyCountIfNeeded();
            MessageCountCurrentHour++;
            LastMessageAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            ExtendExpiration();
        }

        private void ResetHourlyCountIfNeeded()
        {
            var now = DateTime.UtcNow;
            if ((now - CurrentHourStart).TotalHours >= 1)
            {
                CurrentHourStart = now;
                MessageCountCurrentHour = 0;
            }
        }

        public void UpdateContext(string context)
        {
            if (string.IsNullOrWhiteSpace(context))
                throw new ArgumentException("Context cannot be empty", nameof(context));
            
            Context = context;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateState(string state)
        {
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be empty", nameof(state));
            
            State = state;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ExtendExpiration()
        {
            ExpiresAt = DateTime.UtcNow.AddHours(24);
            UpdatedAt = DateTime.UtcNow;
        }

        public void EndSession()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }
    }
}
