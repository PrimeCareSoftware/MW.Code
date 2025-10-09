using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Invoices;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Invoices
{
    public class GetOverdueInvoicesQueryHandler : IRequestHandler<GetOverdueInvoicesQuery, List<InvoiceDto>>
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IMapper _mapper;

        public GetOverdueInvoicesQueryHandler(IInvoiceRepository invoiceRepository, IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _mapper = mapper;
        }

        public async Task<List<InvoiceDto>> Handle(GetOverdueInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await _invoiceRepository.GetOverdueInvoicesAsync();
            return _mapper.Map<List<InvoiceDto>>(invoices);
        }
    }
}
