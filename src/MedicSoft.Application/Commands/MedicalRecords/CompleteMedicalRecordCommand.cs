using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public record CompleteMedicalRecordCommand(Guid Id, CompleteMedicalRecordDto CompleteDto, string TenantId) 
        : IRequest<MedicalRecordDto>;
}
