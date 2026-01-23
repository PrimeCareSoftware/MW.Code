using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class ReopenMedicalRecordCommandHandler : IRequestHandler<ReopenMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMedicalRecordVersionService _versionService;
        private readonly IMapper _mapper;

        public ReopenMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMedicalRecordVersionService versionService,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _versionService = versionService;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(ReopenMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // CFM 1.638/2002 - Reopen with justification
            medicalRecord.ReopenMedicalRecord(request.UserId, request.Reason);

            // Update in repository
            await _medicalRecordRepository.UpdateAsync(medicalRecord);

            // CFM 1.638/2002 - Create version snapshot with reason
            await _versionService.CreateVersionAsync(
                medicalRecordId: medicalRecord.Id,
                changeType: "Reopened",
                userId: request.UserId,
                tenantId: request.TenantId,
                reason: request.Reason
            );

            return _mapper.Map<MedicalRecordDto>(medicalRecord);
        }
    }
}
