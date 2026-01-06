using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.DigitalPrescriptions
{
    public record GetDigitalPrescriptionByIdQuery(Guid Id, string TenantId) 
        : IRequest<DigitalPrescriptionDto?>;
}
