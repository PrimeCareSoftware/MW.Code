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
    public class MedicalRecordAccessLogRepository : IMedicalRecordAccessLogRepository
    {
        private readonly MedicSoftDbContext _context;

        public MedicalRecordAccessLogRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecordAccessLog> CreateAsync(MedicalRecordAccessLog log)
        {
            await _context.Set<MedicalRecordAccessLog>().AddAsync(log);
            await _context.SaveChangesAsync();
            return log;
        }

        public async Task<List<MedicalRecordAccessLog>> GetAccessLogsAsync(Guid medicalRecordId, string tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Set<MedicalRecordAccessLog>()
                .Include(l => l.User)
                .Where(l => l.MedicalRecordId == medicalRecordId && l.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(l => l.AccessedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.AccessedAt <= endDate.Value);

            return await query
                .OrderByDescending(l => l.AccessedAt)
                .ToListAsync();
        }

        public async Task<List<MedicalRecordAccessLog>> GetUserAccessLogsAsync(Guid userId, string tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Set<MedicalRecordAccessLog>()
                .Include(l => l.User)
                .Include(l => l.MedicalRecord)
                .Where(l => l.UserId == userId && l.TenantId == tenantId);

            if (startDate.HasValue)
                query = query.Where(l => l.AccessedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(l => l.AccessedAt <= endDate.Value);

            return await query
                .OrderByDescending(l => l.AccessedAt)
                .ToListAsync();
        }

        public async Task<List<MedicalRecordAccessLog>> GetAllAsync(string tenantId)
        {
            return await _context.Set<MedicalRecordAccessLog>()
                .Include(l => l.User)
                .Include(l => l.MedicalRecord)
                .Where(l => l.TenantId == tenantId)
                .OrderByDescending(l => l.AccessedAt)
                .ToListAsync();
        }
    }
}
