using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.DigitalPrescriptions
{
    public record CreateDigitalPrescriptionCommand(CreateDigitalPrescriptionDto PrescriptionDto, string TenantId) 
        : IRequest<DigitalPrescriptionDto>;
}
