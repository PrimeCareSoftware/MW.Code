using MediatR;
using MedicSoft.Application.Commands.InformedConsents;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.InformedConsents;

namespace MedicSoft.Application.Services
{
    public interface IInformedConsentService
    {
        Task<InformedConsentDto> CreateInformedConsentAsync(CreateInformedConsentDto createDto, string tenantId);
        Task<InformedConsentDto> AcceptInformedConsentAsync(Guid id, AcceptInformedConsentDto acceptDto, string tenantId);
        Task<IEnumerable<InformedConsentDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }

    public class InformedConsentService : IInformedConsentService
    {
        private readonly IMediator _mediator;

        public InformedConsentService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<InformedConsentDto> CreateInformedConsentAsync(CreateInformedConsentDto createDto, string tenantId)
        {
            var command = new CreateInformedConsentCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<InformedConsentDto> AcceptInformedConsentAsync(Guid id, AcceptInformedConsentDto acceptDto, string tenantId)
        {
            var command = new AcceptInformedConsentCommand(id, acceptDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<IEnumerable<InformedConsentDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            var query = new GetInformedConsentsByMedicalRecordQuery(medicalRecordId, tenantId);
            return await _mediator.Send(query);
        }
    }
}
