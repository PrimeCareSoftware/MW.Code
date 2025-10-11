using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Procedures
{
    public class CreateProcedureCommand : IRequest<ProcedureDto>
    {
        public CreateProcedureDto Procedure { get; }
        public string TenantId { get; }

        public CreateProcedureCommand(CreateProcedureDto procedure, string tenantId)
        {
            Procedure = procedure;
            TenantId = tenantId;
        }
    }
}
