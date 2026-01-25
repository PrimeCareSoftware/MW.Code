using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Clinics
{
    public class GetClinicByIdQuery : IRequest<ClinicDto?>
    {
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetClinicByIdQuery(Guid clinicId, string tenantId)
        {
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }
}
