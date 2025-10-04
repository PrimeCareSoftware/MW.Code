using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public record CreateMedicalRecordCommand(CreateMedicalRecordDto MedicalRecordDto, string TenantId) 
        : IRequest<MedicalRecordDto>;
}
