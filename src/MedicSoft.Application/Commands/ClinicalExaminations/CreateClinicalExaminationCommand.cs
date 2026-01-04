using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.ClinicalExaminations
{
    public record CreateClinicalExaminationCommand(CreateClinicalExaminationDto ExaminationDto, string TenantId) 
        : IRequest<ClinicalExaminationDto>;
}
