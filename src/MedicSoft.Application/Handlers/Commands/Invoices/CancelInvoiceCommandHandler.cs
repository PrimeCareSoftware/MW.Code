using MediatR;
using MedicSoft.Application.Commands.Invoices;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Invoices
{
    public class CancelInvoiceCommandHandler : IRequestHandler<CancelInvoiceCommand, bool>
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public CancelInvoiceCommandHandler(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
        }

        public async Task<bool> Handle(CancelInvoiceCommand request, CancellationToken cancellationToken)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(request.InvoiceId, request.TenantId);

            if (invoice == null)
            {
                throw new InvalidOperationException("Invoice not found");
            }

            invoice.Cancel(request.Reason);
            await _invoiceRepository.UpdateAsync(invoice);

            return true;
        }
    }
}
