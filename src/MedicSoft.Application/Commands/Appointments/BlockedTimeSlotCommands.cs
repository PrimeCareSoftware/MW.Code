using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Appointments
{
    public class CreateBlockedTimeSlotCommand : IRequest<BlockedTimeSlotDto>
    {
        public CreateBlockedTimeSlotDto BlockedTimeSlot { get; }
        public string TenantId { get; }

        public CreateBlockedTimeSlotCommand(CreateBlockedTimeSlotDto blockedTimeSlot, string tenantId)
        {
            BlockedTimeSlot = blockedTimeSlot;
            TenantId = tenantId;
        }
    }

    public class DeleteBlockedTimeSlotCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeleteBlockedTimeSlotCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }

    public class UpdateBlockedTimeSlotCommand : IRequest<BlockedTimeSlotDto>
    {
        public Guid Id { get; }
        public UpdateBlockedTimeSlotDto BlockedTimeSlot { get; }
        public string TenantId { get; }

        public UpdateBlockedTimeSlotCommand(Guid id, UpdateBlockedTimeSlotDto blockedTimeSlot, string tenantId)
        {
            Id = id;
            BlockedTimeSlot = blockedTimeSlot;
            TenantId = tenantId;
        }
    }
}
