using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Companies
{
    public class GetCompanyByIdQuery : IRequest<CompanyDto?>
    {
        public Guid CompanyId { get; }
        public string TenantId { get; }

        public GetCompanyByIdQuery(Guid companyId, string tenantId)
        {
            CompanyId = companyId;
            TenantId = tenantId;
        }
    }
}
