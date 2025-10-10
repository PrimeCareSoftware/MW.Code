using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Payments
{
    public class ProcessPaymentCommand : IRequest<PaymentDto>
    {
        public ProcessPaymentDto ProcessPayment { get; }
        public string TenantId { get; }

        public ProcessPaymentCommand(ProcessPaymentDto processPayment, string tenantId)
        {
            ProcessPayment = processPayment;
            TenantId = tenantId;
        }
    }
}
