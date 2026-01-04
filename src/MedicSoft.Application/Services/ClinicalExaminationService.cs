using MediatR;
using MedicSoft.Application.Commands.ClinicalExaminations;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.ClinicalExaminations;

namespace MedicSoft.Application.Services
{
    public interface IClinicalExaminationService
    {
        Task<ClinicalExaminationDto> CreateClinicalExaminationAsync(CreateClinicalExaminationDto createDto, string tenantId);
        Task<ClinicalExaminationDto> UpdateClinicalExaminationAsync(Guid id, UpdateClinicalExaminationDto updateDto, string tenantId);
        Task<IEnumerable<ClinicalExaminationDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }

    public class ClinicalExaminationService : IClinicalExaminationService
    {
        private readonly IMediator _mediator;

        public ClinicalExaminationService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ClinicalExaminationDto> CreateClinicalExaminationAsync(CreateClinicalExaminationDto createDto, string tenantId)
        {
            var command = new CreateClinicalExaminationCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<ClinicalExaminationDto> UpdateClinicalExaminationAsync(Guid id, UpdateClinicalExaminationDto updateDto, string tenantId)
        {
            var command = new UpdateClinicalExaminationCommand(id, updateDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<IEnumerable<ClinicalExaminationDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            var query = new GetClinicalExaminationsByMedicalRecordQuery(medicalRecordId, tenantId);
            return await _mediator.Send(query);
        }
    }
}
