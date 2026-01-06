using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.DigitalPrescriptions
{
    public record GetDigitalPrescriptionsByPatientQuery(Guid PatientId, string TenantId) 
        : IRequest<IEnumerable<DigitalPrescriptionDto>>;
}
