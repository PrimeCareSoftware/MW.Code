using System;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.WhatsAppAgent.DTOs;
using MedicSoft.WhatsAppAgent.Entities;
using MedicSoft.WhatsAppAgent.Interfaces;
using MedicSoft.WhatsAppAgent.Security;

namespace MedicSoft.WhatsAppAgent.Services
{
    /// <summary>
    /// Main orchestration service for WhatsApp AI Agent
    /// Handles incoming messages, manages conversations, and coordinates with AI
    /// </summary>
    public class WhatsAppAgentService
    {
        private readonly IWhatsAppAgentConfigurationRepository _configRepository;
        private readonly IConversationSessionRepository _sessionRepository;
        private readonly IAiService _aiService;
        private readonly IWhatsAppBusinessService _whatsAppService;
        private readonly IAppointmentManagementService _appointmentService;

        public WhatsAppAgentService(
            IWhatsAppAgentConfigurationRepository configRepository,
            IConversationSessionRepository sessionRepository,
            IAiService aiService,
            IWhatsAppBusinessService whatsAppService,
            IAppointmentManagementService appointmentService)
        {
            _configRepository = configRepository ?? throw new ArgumentNullException(nameof(configRepository));
            _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
            _aiService = aiService ?? throw new ArgumentNullException(nameof(aiService));
            _whatsAppService = whatsAppService ?? throw new ArgumentNullException(nameof(whatsAppService));
            _appointmentService = appointmentService ?? throw new ArgumentNullException(nameof(appointmentService));
        }

        /// <summary>
        /// Process an incoming WhatsApp message
        /// </summary>
        public async Task<WhatsAppResponseDto> ProcessMessageAsync(WhatsAppWebhookDto webhook)
        {
            try
            {
                // Get configuration for the WhatsApp number
                var config = await _configRepository.GetByWhatsAppNumberAsync(webhook.To);
                if (config == null || !config.IsActive)
                {
                    return new WhatsAppResponseDto
                    {
                        Success = false,
                        Message = "Agent not configured or inactive"
                    };
                }

                // Check business hours
                if (!config.IsWithinBusinessHours(DateTime.Now))
                {
                    await SendWhatsAppMessageAsync(config, webhook.From, 
                        "Desculpe, nosso horário de atendimento é " +
                        $"{config.BusinessHoursStart} às {config.BusinessHoursEnd}, " +
                        $"{config.ActiveDays}. Retornaremos seu contato em breve.");
                    
                    return new WhatsAppResponseDto
                    {
                        Success = true,
                        Message = "Outside business hours"
                    };
                }

                // Security: Check for prompt injection
                if (PromptInjectionGuard.IsSuspicious(webhook.Body))
                {
                    await SendWhatsAppMessageAsync(config, webhook.From, config.FallbackMessage);
                    
                    return new WhatsAppResponseDto
                    {
                        Success = false,
                        Message = "Suspicious input detected"
                    };
                }

                // Sanitize input
                var sanitizedMessage = PromptInjectionGuard.Sanitize(webhook.Body);

                // Get or create conversation session
                var session = await GetOrCreateSessionAsync(config, webhook.From);

                // Check rate limiting
                if (!session.CanSendMessage(config.MaxMessagesPerHour))
                {
                    await SendWhatsAppMessageAsync(config, webhook.From,
                        "Você atingiu o limite de mensagens por hora. Por favor, tente novamente mais tarde.");
                    
                    return new WhatsAppResponseDto
                    {
                        Success = false,
                        Message = "Rate limit exceeded"
                    };
                }

                // Increment message count
                session.IncrementMessageCount();
                await _sessionRepository.UpdateAsync(session);

                // Generate safe system prompt
                var safeSystemPrompt = PromptInjectionGuard.GenerateSafeSystemPrompt(config.SystemPrompt);

                // Get AI response
                var aiResponse = await _aiService.SendMessageAsync(
                    safeSystemPrompt,
                    sanitizedMessage,
                    session.Context);

                // Update conversation context
                var updatedContext = UpdateConversationContext(session.Context, sanitizedMessage, aiResponse);
                session.UpdateContext(updatedContext);
                await _sessionRepository.UpdateAsync(session);

                // Send response via WhatsApp
                await SendWhatsAppMessageAsync(config, webhook.From, aiResponse);

                return new WhatsAppResponseDto
                {
                    Success = true,
                    Message = "Message processed successfully",
                    ResponseText = aiResponse
                };
            }
            catch (Exception ex)
            {
                return new WhatsAppResponseDto
                {
                    Success = false,
                    Message = $"Error processing message: {ex.Message}"
                };
            }
        }

        private async Task<ConversationSession> GetOrCreateSessionAsync(
            WhatsAppAgentConfiguration config, 
            string userPhoneNumber)
        {
            var session = await _sessionRepository.GetActiveSessionAsync(config.Id, userPhoneNumber);
            
            if (session == null || session.IsExpired())
            {
                session = new ConversationSession(config.Id, config.TenantId, userPhoneNumber);
                await _sessionRepository.AddAsync(session);
            }

            return session;
        }

        private async Task SendWhatsAppMessageAsync(
            WhatsAppAgentConfiguration config,
            string toNumber,
            string message)
        {
            await _whatsAppService.SendMessageAsync(
                config.WhatsAppApiKey,
                config.WhatsAppNumber,
                toNumber,
                message);
        }

        private string UpdateConversationContext(string currentContext, string userMessage, string aiResponse)
        {
            try
            {
                var context = JsonSerializer.Deserialize<ConversationContext>(currentContext) 
                    ?? new ConversationContext();
                
                context.Messages.Add(new ConversationMessage
                {
                    Role = "user",
                    Content = userMessage,
                    Timestamp = DateTime.UtcNow
                });

                context.Messages.Add(new ConversationMessage
                {
                    Role = "assistant",
                    Content = aiResponse,
                    Timestamp = DateTime.UtcNow
                });

                // Keep only last 10 messages to avoid context overflow
                if (context.Messages.Count > 10)
                {
                    context.Messages.RemoveRange(0, context.Messages.Count - 10);
                }

                return JsonSerializer.Serialize(context);
            }
            catch
            {
                return "{}";
            }
        }

        /// <summary>
        /// Clean up expired sessions
        /// </summary>
        public async Task CleanupExpiredSessionsAsync()
        {
            var expiredSessions = await _sessionRepository.GetExpiredSessionsAsync();
            
            foreach (var session in expiredSessions)
            {
                session.EndSession();
                await _sessionRepository.UpdateAsync(session);
            }
        }
    }

    internal class ConversationContext
    {
        public System.Collections.Generic.List<ConversationMessage> Messages { get; set; } = new();
    }

    internal class ConversationMessage
    {
        public string Role { get; set; }
        public string Content { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
