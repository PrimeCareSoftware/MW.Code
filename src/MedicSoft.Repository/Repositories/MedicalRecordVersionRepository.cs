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
    public class MedicalRecordVersionRepository : IMedicalRecordVersionRepository
    {
        private readonly MedicSoftDbContext _context;

        public MedicalRecordVersionRepository(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<MedicalRecordVersion?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _context.Set<MedicalRecordVersion>()
                .Where(v => v.Id == id && v.TenantId == tenantId)
                .Include(v => v.ChangedBy)
                .Include(v => v.MedicalRecord)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<List<MedicalRecordVersion>> GetVersionHistoryAsync(Guid medicalRecordId, string tenantId)
        {
            return await _context.Set<MedicalRecordVersion>()
                .Where(v => v.MedicalRecordId == medicalRecordId && v.TenantId == tenantId)
                .Include(v => v.ChangedBy)
                .AsNoTracking()
                .OrderByDescending(v => v.Version)
                .ToListAsync();
        }

        public async Task<MedicalRecordVersion?> GetVersionAsync(Guid medicalRecordId, int version, string tenantId)
        {
            return await _context.Set<MedicalRecordVersion>()
                .Where(v => v.MedicalRecordId == medicalRecordId 
                    && v.Version == version 
                    && v.TenantId == tenantId)
                .Include(v => v.ChangedBy)
                .Include(v => v.MedicalRecord)
                .AsNoTracking()
                .FirstOrDefaultAsync();
        }

        public async Task<MedicalRecordVersion?> GetLatestVersionAsync(Guid medicalRecordId, string tenantId)
        {
            return await _context.Set<MedicalRecordVersion>()
                .Where(v => v.MedicalRecordId == medicalRecordId && v.TenantId == tenantId)
                .Include(v => v.ChangedBy)
                .Include(v => v.MedicalRecord)
                .AsNoTracking()
                .OrderByDescending(v => v.Version)
                .FirstOrDefaultAsync();
        }

        public async Task<MedicalRecordVersion> CreateAsync(MedicalRecordVersion version)
        {
            await _context.Set<MedicalRecordVersion>().AddAsync(version);
            await _context.SaveChangesAsync();
            return version;
        }

        public async Task<List<MedicalRecordVersion>> GetAllAsync(string tenantId)
        {
            return await _context.Set<MedicalRecordVersion>()
                .Where(v => v.TenantId == tenantId)
                .Include(v => v.ChangedBy)
                .Include(v => v.MedicalRecord)
                .AsNoTracking()
                .OrderByDescending(v => v.ChangedAt)
                .ToListAsync();
        }
    }
}
