using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Payments
{
    public class GetPaymentByIdQuery : IRequest<PaymentDto?>
    {
        public Guid PaymentId { get; }
        public string TenantId { get; }

        public GetPaymentByIdQuery(Guid paymentId, string tenantId)
        {
            PaymentId = paymentId;
            TenantId = tenantId;
        }
    }
}
