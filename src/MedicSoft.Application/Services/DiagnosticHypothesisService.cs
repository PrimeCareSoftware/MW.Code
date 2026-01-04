using MediatR;
using MedicSoft.Application.Commands.DiagnosticHypotheses;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.DiagnosticHypotheses;

namespace MedicSoft.Application.Services
{
    public interface IDiagnosticHypothesisService
    {
        Task<DiagnosticHypothesisDto> CreateDiagnosticHypothesisAsync(CreateDiagnosticHypothesisDto createDto, string tenantId);
        Task<DiagnosticHypothesisDto> UpdateDiagnosticHypothesisAsync(Guid id, UpdateDiagnosticHypothesisDto updateDto, string tenantId);
        Task<bool> DeleteDiagnosticHypothesisAsync(Guid id, string tenantId);
        Task<IEnumerable<DiagnosticHypothesisDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId);
    }

    public class DiagnosticHypothesisService : IDiagnosticHypothesisService
    {
        private readonly IMediator _mediator;

        public DiagnosticHypothesisService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<DiagnosticHypothesisDto> CreateDiagnosticHypothesisAsync(CreateDiagnosticHypothesisDto createDto, string tenantId)
        {
            var command = new CreateDiagnosticHypothesisCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<DiagnosticHypothesisDto> UpdateDiagnosticHypothesisAsync(Guid id, UpdateDiagnosticHypothesisDto updateDto, string tenantId)
        {
            var command = new UpdateDiagnosticHypothesisCommand(id, updateDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<bool> DeleteDiagnosticHypothesisAsync(Guid id, string tenantId)
        {
            var command = new DeleteDiagnosticHypothesisCommand(id, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<IEnumerable<DiagnosticHypothesisDto>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            var query = new GetDiagnosticHypothesesByMedicalRecordQuery(medicalRecordId, tenantId);
            return await _mediator.Send(query);
        }
    }
}
