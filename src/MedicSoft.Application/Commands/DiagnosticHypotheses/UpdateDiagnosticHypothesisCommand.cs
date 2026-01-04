using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.DiagnosticHypotheses
{
    public record UpdateDiagnosticHypothesisCommand(Guid Id, UpdateDiagnosticHypothesisDto UpdateDto, string TenantId) 
        : IRequest<DiagnosticHypothesisDto>;
}
