using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class AuditRepository : IAuditRepository
    {
        private readonly MedicSoftDbContext _context;

        public AuditRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(AuditLog auditLog)
        {
            try
            {
                if (_context.ChangeTracker.HasChanges())
                {
                    _context.ChangeTracker.Clear();
                }

                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Clear stale tracked entities and retry once to avoid breaking the request pipeline
                _context.ChangeTracker.Clear();
                await _context.AuditLogs.AddAsync(auditLog);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<AuditLog>> QueryAsync(AuditFilter filter)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(a => a.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.TenantId))
                query = query.Where(a => a.TenantId == filter.TenantId);

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(a => a.EntityType == filter.EntityType);

            if (!string.IsNullOrEmpty(filter.EntityId))
                query = query.Where(a => a.EntityId == filter.EntityId);

            if (filter.Action.HasValue)
                query = query.Where(a => a.Action == filter.Action.Value);

            if (filter.Result.HasValue)
                query = query.Where(a => a.Result == filter.Result.Value);

            if (filter.Severity.HasValue)
                query = query.Where(a => a.Severity == filter.Severity.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .ToListAsync();
        }

        public async Task<int> CountAsync(AuditFilter filter)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(a => a.Timestamp >= filter.StartDate.Value);

            if (filter.EndDate.HasValue)
                query = query.Where(a => a.Timestamp <= filter.EndDate.Value);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(a => a.UserId == filter.UserId);

            if (!string.IsNullOrEmpty(filter.TenantId))
                query = query.Where(a => a.TenantId == filter.TenantId);

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(a => a.EntityType == filter.EntityType);

            if (!string.IsNullOrEmpty(filter.EntityId))
                query = query.Where(a => a.EntityId == filter.EntityId);

            if (filter.Action.HasValue)
                query = query.Where(a => a.Action == filter.Action.Value);

            if (filter.Result.HasValue)
                query = query.Where(a => a.Result == filter.Result.Value);

            if (filter.Severity.HasValue)
                query = query.Where(a => a.Severity == filter.Severity.Value);

            return await query.CountAsync();
        }

        public async Task<List<AuditLog>> GetByUserIdAsync(
            string userId,
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var query = _context.AuditLogs
                .Where(a => a.UserId == userId && a.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetByEntityAsync(
            string entityType,
            string entityId,
            string tenantId)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId && a.TenantId == tenantId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<List<AuditLog>> GetSecurityEventsAsync(
            string tenantId,
            DateTime? startDate = null,
            DateTime? endDate = null)
        {
            var securitySeverities = new[] { AuditSeverity.WARNING, AuditSeverity.ERROR, AuditSeverity.CRITICAL };
            var securityActions = new[]
            {
                AuditAction.LOGIN_FAILED,
                AuditAction.ACCESS_DENIED,
                AuditAction.PERMISSION_CHANGED,
                AuditAction.ROLE_CHANGED
            };

            var query = _context.AuditLogs
                .Where(a => a.TenantId == tenantId &&
                            (securitySeverities.Contains(a.Severity) || securityActions.Contains(a.Action)));

            if (startDate.HasValue)
                query = query.Where(a => a.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(a => a.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }
    }

    public class DataProcessingConsentRepository : IDataProcessingConsentRepository
    {
        private readonly MedicSoftDbContext _context;

        public DataProcessingConsentRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(DataProcessingConsent consent)
        {
            await _context.DataProcessingConsents.AddAsync(consent);
            await _context.SaveChangesAsync();
        }

        public async Task<DataProcessingConsent?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.DataProcessingConsents
                .FirstOrDefaultAsync(c => c.Id == id && c.TenantId == tenantId);
        }

        public async Task<List<DataProcessingConsent>> GetByUserIdAsync(string userId, string tenantId)
        {
            return await _context.DataProcessingConsents
                .Where(c => c.UserId == userId && c.TenantId == tenantId)
                .OrderByDescending(c => c.ConsentDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(DataProcessingConsent consent)
        {
            _context.DataProcessingConsents.Update(consent);
            await _context.SaveChangesAsync();
        }
    }
}
