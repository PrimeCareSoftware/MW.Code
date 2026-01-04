using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.ClinicalExaminations
{
    public record UpdateClinicalExaminationCommand(Guid Id, UpdateClinicalExaminationDto UpdateDto, string TenantId) 
        : IRequest<ClinicalExaminationDto>;
}
