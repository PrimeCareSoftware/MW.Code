using System;
using System.Threading.Tasks;
using MedicSoft.WhatsAppAgent.Entities;

namespace MedicSoft.WhatsAppAgent.Interfaces
{
    /// <summary>
    /// Repository interface for WhatsApp Agent Configuration
    /// </summary>
    public interface IWhatsAppAgentConfigurationRepository
    {
        Task<WhatsAppAgentConfiguration> GetByIdAsync(Guid id);
        Task<WhatsAppAgentConfiguration> GetByTenantIdAsync(string tenantId);
        Task<WhatsAppAgentConfiguration> GetByWhatsAppNumberAsync(string whatsAppNumber);
        Task AddAsync(WhatsAppAgentConfiguration configuration);
        Task UpdateAsync(WhatsAppAgentConfiguration configuration);
        Task DeleteAsync(Guid id);
    }
}
