using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public record CreateMedicalRecordCommand(CreateMedicalRecordDto MedicalRecordDto, Guid UserId, string TenantId) 
        : IRequest<MedicalRecordDto>;
}
