using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.TherapeuticPlans;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.TherapeuticPlans
{
    public class UpdateTherapeuticPlanCommandHandler : IRequestHandler<UpdateTherapeuticPlanCommand, TherapeuticPlanDto>
    {
        private readonly ITherapeuticPlanRepository _therapeuticPlanRepository;
        private readonly IMapper _mapper;

        public UpdateTherapeuticPlanCommandHandler(
            ITherapeuticPlanRepository therapeuticPlanRepository,
            IMapper mapper)
        {
            _therapeuticPlanRepository = therapeuticPlanRepository;
            _mapper = mapper;
        }

        public async Task<TherapeuticPlanDto> Handle(UpdateTherapeuticPlanCommand request, CancellationToken cancellationToken)
        {
            var plan = await _therapeuticPlanRepository.GetByIdAsync(request.Id, request.TenantId);
            if (plan == null)
            {
                throw new InvalidOperationException("Therapeutic plan not found");
            }

            // Update treatment if provided
            if (!string.IsNullOrWhiteSpace(request.UpdateDto.Treatment))
            {
                plan.UpdateTreatment(request.UpdateDto.Treatment);
            }

            // Update medication prescription if provided
            if (request.UpdateDto.MedicationPrescription != null)
            {
                plan.UpdateMedicationPrescription(request.UpdateDto.MedicationPrescription);
            }

            // Update exam requests if provided
            if (request.UpdateDto.ExamRequests != null)
            {
                plan.UpdateExamRequests(request.UpdateDto.ExamRequests);
            }

            // Update referrals if provided
            if (request.UpdateDto.Referrals != null)
            {
                plan.UpdateReferrals(request.UpdateDto.Referrals);
            }

            // Update patient guidance if provided
            if (request.UpdateDto.PatientGuidance != null)
            {
                plan.UpdatePatientGuidance(request.UpdateDto.PatientGuidance);
            }

            // Update return date if provided
            if (request.UpdateDto.ReturnDate.HasValue)
            {
                plan.UpdateReturnDate(request.UpdateDto.ReturnDate);
            }

            await _therapeuticPlanRepository.UpdateAsync(plan);

            return _mapper.Map<TherapeuticPlanDto>(plan);
        }
    }
}
