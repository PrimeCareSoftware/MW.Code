using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.SNGPCReports
{
    public record GetSNGPCReportsByYearQuery(int Year, string TenantId) 
        : IRequest<IEnumerable<SNGPCReportDto>>;
}
