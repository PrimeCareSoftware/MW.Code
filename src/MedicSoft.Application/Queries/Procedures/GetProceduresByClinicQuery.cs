using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Procedures
{
    public class GetProceduresByClinicQuery : IRequest<IEnumerable<ProcedureDto>>
    {
        public string TenantId { get; }
        public bool ActiveOnly { get; }
        public Guid? OwnerId { get; }

        public GetProceduresByClinicQuery(string tenantId, bool activeOnly = true, Guid? ownerId = null)
        {
            TenantId = tenantId;
            ActiveOnly = activeOnly;
            OwnerId = ownerId;
        }
    }
}
