using MediatR;

namespace MedicSoft.Application.Commands.Invoices
{
    public class IssueInvoiceCommand : IRequest<bool>
    {
        public Guid InvoiceId { get; }
        public string TenantId { get; }

        public IssueInvoiceCommand(Guid invoiceId, string tenantId)
        {
            InvoiceId = invoiceId;
            TenantId = tenantId;
        }
    }
}
