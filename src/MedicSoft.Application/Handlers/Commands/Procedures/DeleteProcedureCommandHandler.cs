using MediatR;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Procedures
{
    public class DeleteProcedureCommandHandler : IRequestHandler<DeleteProcedureCommand, bool>
    {
        private readonly IProcedureRepository _procedureRepository;

        public DeleteProcedureCommandHandler(IProcedureRepository procedureRepository)
        {
            _procedureRepository = procedureRepository;
        }

        public async Task<bool> Handle(DeleteProcedureCommand request, CancellationToken cancellationToken)
        {
            var procedure = await _procedureRepository.GetByIdAsync(request.ProcedureId, request.TenantId);
            if (procedure == null)
            {
                return false;
            }

            // Deactivate instead of delete to maintain data integrity
            procedure.Deactivate();
            await _procedureRepository.UpdateAsync(procedure);
            return true;
        }
    }
}
