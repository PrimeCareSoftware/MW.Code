using System.Threading.Tasks;

namespace MedicSoft.WhatsAppAgent.Interfaces
{
    /// <summary>
    /// Interface for WhatsApp Business API integration
    /// </summary>
    public interface IWhatsAppBusinessService
    {
        /// <summary>
        /// Send a message via WhatsApp
        /// </summary>
        Task<bool> SendMessageAsync(string apiKey, string fromNumber, string toNumber, string message);
        
        /// <summary>
        /// Validate the WhatsApp API configuration
        /// </summary>
        Task<bool> ValidateConfigurationAsync(string apiKey, string phoneNumber);
    }
}
