using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.MedicalRecords;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.MedicalRecords
{
    public class GetPatientMedicalRecordsQueryHandler : IRequestHandler<GetPatientMedicalRecordsQuery, IEnumerable<MedicalRecordDto>>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public GetPatientMedicalRecordsQueryHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MedicalRecordDto>> Handle(GetPatientMedicalRecordsQuery request, CancellationToken cancellationToken)
        {
            var medicalRecords = await _medicalRecordRepository.GetByPatientIdAsync(request.PatientId, request.TenantId);
            return _mapper.Map<IEnumerable<MedicalRecordDto>>(medicalRecords);
        }
    }
}
