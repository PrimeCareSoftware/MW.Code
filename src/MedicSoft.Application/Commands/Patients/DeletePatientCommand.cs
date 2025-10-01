using MediatR;

namespace MedicSoft.Application.Commands.Patients
{
    public class DeletePatientCommand : IRequest<bool>
    {
        public Guid PatientId { get; }
        public string TenantId { get; }

        public DeletePatientCommand(Guid patientId, string tenantId)
        {
            PatientId = patientId;
            TenantId = tenantId;
        }
    }
}