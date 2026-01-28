using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for global search across system entities
    /// </summary>
    [ApiController]
    [Route("api/system-admin/search")]
    [Authorize] // Add role authorization in production: [Authorize(Roles = "SystemAdmin")]
    public class SearchController : BaseController
    {
        private readonly IGlobalSearchService _searchService;

        public SearchController(
            ITenantContext tenantContext,
            IGlobalSearchService searchService) : base(tenantContext)
        {
            _searchService = searchService;
        }

        /// <summary>
        /// Global search across clinics, users, tickets, plans, and audit logs
        /// </summary>
        /// <param name="q">Search query</param>
        /// <param name="maxResults">Maximum results per entity type</param>
        [HttpGet]
        public async Task<ActionResult<GlobalSearchResultDto>> Search(
            [FromQuery] string q,
            [FromQuery] int maxResults = 50)
        {
            if (string.IsNullOrWhiteSpace(q) || q.Length < 2)
            {
                return BadRequest(new { message = "Query must be at least 2 characters long" });
            }

            var results = await _searchService.SearchAsync(q, maxResults);
            return Ok(results);
        }
    }
}
