using MediatR;

namespace MedicSoft.Application.Commands.Patients
{
    public class LinkPatientToClinicCommand : IRequest<bool>
    {
        public Guid PatientId { get; }
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public LinkPatientToClinicCommand(Guid patientId, Guid clinicId, string tenantId)
        {
            PatientId = patientId;
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }
}
