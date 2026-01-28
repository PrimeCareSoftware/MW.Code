using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Webhook Controller
    /// 
    /// Manages webhook subscriptions and event delivery for CRM events.
    /// Provides endpoints for creating, updating, and managing webhooks, as well as viewing delivery history.
    /// </summary>
    /// <remarks>
    /// This controller handles:
    /// - Creating and managing webhook subscriptions
    /// - Configuring event types and retry policies
    /// - Viewing webhook delivery history and status
    /// - Manual retry of failed deliveries
    /// 
    /// All endpoints require authentication and operate within tenant context for multi-tenant isolation.
    /// Webhooks are delivered with HMAC-SHA256 signatures for security validation.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/crm/webhooks")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "CRM - Webhooks")]
    public class WebhookController : BaseController
    {
        private readonly IWebhookService _webhookService;
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(
            IWebhookService webhookService,
            ITenantContext tenantContext,
            ILogger<WebhookController> logger)
            : base(tenantContext)
        {
            _webhookService = webhookService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new webhook subscription
        /// </summary>
        /// <remarks>
        /// Creates a new webhook subscription for receiving CRM event notifications.
        /// The subscription is created in an inactive state and must be activated explicitly.
        /// A unique secret is generated for signature validation.
        /// 
        /// Sample request:
        /// 
        ///     POST /api/crm/webhooks
        ///     {
        ///       "name": "Patient Journey Webhook",
        ///       "description": "Receive patient journey stage changes",
        ///       "targetUrl": "https://example.com/webhooks/crm",
        ///       "subscribedEvents": [1, 2],
        ///       "maxRetries": 3,
        ///       "retryDelaySeconds": 60
        ///     }
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateSubscription([FromBody] CreateWebhookSubscriptionDto dto)
        {
            try
            {
                var subscription = await _webhookService.CreateSubscriptionAsync(dto, GetTenantId());
                _logger.LogInformation("Created webhook subscription {Id}", subscription.Id);
                return CreatedAtAction(nameof(GetSubscription), new { id = subscription.Id }, subscription);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating webhook subscription");
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get webhook subscription by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSubscription(Guid id)
        {
            var subscription = await _webhookService.GetSubscriptionAsync(id, GetTenantId());
            if (subscription == null)
                return NotFound();

            return Ok(subscription);
        }

        /// <summary>
        /// Get all webhook subscriptions for current tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(List<WebhookSubscriptionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllSubscriptions()
        {
            var subscriptions = await _webhookService.GetAllSubscriptionsAsync(GetTenantId());
            return Ok(subscriptions);
        }

        /// <summary>
        /// Update webhook subscription
        /// </summary>
        /// <remarks>
        /// Updates webhook subscription properties. Only provided fields will be updated.
        /// </remarks>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSubscription(Guid id, [FromBody] UpdateWebhookSubscriptionDto dto)
        {
            try
            {
                var subscription = await _webhookService.UpdateSubscriptionAsync(id, dto, GetTenantId());
                _logger.LogInformation("Updated webhook subscription {Id}", id);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating webhook subscription {Id}", id);
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Delete webhook subscription
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteSubscription(Guid id)
        {
            try
            {
                await _webhookService.DeleteSubscriptionAsync(id, GetTenantId());
                _logger.LogInformation("Deleted webhook subscription {Id}", id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Activate webhook subscription
        /// </summary>
        /// <remarks>
        /// Activates a webhook subscription to start receiving events.
        /// </remarks>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ActivateSubscription(Guid id)
        {
            try
            {
                var subscription = await _webhookService.ActivateSubscriptionAsync(id, GetTenantId());
                _logger.LogInformation("Activated webhook subscription {Id}", id);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate webhook subscription
        /// </summary>
        /// <remarks>
        /// Deactivates a webhook subscription to stop receiving events.
        /// </remarks>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateSubscription(Guid id)
        {
            try
            {
                var subscription = await _webhookService.DeactivateSubscriptionAsync(id, GetTenantId());
                _logger.LogInformation("Deactivated webhook subscription {Id}", id);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Regenerate webhook secret
        /// </summary>
        /// <remarks>
        /// Generates a new secret for the webhook subscription.
        /// The old secret will no longer be valid for signature verification.
        /// </remarks>
        [HttpPost("{id}/regenerate-secret")]
        [ProducesResponseType(typeof(WebhookSubscriptionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RegenerateSecret(Guid id)
        {
            try
            {
                var subscription = await _webhookService.RegenerateSecretAsync(id, GetTenantId());
                _logger.LogInformation("Regenerated secret for webhook subscription {Id}", id);
                return Ok(subscription);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get webhook deliveries for a subscription
        /// </summary>
        /// <remarks>
        /// Retrieves the delivery history for a webhook subscription, including status and error details.
        /// </remarks>
        [HttpGet("{subscriptionId}/deliveries")]
        [ProducesResponseType(typeof(List<WebhookDeliveryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDeliveries(Guid subscriptionId, [FromQuery] int limit = 50)
        {
            var deliveries = await _webhookService.GetDeliveriesAsync(subscriptionId, GetTenantId(), limit);
            return Ok(deliveries);
        }

        /// <summary>
        /// Get webhook delivery by ID
        /// </summary>
        [HttpGet("deliveries/{id}")]
        [ProducesResponseType(typeof(WebhookDeliveryDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetDelivery(Guid id)
        {
            var delivery = await _webhookService.GetDeliveryAsync(id, GetTenantId());
            if (delivery == null)
                return NotFound();

            return Ok(delivery);
        }

        /// <summary>
        /// Retry failed webhook delivery
        /// </summary>
        /// <remarks>
        /// Manually retries a failed webhook delivery. The delivery must not already be delivered.
        /// </remarks>
        [HttpPost("deliveries/{id}/retry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RetryDelivery(Guid id)
        {
            try
            {
                await _webhookService.RetryFailedDeliveryAsync(id, GetTenantId());
                _logger.LogInformation("Retried webhook delivery {Id}", id);
                return Ok(new { message = "Delivery retry initiated" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get available webhook events
        /// </summary>
        /// <remarks>
        /// Returns a list of all available webhook events that can be subscribed to.
        /// </remarks>
        [HttpGet("events")]
        [ProducesResponseType(typeof(Dictionary<int, string>), StatusCodes.Status200OK)]
        public IActionResult GetAvailableEvents()
        {
            var events = Enum.GetValues<WebhookEvent>()
                .ToDictionary(e => (int)e, e => e.ToString());
            return Ok(events);
        }
    }
}
