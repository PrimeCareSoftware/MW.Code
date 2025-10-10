using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Payments;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Payments
{
    public class GetAppointmentPaymentsQueryHandler : IRequestHandler<GetAppointmentPaymentsQuery, List<PaymentDto>>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public GetAppointmentPaymentsQueryHandler(IPaymentRepository paymentRepository, IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<List<PaymentDto>> Handle(GetAppointmentPaymentsQuery request, CancellationToken cancellationToken)
        {
            var payments = await _paymentRepository.GetByAppointmentIdAsync(request.AppointmentId);
            return _mapper.Map<List<PaymentDto>>(payments);
        }
    }
}
