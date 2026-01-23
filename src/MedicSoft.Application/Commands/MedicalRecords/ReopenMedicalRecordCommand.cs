using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.MedicalRecords
{
    public class ReopenMedicalRecordCommand : IRequest<MedicalRecordDto>
    {
        public Guid Id { get; }
        public Guid UserId { get; }
        public string Reason { get; }
        public string TenantId { get; }

        public ReopenMedicalRecordCommand(Guid id, Guid userId, string reason, string tenantId)
        {
            Id = id;
            UserId = userId;
            Reason = reason;
            TenantId = tenantId;
        }
    }
}
