using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Payments
{
    public class CreatePaymentCommand : IRequest<PaymentDto>
    {
        public CreatePaymentDto Payment { get; }
        public string TenantId { get; }

        public CreatePaymentCommand(CreatePaymentDto payment, string tenantId)
        {
            Payment = payment;
            TenantId = tenantId;
        }
    }
}
