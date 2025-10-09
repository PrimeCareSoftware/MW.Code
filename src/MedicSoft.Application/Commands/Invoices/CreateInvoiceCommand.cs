using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Invoices
{
    public class CreateInvoiceCommand : IRequest<InvoiceDto>
    {
        public CreateInvoiceDto Invoice { get; }
        public string TenantId { get; }

        public CreateInvoiceCommand(CreateInvoiceDto invoice, string tenantId)
        {
            Invoice = invoice;
            TenantId = tenantId;
        }
    }
}
