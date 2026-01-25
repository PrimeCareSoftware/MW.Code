using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Companies
{
    public class CreateCompanyCommand : IRequest<CompanyDto>
    {
        public CreateCompanyDto Company { get; }
        public string TenantId { get; }

        public CreateCompanyCommand(CreateCompanyDto company, string tenantId)
        {
            Company = company;
            TenantId = tenantId;
        }
    }
}
