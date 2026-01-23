using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repository for managing anamnesis templates with specialty-based filtering
    /// </summary>
    public class AnamnesisTemplateRepository : BaseRepository<AnamnesisTemplate>, IAnamnesisTemplateRepository
    {
        public AnamnesisTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<List<AnamnesisTemplate>> GetBySpecialtyAsync(MedicalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Specialty == specialty && t.TenantId == tenantId && t.IsActive)
                .OrderByDescending(t => t.IsDefault)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<AnamnesisTemplate?> GetDefaultTemplateAsync(MedicalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Specialty == specialty && t.TenantId == tenantId && t.IsDefault && t.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<List<AnamnesisTemplate>> GetActiveTemplatesAsync(string tenantId)
        {
            return await _dbSet
                .Where(t => t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Specialty)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }
    }
}
