using MediatR;

namespace MedicSoft.Application.Commands.Payments
{
    public class CancelPaymentCommand : IRequest<bool>
    {
        public Guid PaymentId { get; }
        public string Reason { get; }
        public string TenantId { get; }

        public CancelPaymentCommand(Guid paymentId, string reason, string tenantId)
        {
            PaymentId = paymentId;
            Reason = reason;
            TenantId = tenantId;
        }
    }
}
