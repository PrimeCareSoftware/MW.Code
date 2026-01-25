using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Clinics
{
    public class GetClinicsByOwnerQuery : IRequest<IEnumerable<ClinicDto>>
    {
        public Guid OwnerId { get; }
        public string TenantId { get; }

        public GetClinicsByOwnerQuery(Guid ownerId, string tenantId)
        {
            OwnerId = ownerId;
            TenantId = tenantId;
        }
    }
}
