using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Patients
{
    public class CreatePatientCommand : IRequest<PatientDto>
    {
        public CreatePatientDto Patient { get; }
        public string TenantId { get; }

        public CreatePatientCommand(CreatePatientDto patient, string tenantId)
        {
            Patient = patient;
            TenantId = tenantId;
        }
    }
}