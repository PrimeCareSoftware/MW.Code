using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Companies
{
    public class UpdateCompanyCommand : IRequest<CompanyDto>
    {
        public Guid CompanyId { get; }
        public UpdateCompanyDto Company { get; }
        public string TenantId { get; }

        public UpdateCompanyCommand(Guid companyId, UpdateCompanyDto company, string tenantId)
        {
            CompanyId = companyId;
            Company = company;
            TenantId = tenantId;
        }
    }
}
