using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Clinics;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Clinics
{
    public class CreateClinicCommandHandler : IRequestHandler<CreateClinicCommand, ClinicDto>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IOwnerClinicLinkRepository _ownerClinicLinkRepository;
        private readonly IClinicSubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPlanRepository _planRepository;
        private readonly IMapper _mapper;

        public CreateClinicCommandHandler(
            IClinicRepository clinicRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IClinicSubscriptionRepository subscriptionRepository,
            ISubscriptionPlanRepository planRepository,
            IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _subscriptionRepository = subscriptionRepository;
            _planRepository = planRepository;
            _mapper = mapper;
        }

        public async Task<ClinicDto> Handle(CreateClinicCommand request, CancellationToken cancellationToken)
        {
            // Check if owner can create more clinics based on their subscription plan
            var ownerClinics = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(request.OwnerId);
            var activeClinicsCount = ownerClinics.Count(c => c.IsActive);

            // Get owner's subscription plan to check clinic limit
            // For now, we'll get the first clinic's subscription plan
            // In a real scenario, subscription should be at the owner/company level
            var firstClinicLink = ownerClinics.FirstOrDefault();
            if (firstClinicLink != null)
            {
                var subscription = await _subscriptionRepository.GetByClinicIdAsync(firstClinicLink.ClinicId);
                if (subscription != null)
                {
                    var plan = await _planRepository.GetByIdAsync(subscription.SubscriptionPlanId, request.TenantId);
                    if (plan != null && activeClinicsCount >= plan.MaxClinics)
                    {
                        throw new InvalidOperationException(
                            $"Você atingiu o limite de {plan.MaxClinics} clínica(s) do seu plano. " +
                            "Faça upgrade do plano para adicionar mais clínicas.");
                    }
                }
            }

            // Validate document uniqueness
            if (!await _clinicRepository.IsDocumentUniqueAsync(request.Clinic.Document, request.TenantId))
            {
                throw new InvalidOperationException("Já existe uma clínica com este documento (CPF/CNPJ).");
            }

            // Create the clinic
            var clinic = new Clinic(
                request.Clinic.Name,
                request.Clinic.TradeName,
                request.Clinic.Document,
                request.Clinic.Phone,
                request.Clinic.Email,
                request.Clinic.Address,
                request.Clinic.OpeningTime,
                request.Clinic.ClosingTime,
                request.TenantId,
                request.Clinic.AppointmentDurationMinutes
            );

            var createdClinic = await _clinicRepository.AddAsync(clinic);

            // Automatically link the owner to the new clinic as primary owner
            var ownerLink = new OwnerClinicLink(
                request.OwnerId,
                createdClinic.Id,
                request.TenantId,
                isPrimary: true,
                ownershipPercentage: 100
            );
            await _ownerClinicLinkRepository.AddAsync(ownerLink);

            return _mapper.Map<ClinicDto>(createdClinic);
        }
    }
}
