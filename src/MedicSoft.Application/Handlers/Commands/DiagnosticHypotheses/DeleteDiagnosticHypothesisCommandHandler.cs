using MediatR;
using MedicSoft.Application.Commands.DiagnosticHypotheses;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.DiagnosticHypotheses
{
    public class DeleteDiagnosticHypothesisCommandHandler : IRequestHandler<DeleteDiagnosticHypothesisCommand, bool>
    {
        private readonly IDiagnosticHypothesisRepository _diagnosticHypothesisRepository;

        public DeleteDiagnosticHypothesisCommandHandler(
            IDiagnosticHypothesisRepository diagnosticHypothesisRepository)
        {
            _diagnosticHypothesisRepository = diagnosticHypothesisRepository;
        }

        public async Task<bool> Handle(DeleteDiagnosticHypothesisCommand request, CancellationToken cancellationToken)
        {
            var hypothesis = await _diagnosticHypothesisRepository.GetByIdAsync(request.Id, request.TenantId);
            if (hypothesis == null)
            {
                throw new InvalidOperationException("Diagnostic hypothesis not found");
            }

            await _diagnosticHypothesisRepository.DeleteAsync(hypothesis.Id, request.TenantId);

            return true;
        }
    }
}
