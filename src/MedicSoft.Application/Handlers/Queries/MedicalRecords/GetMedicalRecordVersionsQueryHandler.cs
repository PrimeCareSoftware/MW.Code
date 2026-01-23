using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.MedicalRecords;
using MedicSoft.Application.Services;

namespace MedicSoft.Application.Handlers.Queries.MedicalRecords
{
    public class GetMedicalRecordVersionsQueryHandler : IRequestHandler<GetMedicalRecordVersionsQuery, List<MedicalRecordVersionDto>>
    {
        private readonly IMedicalRecordVersionService _versionService;
        private readonly IMapper _mapper;

        public GetMedicalRecordVersionsQueryHandler(
            IMedicalRecordVersionService versionService,
            IMapper mapper)
        {
            _versionService = versionService;
            _mapper = mapper;
        }

        public async Task<List<MedicalRecordVersionDto>> Handle(GetMedicalRecordVersionsQuery request, CancellationToken cancellationToken)
        {
            var versions = await _versionService.GetVersionHistoryAsync(request.MedicalRecordId, request.TenantId);
            return _mapper.Map<List<MedicalRecordVersionDto>>(versions);
        }
    }
}
