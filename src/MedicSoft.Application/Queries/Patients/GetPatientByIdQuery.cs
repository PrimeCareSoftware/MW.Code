using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class GetPatientByIdQuery : IRequest<PatientDto?>
    {
        public Guid PatientId { get; }
        public string TenantId { get; }

        public GetPatientByIdQuery(Guid patientId, string tenantId)
        {
            PatientId = patientId;
            TenantId = tenantId;
        }
    }
}