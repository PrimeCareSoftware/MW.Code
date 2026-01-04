using MediatR;

namespace MedicSoft.Application.Commands.DiagnosticHypotheses
{
    public record DeleteDiagnosticHypothesisCommand(Guid Id, string TenantId) 
        : IRequest<bool>;
}
