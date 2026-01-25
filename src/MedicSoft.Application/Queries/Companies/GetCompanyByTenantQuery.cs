using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Companies
{
    public class GetCompanyByTenantQuery : IRequest<CompanyDto?>
    {
        public string TenantId { get; }

        public GetCompanyByTenantQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
