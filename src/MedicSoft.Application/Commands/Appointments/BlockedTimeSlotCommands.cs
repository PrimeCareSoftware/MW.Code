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

    /// <summary>
    /// DEPRECATED: Use DeleteRecurringScopeCommand instead for better control over recurring deletions.
    /// This command may delete multiple series accidentally when using DeleteSeries=true.
    /// </summary>
    [Obsolete("Use DeleteRecurringScopeCommand instead to avoid deleting multiple series. This will be removed in a future version.")]
    public class DeleteBlockedTimeSlotCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public string TenantId { get; }
        public bool DeleteSeries { get; } // If true, delete all instances in the series

        public DeleteBlockedTimeSlotCommand(Guid id, string tenantId, bool deleteSeries = false)
        {
            Id = id;
            TenantId = tenantId;
            DeleteSeries = deleteSeries;
        }
    }

    public class UpdateBlockedTimeSlotCommand : IRequest<BlockedTimeSlotDto>
    {
        public Guid Id { get; }
        public UpdateBlockedTimeSlotDto BlockedTimeSlot { get; }
        public string TenantId { get; }
        public bool UpdateSeries { get; } // If true, update all instances in the series

        public UpdateBlockedTimeSlotCommand(Guid id, UpdateBlockedTimeSlotDto blockedTimeSlot, string tenantId, bool updateSeries = false)
        {
            Id = id;
            BlockedTimeSlot = blockedTimeSlot;
            TenantId = tenantId;
            UpdateSeries = updateSeries;
        }
    }
}
