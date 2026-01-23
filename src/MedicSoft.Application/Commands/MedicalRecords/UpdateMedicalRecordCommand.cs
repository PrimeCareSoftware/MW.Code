using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public record UpdateMedicalRecordCommand(Guid Id, UpdateMedicalRecordDto UpdateDto, Guid UserId, string TenantId) 
        : IRequest<MedicalRecordDto>;
}
