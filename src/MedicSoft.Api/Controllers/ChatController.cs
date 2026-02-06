using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Interfaces;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller para gerenciamento do sistema de chat interno
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly IPresenceService _presenceService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(
            IChatService chatService,
            IPresenceService presenceService,
            ITenantContext tenantContext,
            ILogger<ChatController> logger) : base(tenantContext)
        {
            _chatService = chatService;
            _presenceService = presenceService;
            _logger = logger;
        }

        /// <summary>
        /// Obter todas as conversas do usuário
        /// </summary>
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "User not authenticated" });

                var conversations = await _chatService.GetUserConversationsAsync(userId, tenantId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user conversations");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Criar uma conversa direta com outro usuário
        /// </summary>
        [HttpPost("conversations/direct")]
        public async Task<IActionResult> CreateDirectConversation([FromBody] CreateDirectConversationDto dto)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "User not authenticated" });

                if (dto.OtherUserId == Guid.Empty)
                    return BadRequest(new { message = "OtherUserId is required" });

                if (userId == dto.OtherUserId)
                    return BadRequest(new { message = "Cannot create conversation with yourself" });

                var conversation = await _chatService.CreateDirectConversationAsync(userId, dto.OtherUserId, tenantId);
                return Ok(conversation);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating direct conversation");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obter mensagens de uma conversa (paginado)
        /// </summary>
        [HttpGet("conversations/{conversationId}/messages")]
        public async Task<IActionResult> GetMessages(Guid conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 50)
        {
            try
            {
                var tenantId = GetTenantId();

                if (conversationId == Guid.Empty)
                    return BadRequest(new { message = "ConversationId is required" });

                var messages = await _chatService.GetConversationMessagesAsync(conversationId, tenantId, page, pageSize);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation messages");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obter contador de mensagens não lidas em uma conversa
        /// </summary>
        [HttpGet("conversations/{conversationId}/unread-count")]
        public async Task<IActionResult> GetUnreadCount(Guid conversationId)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "User not authenticated" });

                if (conversationId == Guid.Empty)
                    return BadRequest(new { message = "ConversationId is required" });

                var count = await _chatService.GetUnreadCountAsync(conversationId, userId, tenantId);
                return Ok(new { conversationId, unreadCount = count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread count");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Buscar mensagens em uma conversa
        /// </summary>
        [HttpGet("conversations/{conversationId}/search")]
        public async Task<IActionResult> SearchMessages(Guid conversationId, [FromQuery] string query)
        {
            try
            {
                var tenantId = GetTenantId();

                if (conversationId == Guid.Empty)
                    return BadRequest(new { message = "ConversationId is required" });

                if (string.IsNullOrWhiteSpace(query))
                    return BadRequest(new { message = "Query is required" });

                var messages = await _chatService.SearchMessagesAsync(conversationId, query, tenantId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching messages");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obter status de presença de todos os usuários da clínica
        /// </summary>
        [HttpGet("presence")]
        public async Task<IActionResult> GetPresence()
        {
            try
            {
                var tenantId = GetTenantId();
                var presences = await _presenceService.GetAllUserPresencesAsync(tenantId);
                return Ok(presences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user presences");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obter lista de usuários da clínica (para iniciar conversas)
        /// </summary>
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var tenantId = GetTenantId();
                var users = await _chatService.GetClinicUsersAsync(tenantId);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting clinic users");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Editar uma mensagem
        /// </summary>
        [HttpPut("messages/{messageId}")]
        public async Task<IActionResult> EditMessage(Guid messageId, [FromBody] SendMessageDto dto)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "User not authenticated" });

                if (messageId == Guid.Empty)
                    return BadRequest(new { message = "MessageId is required" });

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new { message = "Content is required" });

                var message = await _chatService.EditMessageAsync(messageId, dto.Content, userId, tenantId);
                return Ok(message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing message");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Deletar uma mensagem
        /// </summary>
        [HttpDelete("messages/{messageId}")]
        public async Task<IActionResult> DeleteMessage(Guid messageId)
        {
            try
            {
                var userId = GetUserId();
                var tenantId = GetTenantId();

                if (userId == Guid.Empty)
                    return Unauthorized(new { message = "User not authenticated" });

                if (messageId == Guid.Empty)
                    return BadRequest(new { message = "MessageId is required" });

                await _chatService.DeleteMessageAsync(messageId, userId, tenantId);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        /// <summary>
        /// Obter participantes de uma conversa
        /// </summary>
        [HttpGet("conversations/{conversationId}/participants")]
        public async Task<IActionResult> GetParticipants(Guid conversationId)
        {
            try
            {
                var tenantId = GetTenantId();

                if (conversationId == Guid.Empty)
                    return BadRequest(new { message = "ConversationId is required" });

                var participants = await _chatService.GetConversationParticipantsAsync(conversationId, tenantId);
                return Ok(participants);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting conversation participants");
                return StatusCode(500, new { message = "Internal server error" });
            }
        }
    }
}
