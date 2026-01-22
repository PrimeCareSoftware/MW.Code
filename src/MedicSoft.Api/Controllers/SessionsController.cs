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
            try
            {
                // Get telemedicine service URL from configuration
                var telemedicineUrl = _configuration.GetValue<string>("Microservices:TelemedicineUrl") 
                    ?? "http://localhost:5084/api";

                var httpClient = _httpClientFactory.CreateClient();
                
                // Forward tenant ID header
                var tenantId = GetTenantId();
                httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);

                // Forward authorization header
                if (Request.Headers.ContainsKey("Authorization"))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", Request.Headers["Authorization"].ToString());
                }

                var url = $"{telemedicineUrl}/sessions/clinic/{clinicId}?skip={skip}&take={take}";
                _logger.LogInformation("Proxying request to telemedicine service: {Url}", url);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = "No sessions found for this clinic" });
                }
                else
                {
                    _logger.LogWarning("Telemedicine service returned status code: {StatusCode}", response.StatusCode);
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
                return StatusCode(500, new { message = "An error occurred while retrieving sessions" });
            }
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
            try
            {
                var telemedicineUrl = _configuration.GetValue<string>("Microservices:TelemedicineUrl") 
                    ?? "http://localhost:5084/api";

                var httpClient = _httpClientFactory.CreateClient();
                
                var tenantId = GetTenantId();
                httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);

                if (Request.Headers.ContainsKey("Authorization"))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", Request.Headers["Authorization"].ToString());
                }

                var url = $"{telemedicineUrl}/sessions/{id}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = $"Session {id} not found" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { message = "Error accessing telemedicine service" });
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to telemedicine service");
                return StatusCode(503, new { 
                    message = "Telemedicine service is currently unavailable",
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while proxying to telemedicine service");
                return StatusCode(500, new { message = "An error occurred while retrieving session" });
            }
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
            try
            {
                var telemedicineUrl = _configuration.GetValue<string>("Microservices:TelemedicineUrl") 
                    ?? "http://localhost:5084/api";

                var httpClient = _httpClientFactory.CreateClient();
                
                var tenantId = GetTenantId();
                httpClient.DefaultRequestHeaders.Add("X-Tenant-Id", tenantId);

                if (Request.Headers.ContainsKey("Authorization"))
                {
                    httpClient.DefaultRequestHeaders.Add("Authorization", Request.Headers["Authorization"].ToString());
                }

                var url = $"{telemedicineUrl}/sessions/appointment/{appointmentId}";
                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return Content(content, "application/json");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { message = $"No session found for appointment {appointmentId}" });
                }
                else
                {
                    return StatusCode((int)response.StatusCode, new { message = "Error accessing telemedicine service" });
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error connecting to telemedicine service");
                return StatusCode(503, new { 
                    message = "Telemedicine service is currently unavailable",
                    details = ex.Message 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while proxying to telemedicine service");
                return StatusCode(500, new { message = "An error occurred while retrieving session" });
            }
        }
    }
}
