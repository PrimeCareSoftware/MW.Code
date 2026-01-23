using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class CloseMedicalRecordCommandHandler : IRequestHandler<CloseMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMedicalRecordVersionService _versionService;
        private readonly ICfm1821ValidationService _cfm1821ValidationService;
        private readonly IMapper _mapper;

        public CloseMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMedicalRecordVersionService versionService,
            ICfm1821ValidationService cfm1821ValidationService,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _versionService = versionService;
            _cfm1821ValidationService = cfm1821ValidationService;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(CloseMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // CFM 1.821 - Validate completeness before closing
            var validationResult = await _cfm1821ValidationService.ValidateMedicalRecordCompleteness(request.Id, request.TenantId);
            if (!validationResult.IsCompliant)
            {
                var missingFields = string.Join(", ", validationResult.MissingRequirements);
                throw new InvalidOperationException($"Medical record is incomplete. Missing requirements: {missingFields}. Complete all CFM 1.821 required fields before closing.");
            }

            // Close the medical record
            medicalRecord.CloseMedicalRecord(request.UserId);

            // Update in repository
            await _medicalRecordRepository.UpdateAsync(medicalRecord);

            // CFM 1.638/2002 - Create version snapshot
            await _versionService.CreateVersionAsync(
                medicalRecordId: medicalRecord.Id,
                changeType: "Closed",
                userId: request.UserId,
                tenantId: request.TenantId
            );

            return _mapper.Map<MedicalRecordDto>(medicalRecord);
        }
    }
}
