using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class GetChildrenOfGuardianQuery : IRequest<IEnumerable<PatientDto>>
    {
        public Guid GuardianId { get; }
        public string TenantId { get; }

        public GetChildrenOfGuardianQuery(Guid guardianId, string tenantId)
        {
            GuardianId = guardianId;
            TenantId = tenantId;
        }
    }
}
