using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Payments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Payments
{
    public class ProcessPaymentCommandHandler : IRequestHandler<ProcessPaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public ProcessPaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(
                request.ProcessPayment.PaymentId, 
                request.TenantId);

            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            payment.MarkAsPaid(request.ProcessPayment.TransactionId);
            await _paymentRepository.UpdateAsync(payment);

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
