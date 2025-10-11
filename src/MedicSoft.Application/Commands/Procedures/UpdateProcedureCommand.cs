using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Procedures
{
    public class UpdateProcedureCommand : IRequest<ProcedureDto>
    {
        public Guid ProcedureId { get; }
        public UpdateProcedureDto Procedure { get; }
        public string TenantId { get; }

        public UpdateProcedureCommand(Guid procedureId, UpdateProcedureDto procedure, string tenantId)
        {
            ProcedureId = procedureId;
            Procedure = procedure;
            TenantId = tenantId;
        }
    }
}
