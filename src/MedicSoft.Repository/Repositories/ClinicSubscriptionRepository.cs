using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ClinicSubscriptionRepository : IClinicSubscriptionRepository
    {
        private readonly MedicSoftDbContext _context;

        public ClinicSubscriptionRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<ClinicSubscription?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.ClinicSubscriptions
                .Include(cs => cs.Clinic)
                .Include(cs => cs.SubscriptionPlan)
                .Include(cs => cs.PendingPlan)
                .FirstOrDefaultAsync(cs => cs.Id == id && cs.TenantId == tenantId);
        }

        public async Task<ClinicSubscription?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.ClinicSubscriptions
                .Include(cs => cs.Clinic)
                .Include(cs => cs.SubscriptionPlan)
                .Include(cs => cs.PendingPlan)
                .Where(cs => cs.ClinicId == clinicId && cs.TenantId == tenantId)
                .OrderByDescending(cs => cs.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClinicSubscription>> GetAllAsync(string tenantId)
        {
            return await _context.ClinicSubscriptions
                .Where(cs => cs.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicSubscription>> GetOverdueSubscriptionsAsync()
        {
            var today = DateTime.UtcNow;
            return await _context.ClinicSubscriptions
                .Include(cs => cs.Clinic)
                .Include(cs => cs.SubscriptionPlan)
                .Where(cs => cs.NextPaymentDate.HasValue &&
                           cs.NextPaymentDate.Value < today &&
                           cs.Status == SubscriptionStatus.Active)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicSubscription>> GetExpiringTrialsAsync(int daysBeforeExpiration)
        {
            var targetDate = DateTime.UtcNow.AddDays(daysBeforeExpiration);
            return await _context.ClinicSubscriptions
                .Include(cs => cs.Clinic)
                .Include(cs => cs.SubscriptionPlan)
                .Where(cs => cs.Status == SubscriptionStatus.Trial &&
                           cs.TrialEndDate.HasValue &&
                           cs.TrialEndDate.Value <= targetDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicSubscription>> GetPendingDowngradesAsync()
        {
            var today = DateTime.UtcNow;
            return await _context.ClinicSubscriptions
                .Include(cs => cs.Clinic)
                .Include(cs => cs.SubscriptionPlan)
                .Include(cs => cs.PendingPlan)
                .Where(cs => cs.PendingPlanId.HasValue &&
                           !cs.IsUpgrade &&
                           cs.PlanChangeDate.HasValue &&
                           cs.PlanChangeDate.Value <= today)
                .ToListAsync();
        }

        public async Task AddAsync(ClinicSubscription subscription)
        {
            await _context.ClinicSubscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Adds a subscription to the context without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task AddWithoutSaveAsync(ClinicSubscription subscription)
        {
            await _context.ClinicSubscriptions.AddAsync(subscription);
        }

        /// <summary>
        /// Marks a subscription for deletion without immediately saving changes.
        /// Use this method when batching multiple operations within a transaction.
        /// </summary>
        public async Task DeleteWithoutSaveAsync(Guid id, string tenantId)
        {
            var subscription = await GetByIdAsync(id, tenantId);
            if (subscription != null)
            {
                _context.ClinicSubscriptions.Remove(subscription);
            }
        }

        /// <summary>
        /// Saves all pending changes to the database.
        /// </summary>
        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAsync(ClinicSubscription subscription)
        {
            _context.ClinicSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
