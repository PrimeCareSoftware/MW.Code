using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.ClinicalExaminations;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.ClinicalExaminations
{
    public class GetClinicalExaminationsByMedicalRecordQueryHandler 
        : IRequestHandler<GetClinicalExaminationsByMedicalRecordQuery, IEnumerable<ClinicalExaminationDto>>
    {
        private readonly IClinicalExaminationRepository _clinicalExaminationRepository;
        private readonly IMapper _mapper;

        public GetClinicalExaminationsByMedicalRecordQueryHandler(
            IClinicalExaminationRepository clinicalExaminationRepository,
            IMapper mapper)
        {
            _clinicalExaminationRepository = clinicalExaminationRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClinicalExaminationDto>> Handle(
            GetClinicalExaminationsByMedicalRecordQuery request, 
            CancellationToken cancellationToken)
        {
            var examinations = await _clinicalExaminationRepository
                .GetByMedicalRecordIdAsync(request.MedicalRecordId, request.TenantId);
            
            return _mapper.Map<IEnumerable<ClinicalExaminationDto>>(examinations);
        }
    }
}
