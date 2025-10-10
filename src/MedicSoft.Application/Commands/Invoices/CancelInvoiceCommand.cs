using MediatR;

namespace MedicSoft.Application.Commands.Invoices
{
    public class CancelInvoiceCommand : IRequest<bool>
    {
        public Guid InvoiceId { get; }
        public string Reason { get; }
        public string TenantId { get; }

        public CancelInvoiceCommand(Guid invoiceId, string reason, string tenantId)
        {
            InvoiceId = invoiceId;
            Reason = reason;
            TenantId = tenantId;
        }
    }
}
