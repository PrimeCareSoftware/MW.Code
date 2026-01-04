using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.InformedConsents
{
    public record CreateInformedConsentCommand(CreateInformedConsentDto ConsentDto, string TenantId) 
        : IRequest<InformedConsentDto>;
}
