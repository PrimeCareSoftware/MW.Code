using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Patients
{
    public class SearchPatientsQuery : IRequest<IEnumerable<PatientDto>>
    {
        public string SearchTerm { get; }
        public string TenantId { get; }

        public SearchPatientsQuery(string searchTerm, string tenantId)
        {
            SearchTerm = searchTerm;
            TenantId = tenantId;
        }
    }
}
