using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Invoices
{
    public class GetInvoiceByIdQuery : IRequest<InvoiceDto?>
    {
        public Guid InvoiceId { get; }
        public string TenantId { get; }

        public GetInvoiceByIdQuery(Guid invoiceId, string tenantId)
        {
            InvoiceId = invoiceId;
            TenantId = tenantId;
        }
    }
}
