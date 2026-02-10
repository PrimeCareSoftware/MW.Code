using System;
using MediatR;

namespace MedicSoft.Application.Commands.Appointments
{
    /// <summary>
    /// Command to delete a recurring blocked time slot with specific scope
    /// Supports three deletion patterns: ThisOccurrence, ThisAndFuture, AllInSeries
    /// </summary>
    public class DeleteRecurringScopeCommand : IRequest<bool>
    {
        public Guid BlockedSlotId { get; }
        public RecurringDeleteScope Scope { get; }
        public string TenantId { get; }
        public string? DeletionReason { get; }

        public DeleteRecurringScopeCommand(
            Guid blockedSlotId,
            RecurringDeleteScope scope,
            string tenantId,
            string? deletionReason = null)
        {
            if (blockedSlotId == Guid.Empty)
                throw new ArgumentException("Blocked slot ID cannot be empty", nameof(blockedSlotId));

            if (string.IsNullOrWhiteSpace(tenantId))
                throw new ArgumentException("Tenant ID cannot be empty", nameof(tenantId));

            BlockedSlotId = blockedSlotId;
            Scope = scope;
            TenantId = tenantId;
            DeletionReason = deletionReason?.Trim();
        }
    }
}
