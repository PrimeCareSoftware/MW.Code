using MediatR;

namespace MedicSoft.Application.Commands.Procedures
{
    public class DeleteProcedureCommand : IRequest<bool>
    {
        public Guid ProcedureId { get; }
        public string TenantId { get; }

        public DeleteProcedureCommand(Guid procedureId, string tenantId)
        {
            ProcedureId = procedureId;
            TenantId = tenantId;
        }
    }
}
