using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.TherapeuticPlans;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.TherapeuticPlans
{
    public class CreateTherapeuticPlanCommandHandler : IRequestHandler<CreateTherapeuticPlanCommand, TherapeuticPlanDto>
    {
        private readonly ITherapeuticPlanRepository _therapeuticPlanRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public CreateTherapeuticPlanCommandHandler(
            ITherapeuticPlanRepository therapeuticPlanRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _therapeuticPlanRepository = therapeuticPlanRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<TherapeuticPlanDto> Handle(CreateTherapeuticPlanCommand request, CancellationToken cancellationToken)
        {
            // Validate medical record exists
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.PlanDto.MedicalRecordId, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // Create therapeutic plan
            var plan = new TherapeuticPlan(
                request.PlanDto.MedicalRecordId,
                request.TenantId,
                request.PlanDto.Treatment,
                request.PlanDto.MedicationPrescription,
                request.PlanDto.ExamRequests,
                request.PlanDto.Referrals,
                request.PlanDto.PatientGuidance,
                request.PlanDto.ReturnDate
            );

            await _therapeuticPlanRepository.AddAsync(plan);

            return _mapper.Map<TherapeuticPlanDto>(plan);
        }
    }
}
