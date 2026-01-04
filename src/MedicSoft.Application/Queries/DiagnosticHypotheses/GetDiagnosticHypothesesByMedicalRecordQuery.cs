using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.DiagnosticHypotheses
{
    public record GetDiagnosticHypothesesByMedicalRecordQuery(Guid MedicalRecordId, string TenantId) 
        : IRequest<IEnumerable<DiagnosticHypothesisDto>>;
}
