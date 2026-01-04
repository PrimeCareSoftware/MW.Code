using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.TherapeuticPlans
{
    public record UpdateTherapeuticPlanCommand(Guid Id, UpdateTherapeuticPlanDto UpdateDto, string TenantId) 
        : IRequest<TherapeuticPlanDto>;
}
