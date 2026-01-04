using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.InformedConsents
{
    public record AcceptInformedConsentCommand(Guid Id, AcceptInformedConsentDto AcceptDto, string TenantId) 
        : IRequest<InformedConsentDto>;
}
