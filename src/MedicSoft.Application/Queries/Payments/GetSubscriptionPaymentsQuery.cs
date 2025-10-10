using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Payments
{
    public class GetSubscriptionPaymentsQuery : IRequest<List<PaymentDto>>
    {
        public Guid SubscriptionId { get; }
        public string TenantId { get; }

        public GetSubscriptionPaymentsQuery(Guid subscriptionId, string tenantId)
        {
            SubscriptionId = subscriptionId;
            TenantId = tenantId;
        }
    }
}
