using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;

namespace MedicSoft.Api.Hubs
{
    /// <summary>
    /// SignalR Hub para chat interno em tempo real
    /// Suporta mensagens diretas, indicadores de digitação, status de presença e read receipts
    /// </summary>
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;
        private readonly IPresenceService _presenceService;
        private readonly ILogger<ChatHub> _logger;

        public ChatHub(IChatService chatService, IPresenceService presenceService, ILogger<ChatHub> logger)
        {
            _chatService = chatService;
            _presenceService = presenceService;
            _logger = logger;
        }

        public override async Task OnConnectedAsync()
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                {
                    _logger.LogWarning("Chat connection rejected: missing userId or tenantId");
                    Context.Abort();
                    return;
                }

                // Set user online - continue even if this fails
                try
                {
                    await _presenceService.SetUserOnlineAsync(userId, Context.ConnectionId, tenantId);
                }
                catch (Exception presenceEx)
                {
                    _logger.LogWarning(presenceEx, "Failed to set user presence for {UserId} in tenant {TenantId}. Connection will continue without presence tracking.", userId, tenantId);
                }

                // Add to tenant group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"tenant_{tenantId}");

                // Notify all users in tenant about presence change
                await Clients.Group($"tenant_{tenantId}").SendAsync("UserPresenceChanged", new
                {
                    UserId = userId,
                    Status = "Online",
                    IsOnline = true,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("User {UserId} connected to chat in tenant {TenantId}", userId, tenantId);

                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatHub.OnConnectedAsync");
                throw;
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId != Guid.Empty && !string.IsNullOrEmpty(tenantId))
                {
                    // Set user offline - continue even if this fails
                    try
                    {
                        await _presenceService.SetUserOfflineAsync(userId, tenantId);
                    }
                    catch (Exception presenceEx)
                    {
                        _logger.LogWarning(presenceEx, "Failed to set user offline for {UserId} in tenant {TenantId}", userId, tenantId);
                    }

                    // Notify all users in tenant about presence change
                    await Clients.Group($"tenant_{tenantId}").SendAsync("UserPresenceChanged", new
                    {
                        UserId = userId,
                        Status = "Offline",
                        IsOnline = false,
                        Timestamp = DateTime.UtcNow
                    });

                    _logger.LogInformation("User {UserId} disconnected from chat in tenant {TenantId}", userId, tenantId);
                }

                await base.OnDisconnectedAsync(exception);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ChatHub.OnDisconnectedAsync");
            }
        }

        /// <summary>
        /// Enviar mensagem em uma conversa
        /// </summary>
        public async Task SendMessage(SendMessageDto dto)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                {
                    _logger.LogWarning("SendMessage rejected: missing userId or tenantId");
                    return;
                }

                // Send message through service
                var message = await _chatService.SendMessageAsync(
                    dto.ConversationId, 
                    userId, 
                    dto.Content, 
                    tenantId, 
                    dto.ReplyToMessageId
                );

                // Get conversation participants
                var participants = await _chatService.GetConversationParticipantsAsync(dto.ConversationId, tenantId);

                // Send to all participants except sender
                foreach (var participant in participants.Where(p => p.UserId != userId))
                {
                    await Clients.User(participant.UserId.ToString()).SendAsync("ReceiveMessage", message);
                }

                // Confirm to sender
                await Clients.Caller.SendAsync("MessageSent", message);

                _logger.LogInformation("Message {MessageId} sent by user {UserId} in conversation {ConversationId}", 
                    message.Id, userId, dto.ConversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message in conversation {ConversationId}", dto.ConversationId);
                await Clients.Caller.SendAsync("MessageError", new { error = "Failed to send message" });
            }
        }

        /// <summary>
        /// Enviar indicador de digitação
        /// </summary>
        public async Task SendTypingIndicator(Guid conversationId)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                    return;

                var participants = await _chatService.GetConversationParticipantsAsync(conversationId, tenantId);

                // Send typing indicator to all participants except sender
                foreach (var participant in participants.Where(p => p.UserId != userId))
                {
                    await Clients.User(participant.UserId.ToString()).SendAsync("UserTyping", new
                    {
                        UserId = userId,
                        ConversationId = conversationId,
                        Timestamp = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending typing indicator for conversation {ConversationId}", conversationId);
            }
        }

        /// <summary>
        /// Marcar mensagem como lida
        /// </summary>
        public async Task MarkAsRead(MarkAsReadDto dto)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                    return;

                await _chatService.MarkMessageAsReadAsync(dto.ConversationId, dto.MessageId, userId, tenantId);

                // Get message to notify sender
                var message = await _chatService.GetMessageAsync(dto.MessageId, tenantId);
                if (message != null)
                {
                    // Notify message sender about read receipt
                    await Clients.User(message.SenderId.ToString()).SendAsync("MessageRead", new
                    {
                        MessageId = dto.MessageId,
                        ConversationId = dto.ConversationId,
                        ReadBy = userId,
                        ReadAt = DateTime.UtcNow
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking message {MessageId} as read", dto.MessageId);
            }
        }

        /// <summary>
        /// Atualizar status de presença do usuário
        /// </summary>
        public async Task UpdateStatus(string status, string? statusMessage = null)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                    return;

                await _presenceService.UpdateUserStatusAsync(userId, status, statusMessage, tenantId);

                // Notify all users in tenant
                await Clients.Group($"tenant_{tenantId}").SendAsync("UserPresenceChanged", new
                {
                    UserId = userId,
                    Status = status,
                    StatusMessage = statusMessage,
                    Timestamp = DateTime.UtcNow
                });

                _logger.LogInformation("User {UserId} updated status to {Status} in tenant {TenantId}", 
                    userId, status, tenantId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user status");
            }
        }

        /// <summary>
        /// Entrar em uma conversa (adicionar aos grupos)
        /// </summary>
        public async Task JoinConversation(Guid conversationId)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty || string.IsNullOrEmpty(tenantId))
                    return;

                // Verify user is a participant
                var participants = await _chatService.GetConversationParticipantsAsync(conversationId, tenantId);
                if (participants.Any(p => p.UserId == userId))
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
                    await Clients.Caller.SendAsync("JoinedConversation", conversationId);

                    _logger.LogInformation("User {UserId} joined conversation {ConversationId}", userId, conversationId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error joining conversation {ConversationId}", conversationId);
            }
        }

        /// <summary>
        /// Sair de uma conversa (remover dos grupos)
        /// </summary>
        public async Task LeaveConversation(Guid conversationId)
        {
            try
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
                await Clients.Caller.SendAsync("LeftConversation", conversationId);

                var userId = GetUserId();
                _logger.LogInformation("User {UserId} left conversation {ConversationId}", userId, conversationId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error leaving conversation {ConversationId}", conversationId);
            }
        }

        private Guid GetUserId()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var userIdClaim = httpContext.User.Claims.FirstOrDefault(c =>
                    c.Type == "sub" ||
                    c.Type == "userId" ||
                    c.Type == "nameid" ||
                    c.Type == System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;
                if (!string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
                {
                    return userId;
                }
            }
            return Guid.Empty;
        }

        private string GetTenantId()
        {
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var tenantClaim = httpContext.User.Claims.FirstOrDefault(c =>
                    c.Type == "tenant_id" ||
                    c.Type == "tenantId"
                )?.Value;

                if (!string.IsNullOrEmpty(tenantClaim))
                {
                    return tenantClaim;
                }

                var headerTenantId = httpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
                if (!string.IsNullOrEmpty(headerTenantId))
                {
                    return headerTenantId;
                }

                if (httpContext.Items.TryGetValue("TenantId", out var tenantIdObj) && tenantIdObj is string contextTenantId)
                {
                    return contextTenantId;
                }
            }
            return string.Empty;
        }
    }
}
