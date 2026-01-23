using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public class CloseMedicalRecordCommand : IRequest<MedicalRecordDto>
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public string TenantId { get; }

        public CloseMedicalRecordCommand(Guid id, Guid userId, string tenantId)
        {
            Id = id;
            UserId = userId;
            TenantId = tenantId;
        }
    }
}
