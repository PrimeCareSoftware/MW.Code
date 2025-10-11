using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Procedures
{
    public class GetProcedureByIdQuery : IRequest<ProcedureDto?>
    {
        public Guid ProcedureId { get; }
        public string TenantId { get; }

        public GetProcedureByIdQuery(Guid procedureId, string tenantId)
        {
            ProcedureId = procedureId;
            TenantId = tenantId;
        }
    }
}
