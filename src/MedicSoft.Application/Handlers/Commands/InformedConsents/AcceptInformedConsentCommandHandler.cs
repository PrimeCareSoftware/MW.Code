using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.InformedConsents;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.InformedConsents
{
    public class AcceptInformedConsentCommandHandler : IRequestHandler<AcceptInformedConsentCommand, InformedConsentDto>
    {
        private readonly IInformedConsentRepository _informedConsentRepository;
        private readonly IMapper _mapper;

        public AcceptInformedConsentCommandHandler(
            IInformedConsentRepository informedConsentRepository,
            IMapper mapper)
        {
            _informedConsentRepository = informedConsentRepository;
            _mapper = mapper;
        }

        public async Task<InformedConsentDto> Handle(AcceptInformedConsentCommand request, CancellationToken cancellationToken)
        {
            var consent = await _informedConsentRepository.GetByIdAsync(request.Id, request.TenantId);
            if (consent == null)
            {
                throw new InvalidOperationException("Informed consent not found");
            }

            // Accept the consent
            consent.Accept(
                request.AcceptDto.IPAddress,
                request.AcceptDto.DigitalSignature
            );

            await _informedConsentRepository.UpdateAsync(consent);

            return _mapper.Map<InformedConsentDto>(consent);
        }
    }
}
