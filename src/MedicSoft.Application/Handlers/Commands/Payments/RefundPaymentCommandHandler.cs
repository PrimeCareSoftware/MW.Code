using MediatR;
using MedicSoft.Application.Commands.Payments;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Payments
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, bool>
    {
        private readonly IPaymentRepository _paymentRepository;

        public RefundPaymentCommandHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<bool> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _paymentRepository.GetByIdAsync(request.PaymentId, request.TenantId);

            if (payment == null)
            {
                throw new InvalidOperationException("Payment not found");
            }

            payment.Refund(request.Reason);
            await _paymentRepository.UpdateAsync(payment);

            return true;
        }
    }
}
