using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    /// <summary>
    /// Handler for deleting recurring blocked time slots with specific scope
    /// Implements three deletion patterns:
    /// 1. ThisOccurrence: Soft delete via exception
    /// 2. ThisAndFuture: Split series at date
    /// 3. AllInSeries: Delete entire series
    /// </summary>
    public class DeleteRecurringScopeCommandHandler : IRequestHandler<DeleteRecurringScopeCommand, bool>
    {
        private readonly IBlockedTimeSlotRepository _blockedTimeSlotRepository;
        private readonly IRecurrenceExceptionRepository _exceptionRepository;
        private readonly IRecurringAppointmentPatternRepository _patternRepository;

        public DeleteRecurringScopeCommandHandler(
            IBlockedTimeSlotRepository blockedTimeSlotRepository,
            IRecurrenceExceptionRepository exceptionRepository,
            IRecurringAppointmentPatternRepository patternRepository)
        {
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
            _exceptionRepository = exceptionRepository;
            _patternRepository = patternRepository;
        }

        public async Task<bool> Handle(DeleteRecurringScopeCommand request, CancellationToken cancellationToken)
        {
            var blockedSlot = await _blockedTimeSlotRepository.GetByIdAsync(request.BlockedSlotId, request.TenantId);
            if (blockedSlot == null)
                return false;

            // For non-recurring blocks, just delete directly
            if (!blockedSlot.IsRecurring || !blockedSlot.RecurringSeriesId.HasValue)
            {
                await _blockedTimeSlotRepository.DeleteAsync(request.BlockedSlotId, request.TenantId);
                return true;
            }

            var seriesId = blockedSlot.RecurringSeriesId.Value;

            switch (request.Scope)
            {
                case RecurringDeleteScope.ThisOccurrence:
                    return await DeleteSingleOccurrence(blockedSlot, seriesId, request.TenantId, request.DeletionReason, cancellationToken);
                
                case RecurringDeleteScope.ThisAndFuture:
                    return await DeleteThisAndFutureOccurrences(blockedSlot, seriesId, request.TenantId, request.DeletionReason, cancellationToken);
                
                case RecurringDeleteScope.AllInSeries:
                    return await DeleteEntireSeries(seriesId, blockedSlot.RecurringPatternId, request.TenantId, request.DeletionReason, cancellationToken);
                
                default:
                    throw new ArgumentException($"Unknown recurring delete scope: {request.Scope}");
            }
        }

        /// <summary>
        /// Deletes a single occurrence by creating an exception and removing the blocked slot
        /// </summary>
        private async Task<bool> DeleteSingleOccurrence(
            BlockedTimeSlot blockedSlot,
            Guid seriesId,
            string tenantId,
            string? reason,
            CancellationToken cancellationToken)
        {
            if (!blockedSlot.RecurringPatternId.HasValue)
                return false;

            // Create exception record for audit and future reference
            var exception = new RecurrenceException(
                blockedSlot.RecurringPatternId.Value,
                seriesId,
                blockedSlot.Date,
                RecurrenceExceptionType.Deleted,
                tenantId,
                reason);

            await _exceptionRepository.AddAsync(exception);

            // Remove the blocked slot (hard delete from table)
            await _blockedTimeSlotRepository.DeleteAsync(blockedSlot.Id, tenantId);

            return true;
        }

        /// <summary>
        /// Deletes this occurrence and all future occurrences by updating the pattern's effective end date
        /// </summary>
        private async Task<bool> DeleteThisAndFutureOccurrences(
            BlockedTimeSlot blockedSlot,
            Guid seriesId,
            string tenantId,
            string? reason,
            CancellationToken cancellationToken)
        {
            if (!blockedSlot.RecurringPatternId.HasValue)
                return false;

            // Get the pattern
            var pattern = await _patternRepository.GetByIdAsync(blockedSlot.RecurringPatternId.Value, tenantId);
            if (pattern == null)
                return false;

            // Set effective end date to the day before this occurrence
            var effectiveEndDate = blockedSlot.Date.AddDays(-1);
            pattern.SetEffectiveEndDate(effectiveEndDate);

            await _patternRepository.UpdateAsync(pattern);

            // Delete all future occurrences (including this one)
            await _blockedTimeSlotRepository.DeleteFutureOccurrencesAsync(seriesId, blockedSlot.Date, tenantId);

            return true;
        }

        /// <summary>
        /// Deletes the entire series by removing all occurrences and deactivating the pattern
        /// </summary>
        private async Task<bool> DeleteEntireSeries(
            Guid seriesId,
            Guid? patternId,
            string tenantId,
            string? reason,
            CancellationToken cancellationToken)
        {
            // Delete all blocked slots in the series
            await _blockedTimeSlotRepository.DeleteByRecurringSeriesIdAsync(seriesId, tenantId);

            // Deactivate the pattern if it exists
            if (patternId.HasValue)
            {
                var pattern = await _patternRepository.GetByIdAsync(patternId.Value, tenantId);
                if (pattern != null)
                {
                    pattern.Deactivate();
                    await _patternRepository.UpdateAsync(pattern);
                }
            }

            // Clean up any exceptions for this series
            var exceptions = await _exceptionRepository.GetByRecurringSeriesIdAsync(seriesId, tenantId);
            foreach (var exception in exceptions)
            {
                await _exceptionRepository.DeleteAsync(exception.Id, tenantId);
            }

            return true;
        }
    }
}
