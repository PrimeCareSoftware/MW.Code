using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMedicalRecordVersionService _versionService;
        private readonly IMapper _mapper;

        public UpdateMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMedicalRecordVersionService versionService,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _versionService = versionService;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // CFM 1.821 - Update required fields
            if (!string.IsNullOrWhiteSpace(request.UpdateDto.ChiefComplaint))
            {
                medicalRecord.UpdateChiefComplaint(request.UpdateDto.ChiefComplaint);
            }

            if (!string.IsNullOrWhiteSpace(request.UpdateDto.HistoryOfPresentIllness))
            {
                medicalRecord.UpdateHistoryOfPresentIllness(request.UpdateDto.HistoryOfPresentIllness);
            }

            // CFM 1.821 - Update recommended fields
            if (request.UpdateDto.PastMedicalHistory != null)
            {
                medicalRecord.UpdatePastMedicalHistory(request.UpdateDto.PastMedicalHistory);
            }

            if (request.UpdateDto.FamilyHistory != null)
            {
                medicalRecord.UpdateFamilyHistory(request.UpdateDto.FamilyHistory);
            }

            if (request.UpdateDto.LifestyleHabits != null)
            {
                medicalRecord.UpdateLifestyleHabits(request.UpdateDto.LifestyleHabits);
            }

            if (request.UpdateDto.CurrentMedications != null)
            {
                medicalRecord.UpdateCurrentMedications(request.UpdateDto.CurrentMedications);
            }

            // Legacy fields (maintain backward compatibility)
            if (request.UpdateDto.Diagnosis != null)
            {
                medicalRecord.UpdateDiagnosis(request.UpdateDto.Diagnosis);
            }

            if (request.UpdateDto.Prescription != null)
            {
                medicalRecord.UpdatePrescription(request.UpdateDto.Prescription);
            }

            if (request.UpdateDto.Notes != null)
            {
                medicalRecord.UpdateNotes(request.UpdateDto.Notes);
            }

            if (request.UpdateDto.TherapeuticEvolution != null || request.UpdateDto.NutritionalPlan != null || request.UpdateDto.NutritionalEvolution != null)
            {
                var notesBuilder = medicalRecord.Notes ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(request.UpdateDto.NutritionalPlan))
                {
                    notesBuilder = $"{notesBuilder}\n\n[Plano Alimentar]\n{request.UpdateDto.NutritionalPlan}".Trim();
                }
                if (!string.IsNullOrWhiteSpace(request.UpdateDto.NutritionalEvolution))
                {
                    notesBuilder = $"{notesBuilder}\n\n[Evolução Nutricional]\n{request.UpdateDto.NutritionalEvolution}".Trim();
                }
                if (!string.IsNullOrWhiteSpace(request.UpdateDto.TherapeuticEvolution))
                {
                    notesBuilder = $"{notesBuilder}\n\n[Evolução Terapêutica]\n{request.UpdateDto.TherapeuticEvolution}".Trim();
                }

                medicalRecord.UpdateNotes(notesBuilder);
            }

            if (request.UpdateDto.ConsultationDurationMinutes.HasValue)
            {
                medicalRecord.UpdateConsultationTime(request.UpdateDto.ConsultationDurationMinutes.Value);
            }

            // CFM 1.638/2002 - Increment version before saving
            medicalRecord.IncrementVersion();

            await _medicalRecordRepository.UpdateAsync(medicalRecord);

            // CFM 1.638/2002 - Create version snapshot after update
            await _versionService.CreateVersionAsync(
                medicalRecordId: medicalRecord.Id,
                changeType: "Updated",
                userId: request.UserId,
                tenantId: request.TenantId
            );

            return _mapper.Map<MedicalRecordDto>(medicalRecord);
        }
    }
}
