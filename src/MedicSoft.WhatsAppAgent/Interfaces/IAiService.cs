using System.Threading.Tasks;

namespace MedicSoft.WhatsAppAgent.Interfaces
{
    /// <summary>
    /// Interface for AI service integration (OpenAI, Azure OpenAI, etc.)
    /// </summary>
    public interface IAiService
    {
        /// <summary>
        /// Send a message to the AI and get a response
        /// </summary>
        Task<string> SendMessageAsync(string systemPrompt, string userMessage, string conversationContext);
        
        /// <summary>
        /// Validate the AI API configuration
        /// </summary>
        Task<bool> ValidateConfigurationAsync(string apiKey, string model);
    }
}
