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
    /// <summary>
    /// Repository implementation for CertificadoDigital entity.
    /// </summary>
    public class CertificadoDigitalRepository : BaseRepository<CertificadoDigital>, ICertificadoDigitalRepository
    {
        public CertificadoDigitalRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<CertificadoDigital?> GetCertificadoAtivoAsync(Guid medicoId)
        {
            return await _dbSet
                .Include(c => c.Medico)
                .Where(c => c.MedicoId == medicoId && c.Valido && c.DataExpiracao > DateTime.UtcNow)
                .OrderByDescending(c => c.DataCadastro)
                .FirstOrDefaultAsync();
        }

        public async Task<List<CertificadoDigital>> GetCertificadosPorMedicoAsync(Guid medicoId)
        {
            return await _dbSet
                .Include(c => c.Medico)
                .Where(c => c.MedicoId == medicoId)
                .OrderByDescending(c => c.DataCadastro)
                .ToListAsync();
        }

        public async Task<CertificadoDigital?> GetByThumbprintAsync(string thumbprint)
        {
            return await _dbSet
                .Include(c => c.Medico)
                .FirstOrDefaultAsync(c => c.Thumbprint == thumbprint);
        }
    }
}
