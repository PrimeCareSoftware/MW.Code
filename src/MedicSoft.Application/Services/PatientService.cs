using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;

namespace MedicSoft.Application.Services
{
    public interface IPatientService
    {
        Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto, string tenantId);
        Task<PatientDto> UpdatePatientAsync(Guid patientId, UpdatePatientDto updatePatientDto, string tenantId);
        Task<bool> DeletePatientAsync(Guid patientId, string tenantId);
        Task<PatientDto?> GetPatientByIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string tenantId);
    }

    public class PatientService : IPatientService
    {
        private readonly IMediator _mediator;

        public PatientService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<PatientDto> CreatePatientAsync(CreatePatientDto createPatientDto, string tenantId)
        {
            var command = new CreatePatientCommand(createPatientDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<PatientDto> UpdatePatientAsync(Guid patientId, UpdatePatientDto updatePatientDto, string tenantId)
        {
            var command = new UpdatePatientCommand(patientId, updatePatientDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<bool> DeletePatientAsync(Guid patientId, string tenantId)
        {
            var command = new DeletePatientCommand(patientId, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<PatientDto?> GetPatientByIdAsync(Guid patientId, string tenantId)
        {
            var query = new GetPatientByIdQuery(patientId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<PatientDto>> GetAllPatientsAsync(string tenantId)
        {
            var query = new GetAllPatientsQuery(tenantId);
            return await _mediator.Send(query);
        }
    }
}