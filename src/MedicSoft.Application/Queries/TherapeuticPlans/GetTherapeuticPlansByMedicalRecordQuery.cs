using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.TherapeuticPlans
{
    public record GetTherapeuticPlansByMedicalRecordQuery(Guid MedicalRecordId, string TenantId) 
        : IRequest<IEnumerable<TherapeuticPlanDto>>;
}
