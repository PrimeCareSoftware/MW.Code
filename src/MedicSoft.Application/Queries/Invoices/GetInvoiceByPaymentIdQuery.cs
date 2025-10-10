using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Invoices
{
    public class GetInvoiceByPaymentIdQuery : IRequest<InvoiceDto?>
    {
        public Guid PaymentId { get; }
        public string TenantId { get; }

        public GetInvoiceByPaymentIdQuery(Guid paymentId, string tenantId)
        {
            PaymentId = paymentId;
            TenantId = tenantId;
        }
    }
}
