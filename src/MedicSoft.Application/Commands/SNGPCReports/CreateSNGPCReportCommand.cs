using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.SNGPCReports
{
    public record CreateSNGPCReportCommand(CreateSNGPCReportDto ReportDto, string TenantId) 
        : IRequest<SNGPCReportDto>;
}
