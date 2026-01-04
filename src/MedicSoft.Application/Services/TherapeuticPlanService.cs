using MediatR;
using MedicSoft.Application.Commands.TherapeuticPlans;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.TherapeuticPlans;

namespace MedicSoft.Application.Services
{
    public interface ITherapeuticPlanService
    {
        Task<TherapeuticPlanDto> CreateTherapeuticPlanAsync(CreateTherapeuticPlanDto createDto, string tenantId);
        Task<TherapeuticPlanDto> UpdateTherapeuticPlanAsync(Guid id, UpdateTherapeuticPlanDto updateDto, string tenantId);
        Task<IEnumerable<TherapeuticPlanDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }

    public class TherapeuticPlanService : ITherapeuticPlanService
    {
        private readonly IMediator _mediator;

        public TherapeuticPlanService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TherapeuticPlanDto> CreateTherapeuticPlanAsync(CreateTherapeuticPlanDto createDto, string tenantId)
        {
            var command = new CreateTherapeuticPlanCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<TherapeuticPlanDto> UpdateTherapeuticPlanAsync(Guid id, UpdateTherapeuticPlanDto updateDto, string tenantId)
        {
            var command = new UpdateTherapeuticPlanCommand(id, updateDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<IEnumerable<TherapeuticPlanDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            var query = new GetTherapeuticPlansByMedicalRecordQuery(medicalRecordId, tenantId);
            return await _mediator.Send(query);
        }
    }
}
