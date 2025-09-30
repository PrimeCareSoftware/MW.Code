using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class GetAllPatientsQuery : IRequest<IEnumerable<PatientDto>>
    {
        public string TenantId { get; }

        public GetAllPatientsQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}