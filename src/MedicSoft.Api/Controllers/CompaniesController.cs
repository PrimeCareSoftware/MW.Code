using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.Companies;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Companies;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : BaseController
    {
        private readonly IMediator _mediator;

        public CompaniesController(IMediator mediator, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get company information for the current tenant (requires company.view permission)
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.CompanyView)]
        public async Task<ActionResult<CompanyDto>> GetCompany()
        {
            var tenantId = GetTenantId();
            var query = new GetCompanyByTenantQuery(tenantId);
            var company = await _mediator.Send(query);
            
            if (company == null)
                return NotFound("Company information not found");

            return Ok(company);
        }

        /// <summary>
        /// Get company by ID (requires company.view permission)
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.CompanyView)]
        public async Task<ActionResult<CompanyDto>> GetById(Guid id)
        {
            var query = new GetCompanyByIdQuery(id, GetTenantId());
            var company = await _mediator.Send(query);
            
            if (company == null)
                return NotFound($"Company with ID {id} not found");

            return Ok(company);
        }

        /// <summary>
        /// Create a new company (requires company.edit permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.CompanyEdit)]
        public async Task<ActionResult<CompanyDto>> Create([FromBody] CreateCompanyDto dto)
        {
            var command = new CreateCompanyCommand(dto, GetTenantId());
            var company = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = company.Id }, company);
        }

        /// <summary>
        /// Update company information (requires company.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.CompanyEdit)]
        public async Task<ActionResult<CompanyDto>> Update(Guid id, [FromBody] UpdateCompanyDto dto)
        {
            var command = new UpdateCompanyCommand(id, dto, GetTenantId());
            var company = await _mediator.Send(command);
            return Ok(company);
        }
    }
}
