using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.MedicalRecords;
using MedicSoft.Application.Services;

namespace MedicSoft.Application.Handlers.Queries.MedicalRecords
{
    public class GetMedicalRecordAccessLogsQueryHandler : IRequestHandler<GetMedicalRecordAccessLogsQuery, List<MedicalRecordAccessLogDto>>
    {
        private readonly IMedicalRecordAuditService _auditService;
        private readonly IMapper _mapper;

        public GetMedicalRecordAccessLogsQueryHandler(
            IMedicalRecordAuditService auditService,
            IMapper mapper)
        {
            _auditService = auditService;
            _mapper = mapper;
        }

        public async Task<List<MedicalRecordAccessLogDto>> Handle(GetMedicalRecordAccessLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _auditService.GetAccessLogsAsync(
                request.MedicalRecordId, 
                request.TenantId, 
                request.StartDate, 
                request.EndDate);
            
            return _mapper.Map<List<MedicalRecordAccessLogDto>>(logs);
        }
    }
}
