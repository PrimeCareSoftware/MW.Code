using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.MedicalRecords;

namespace MedicSoft.Application.Services
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto createDto, string tenantId);
        Task<MedicalRecordDto> UpdateMedicalRecordAsync(Guid id, UpdateMedicalRecordDto updateDto, string tenantId);
        Task<MedicalRecordDto> CompleteMedicalRecordAsync(Guid id, CompleteMedicalRecordDto completeDto, string tenantId);
        Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<MedicalRecordDto>> GetPatientMedicalRecordsAsync(Guid patientId, string tenantId);
    }

    public class MedicalRecordService : IMedicalRecordService
    {
        private readonly IMediator _mediator;

        public MedicalRecordService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<MedicalRecordDto> CreateMedicalRecordAsync(CreateMedicalRecordDto createDto, string tenantId)
        {
            var command = new CreateMedicalRecordCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<MedicalRecordDto> UpdateMedicalRecordAsync(Guid id, UpdateMedicalRecordDto updateDto, string tenantId)
        {
            var command = new UpdateMedicalRecordCommand(id, updateDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<MedicalRecordDto> CompleteMedicalRecordAsync(Guid id, CompleteMedicalRecordDto completeDto, string tenantId)
        {
            var command = new CompleteMedicalRecordCommand(id, completeDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<MedicalRecordDto?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            var query = new GetMedicalRecordByAppointmentQuery(appointmentId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<MedicalRecordDto>> GetPatientMedicalRecordsAsync(Guid patientId, string tenantId)
        {
            var query = new GetPatientMedicalRecordsQuery(patientId, tenantId);
            return await _mediator.Send(query);
        }
    }
}
