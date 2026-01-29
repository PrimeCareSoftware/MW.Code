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
    public class FilaEsperaRepository : BaseRepository<FilaEspera>, IFilaEsperaRepository
    {
        public FilaEsperaRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public override async Task<FilaEspera?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Include(f => f.Clinica)
                .Include(f => f.Senhas)
                .Where(f => f.Id == id && f.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<FilaEspera?> GetByClinicaIdAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Include(f => f.Clinica)
                .Where(f => f.ClinicaId == clinicaId && f.Ativa && f.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<List<FilaEspera>> GetActiveByClinicaAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Include(f => f.Clinica)
                .Where(f => f.ClinicaId == clinicaId && f.Ativa && f.TenantId == tenantId)
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }

        public async Task<List<FilaEspera>> GetAllByClinicaAsync(Guid clinicaId, string tenantId)
        {
            return await _dbSet
                .Include(f => f.Clinica)
                .Where(f => f.ClinicaId == clinicaId && f.TenantId == tenantId)
                .OrderBy(f => f.Nome)
                .ToListAsync();
        }
    }
}
