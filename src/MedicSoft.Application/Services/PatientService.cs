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
        Task<IEnumerable<PatientDto>> GetPatientsByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm, string tenantId);
        Task<IEnumerable<PatientDto>> SearchPatientsByClinicAsync(string searchTerm, string tenantId, Guid clinicId);
        Task<PatientDto?> GetPatientByDocumentGlobalAsync(string document);
        Task<bool> LinkPatientToClinicAsync(Guid patientId, Guid clinicId, string tenantId);
        Task<bool> LinkChildToGuardianAsync(Guid childId, Guid guardianId, string tenantId);
        Task<IEnumerable<PatientDto>> GetChildrenOfGuardianAsync(Guid guardianId, string tenantId);
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

        public async Task<IEnumerable<PatientDto>> GetPatientsByClinicIdAsync(Guid clinicId, string tenantId)
        {
            var query = new GetPatientsByClinicIdQuery(clinicId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsAsync(string searchTerm, string tenantId)
        {
            var query = new SearchPatientsQuery(searchTerm, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<PatientDto>> SearchPatientsByClinicAsync(string searchTerm, string tenantId, Guid clinicId)
        {
            var query = new SearchPatientsByClinicQuery(searchTerm, tenantId, clinicId);
            return await _mediator.Send(query);
        }

        public async Task<PatientDto?> GetPatientByDocumentGlobalAsync(string document)
        {
            var query = new GetPatientByDocumentGlobalQuery(document);
            return await _mediator.Send(query);
        }

        public async Task<bool> LinkPatientToClinicAsync(Guid patientId, Guid clinicId, string tenantId)
        {
            var command = new LinkPatientToClinicCommand(patientId, clinicId, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<bool> LinkChildToGuardianAsync(Guid childId, Guid guardianId, string tenantId)
        {
            var command = new LinkChildToGuardianCommand(childId, guardianId, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<IEnumerable<PatientDto>> GetChildrenOfGuardianAsync(Guid guardianId, string tenantId)
        {
            var query = new GetChildrenOfGuardianQuery(guardianId, tenantId);
            return await _mediator.Send(query);
        }
    }
}