using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class DataAccessLogRepository : IDataAccessLogRepository
    {
        private readonly MedicSoftDbContext _context;

        public DataAccessLogRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(DataAccessLog log)
        {
            await _context.DataAccessLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<List<DataAccessLog>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _context.DataAccessLogs
                .Where(l => l.PatientId == patientId && l.TenantId == tenantId)
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<List<DataAccessLog>> GetByUserIdAsync(
            string userId, 
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _context.DataAccessLogs
                .Where(l => l.UserId == userId && l.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(l => l.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }

        public async Task<List<DataAccessLog>> GetUnauthorizedAccessesAsync(
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _context.DataAccessLogs
                .Where(l => !l.WasAuthorized && l.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(l => l.Timestamp >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.Timestamp <= endDate.Value);

            return await query
                .OrderByDescending(l => l.Timestamp)
                .ToListAsync();
        }
    }

    public class DataConsentLogRepository : IDataConsentLogRepository
    {
        private readonly MedicSoftDbContext _context;

        public DataConsentLogRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(DataConsentLog log)
        {
            await _context.DataConsentLogs.AddAsync(log);
            await _context.SaveChangesAsync();
        }

        public async Task<DataConsentLog?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.DataConsentLogs
                .FirstOrDefaultAsync(l => l.Id == id && l.TenantId == tenantId);
        }

        public async Task<List<DataConsentLog>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _context.DataConsentLogs
                .Where(l => l.PatientId == patientId && l.TenantId == tenantId)
                .OrderByDescending(l => l.ConsentDate)
                .ToListAsync();
        }

        public async Task<List<DataConsentLog>> GetActiveConsentsByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _context.DataConsentLogs
                .Where(l => l.PatientId == patientId && 
                           l.TenantId == tenantId && 
                           l.Status == ConsentStatus.Active &&
                           (l.ExpirationDate == null || l.ExpirationDate > DateTime.UtcNow))
                .ToListAsync();
        }

        public async Task UpdateAsync(DataConsentLog log)
        {
            _context.DataConsentLogs.Update(log);
            await _context.SaveChangesAsync();
        }
    }

    public class DataDeletionRequestRepository : IDataDeletionRequestRepository
    {
        private readonly MedicSoftDbContext _context;

        public DataDeletionRequestRepository(MedicSoftDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(DataDeletionRequest request)
        {
            await _context.DataDeletionRequests.AddAsync(request);
            await _context.SaveChangesAsync();
        }

        public async Task<DataDeletionRequest?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.DataDeletionRequests
                .FirstOrDefaultAsync(r => r.Id == id && r.TenantId == tenantId);
        }

        public async Task<List<DataDeletionRequest>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _context.DataDeletionRequests
                .Where(r => r.PatientId == patientId && r.TenantId == tenantId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<DataDeletionRequest>> GetPendingRequestsAsync(string tenantId)
        {
            return await _context.DataDeletionRequests
                .Where(r => r.Status == DeletionRequestStatus.Pending && r.TenantId == tenantId)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task UpdateAsync(DataDeletionRequest request)
        {
            _context.DataDeletionRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
}
