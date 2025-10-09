using MediatR;
using MedicSoft.Application.Commands.Invoices;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Invoices
{
    public class IssueInvoiceCommandHandler : IRequestHandler<IssueInvoiceCommand, bool>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public IssueInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> Handle(IssueInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, request.TenantId);

            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found");
            }

            invoice.Issue();
            await _invoiceRepository.UpdateAsync(invoice);

            return true;
        }
    }
}
