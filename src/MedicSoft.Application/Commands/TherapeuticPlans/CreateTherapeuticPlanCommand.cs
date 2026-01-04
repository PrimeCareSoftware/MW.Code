using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.TherapeuticPlans
{
    public record CreateTherapeuticPlanCommand(CreateTherapeuticPlanDto PlanDto, string TenantId) 
        : IRequest<TherapeuticPlanDto>;
}
