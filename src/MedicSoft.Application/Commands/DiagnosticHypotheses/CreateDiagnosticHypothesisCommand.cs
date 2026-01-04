using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.DiagnosticHypotheses
{
    public record CreateDiagnosticHypothesisCommand(CreateDiagnosticHypothesisDto HypothesisDto, string TenantId) 
        : IRequest<DiagnosticHypothesisDto>;
}
