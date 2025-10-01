using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Patients
{
    public class UpdatePatientCommand : IRequest<PatientDto>
    {
        public Guid PatientId { get; }
        public UpdatePatientDto Patient { get; }
        public string TenantId { get; }

        public UpdatePatientCommand(Guid patientId, UpdatePatientDto patient, string tenantId)
        {
            PatientId = patientId;
            Patient = patient;
            TenantId = tenantId;
        }
    }
}