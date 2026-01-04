using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.InformedConsents
{
    public record GetInformedConsentsByMedicalRecordQuery(Guid MedicalRecordId, string TenantId) 
        : IRequest<IEnumerable<InformedConsentDto>>;
}
