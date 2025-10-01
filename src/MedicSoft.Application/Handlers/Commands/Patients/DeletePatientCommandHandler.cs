using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Patients
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;

        public DeletePatientCommandHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<bool> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId, request.TenantId);
            if (patient == null)
            {
                return false;
            }

            // Soft delete by deactivating the patient
            patient.Deactivate();
            await _patientRepository.UpdateAsync(patient);
            
            return true;
        }
    }
}