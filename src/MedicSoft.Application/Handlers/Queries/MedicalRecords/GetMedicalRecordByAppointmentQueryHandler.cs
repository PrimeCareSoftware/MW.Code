using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.MedicalRecords;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.MedicalRecords
{
    public class GetMedicalRecordByAppointmentQueryHandler : IRequestHandler<GetMedicalRecordByAppointmentQuery, MedicalRecordDto?>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public GetMedicalRecordByAppointmentQueryHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto?> Handle(GetMedicalRecordByAppointmentQuery request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByAppointmentIdAsync(request.AppointmentId, request.TenantId);
            return medicalRecord != null ? _mapper.Map<MedicalRecordDto>(medicalRecord) : null;
        }
    }
}
