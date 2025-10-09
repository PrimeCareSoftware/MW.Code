using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Invoices;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Invoices
{
    public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public CreateInvoiceCommandHandler(
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
        {
            // Validate payment exists
            var payment = await _paymentRepository.GetByIdAsync(request.Invoice.PaymentId, request.TenantId);
            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            // Check if invoice already exists for this payment
            var existingInvoice = await _invoiceRepository.GetByPaymentIdAsync(request.Invoice.PaymentId);
            if (existingInvoice != null)
            {
                throw new InvalidOperationException("Invoice already exists for this payment");
            }

            // Parse invoice type
            if (!Enum.TryParse<InvoiceType>(request.Invoice.Type, true, out var invoiceType))
            {
                invoiceType = InvoiceType.Appointment;
            }

            // Create invoice
            var invoice = new Invoice(
                request.Invoice.InvoiceNumber,
                request.Invoice.PaymentId,
                invoiceType,
                request.Invoice.Amount,
                request.Invoice.TaxAmount,
                request.Invoice.DueDate,
                request.Invoice.CustomerName,
                request.TenantId,
                request.Invoice.Description,
                request.Invoice.CustomerDocument,
                request.Invoice.CustomerAddress
            );

            await _invoiceRepository.AddAsync(invoice);

            return _mapper.Map<InvoiceDto>(invoice);
        }
    }
}
