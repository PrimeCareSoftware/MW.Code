using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.WhatsAppAgent.Entities;

namespace MedicSoft.WhatsAppAgent.Interfaces
{
    /// <summary>
    /// Repository interface for Conversation Sessions
    /// </summary>
    public interface IConversationSessionRepository
    {
        Task<ConversationSession> GetByIdAsync(Guid id);
        Task<ConversationSession> GetActiveSessionAsync(Guid configurationId, string userPhoneNumber);
        Task<List<ConversationSession>> GetExpiredSessionsAsync();
        Task AddAsync(ConversationSession session);
        Task UpdateAsync(ConversationSession session);
        Task DeleteAsync(Guid id);
    }
}
