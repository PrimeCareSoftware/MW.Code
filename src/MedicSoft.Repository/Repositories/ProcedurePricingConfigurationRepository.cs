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
    public class ProcedurePricingConfigurationRepository : BaseRepository<ProcedurePricingConfiguration>, IProcedurePricingConfigurationRepository
    {
        public ProcedurePricingConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ProcedurePricingConfiguration?> GetByProcedureAndClinicAsync(Guid procedureId, Guid clinicId, string tenantId)
        {
            return await _context.ProcedurePricingConfigurations
                .FirstOrDefaultAsync(p => p.ProcedureId == procedureId && 
                                        p.ClinicId == clinicId && 
                                        p.TenantId == tenantId);
        }

        public async Task<IEnumerable<ProcedurePricingConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.ProcedurePricingConfigurations
                .Where(p => p.ClinicId == clinicId && p.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<IEnumerable<ProcedurePricingConfiguration>> GetByProcedureIdAsync(Guid procedureId, string tenantId)
        {
            return await _context.ProcedurePricingConfigurations
                .Where(p => p.ProcedureId == procedureId && p.TenantId == tenantId)
                .ToListAsync();
        }
    }
}
