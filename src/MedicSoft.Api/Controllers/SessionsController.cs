using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Proxy controller for Telemedicine Sessions
    /// Routes requests to the Telemedicine microservice
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SessionsController : BaseController
    {
        private const string DefaultTelemedicineUrl = "http://localhost:5084/api";
        
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SessionsController> _logger;

        public SessionsController(
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ILogger<SessionsController> logger,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Gets the configured telemedicine service URL
        /// </summary>
        private string GetTelemedicineUrl()
        {
            return _configuration.GetValue<string>("Microservices:TelemedicineUrl") ?? DefaultTelemedicineUrl;
        }

        /// <summary>
        /// Configures HttpClient with required headers for telemedicine service
        /// </summary>
        private void ConfigureHttpClientHeaders(HttpClient httpClient)
        {
            // Clear any existing headers to prevent duplicates
            httpClient.DefaultRequestHeaders.Clear();

            // Forward tenant ID header
            var tenantId = GetTenantId();
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("X-Tenant-Id", tenantId);

            // Forward authorization header if present
            if (Request.Headers.TryGetValue("Authorization", out var authHeader))
            {
                var authValue = authHeader.FirstOrDefault();
                if (!string.IsNullOrEmpty(authValue))
                {
                    httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authValue);
                }
            }
        }

        /// <summary>
        /// Proxies GET request to telemedicine service
        /// </summary>
        private async Task<IActionResult> ProxyGetRequestAsync(string endpoint, string notFoundMessage)
        {
            try
            {
                var telemedicineUrl = GetTelemedicineUrl();
                using var httpClient = _httpClientFactory.CreateClient();
                ConfigureHttpClientHeaders(httpClient);

                var url = $"{telemedicineUrl}{endpoint}";
                _logger.LogInformation("Proxying request to telemedicine service: {Url}", url);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = notFoundMessage });
                }
                else
                {
                    _logger.LogWarning("Telemedicine service returned status code: {StatusCode} for {Url}", 
                        response.StatusCode, url);
                    return StatusCode((int)response.StatusCode, new { message = "Error accessing telemedicine service" });
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to telemedicine service");
                return StatusCode(503, new { 
                    message = "Telemedicine service is currently unavailable. Please ensure the telemedicine microservice is running.",
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while proxying to telemedicine service");
                return StatusCode(500, new { message = "An error occurred while retrieving data from telemedicine service" });
            }
        }

        /// <summary>
        /// Gets all sessions for a clinic (proxied to telemedicine microservice)
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetClinicSessions(
            Guid clinicId,
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50)
        {
            var endpoint = $"/sessions/clinic/{clinicId}?skip={skip}&take={take}";
            return await ProxyGetRequestAsync(endpoint, "No sessions found for this clinic");
        }

        /// <summary>
        /// Gets a session by its ID (proxied to telemedicine microservice)
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetSessionById(Guid id)
        {
            var endpoint = $"/sessions/{id}";
            return await ProxyGetRequestAsync(endpoint, $"Session {id} not found");
        }

        /// <summary>
        /// Gets a session by appointment ID (proxied to telemedicine microservice)
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
        public async Task<IActionResult> GetSessionByAppointmentId(Guid appointmentId)
        {
            var endpoint = $"/sessions/appointment/{appointmentId}";
            return await ProxyGetRequestAsync(endpoint, $"No session found for appointment {appointmentId}");
        }
    }
}
