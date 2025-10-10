using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Invoices
{
    public class GetOverdueInvoicesQuery : IRequest<List<InvoiceDto>>
    {
        public string TenantId { get; }

        public GetOverdueInvoicesQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
