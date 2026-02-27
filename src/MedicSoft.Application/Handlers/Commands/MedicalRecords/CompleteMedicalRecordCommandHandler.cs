using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class CompleteMedicalRecordCommandHandler : IRequestHandler<CompleteMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICfm1821ValidationService _cfm1821ValidationService;
        private readonly IMapper _mapper;
        private readonly ILogger<CompleteMedicalRecordCommandHandler> _logger;

        public CompleteMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            ICfm1821ValidationService cfm1821ValidationService,
            IMapper mapper,
            ILogger<CompleteMedicalRecordCommandHandler> logger)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _cfm1821ValidationService = cfm1821ValidationService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<MedicalRecordDto> Handle(CompleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            return await _medicalRecordRepository.ExecuteInTransactionAsync(async () =>
            {
                var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
                if (medicalRecord == null)
                {
                    throw new InvalidOperationException("Medical record not found");
                }

                // CFM 1.821 - Check compliance but allow completion with warning
                var validationResult = await _cfm1821ValidationService.ValidateMedicalRecordCompleteness(request.Id, request.TenantId);
                if (!validationResult.IsCompliant)
                {
                    var missingFields = string.Join("; ", validationResult.MissingRequirements);
                    _logger.LogWarning("Medical record {MedicalRecordId} completed without CFM 1.821/2007 compliance. Missing requirements: {MissingFields}. Tenant: {TenantId}", 
                        request.Id, missingFields, request.TenantId);
                }

                // Complete the medical record
                var completionNotes = request.CompleteDto.Notes;
                if (!string.IsNullOrWhiteSpace(request.CompleteDto.NutritionalPlan))
                {
                    completionNotes = $"{completionNotes}\n\n[Plano Alimentar]\n{request.CompleteDto.NutritionalPlan}".Trim();
                }
                if (!string.IsNullOrWhiteSpace(request.CompleteDto.NutritionalEvolution))
                {
                    completionNotes = $"{completionNotes}\n\n[Evolução Nutricional]\n{request.CompleteDto.NutritionalEvolution}".Trim();
                }
                if (!string.IsNullOrWhiteSpace(request.CompleteDto.TherapeuticEvolution))
                {
                    completionNotes = $"{completionNotes}\n\n[Evolução Terapêutica]\n{request.CompleteDto.TherapeuticEvolution}".Trim();
                }

                medicalRecord.CompleteConsultation(
                    request.CompleteDto.Diagnosis,
                    request.CompleteDto.Prescription,
                    completionNotes
                );

                await _medicalRecordRepository.UpdateAsync(medicalRecord);

                // Check out the appointment
                var appointment = await _appointmentRepository.GetByIdAsync(medicalRecord.AppointmentId, request.TenantId);
                if (appointment != null)
                {
                    appointment.CheckOut(medicalRecord.Notes);
                    await _appointmentRepository.UpdateAsync(appointment);
                }

                return _mapper.Map<MedicalRecordDto>(medicalRecord);
            });
        }
    }
}
