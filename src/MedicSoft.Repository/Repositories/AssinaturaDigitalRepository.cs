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
    /// Repository implementation for AssinaturaDigital entity.
    /// </summary>
    public class AssinaturaDigitalRepository : BaseRepository<AssinaturaDigital>, IAssinaturaDigitalRepository
    {
        public AssinaturaDigitalRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<List<AssinaturaDigital>> GetAssinaturasPorDocumentoAsync(
            Guid documentoId, 
            TipoDocumento tipoDocumento)
        {
            return await _dbSet
                .Include(a => a.Medico)
                .Include(a => a.Certificado)
                .Where(a => a.DocumentoId == documentoId && a.TipoDocumento == tipoDocumento)
                .OrderByDescending(a => a.DataHoraAssinatura)
                .ToListAsync();
        }

        public async Task<List<AssinaturaDigital>> GetAssinaturasPorMedicoAsync(Guid medicoId)
        {
            return await _dbSet
                .Include(a => a.Certificado)
                .Where(a => a.MedicoId == medicoId)
                .OrderByDescending(a => a.DataHoraAssinatura)
                .ToListAsync();
        }

        public async Task<AssinaturaDigital?> GetUltimaAssinaturaPorDocumentoAsync(
            Guid documentoId, 
            TipoDocumento tipoDocumento)
        {
            return await _dbSet
                .Include(a => a.Medico)
                .Include(a => a.Certificado)
                .Where(a => a.DocumentoId == documentoId && a.TipoDocumento == tipoDocumento)
                .OrderByDescending(a => a.DataHoraAssinatura)
                .FirstOrDefaultAsync();
        }

        public async Task<AssinaturaDigital?> GetAssinaturaComRelacoesAsync(Guid assinaturaId)
        {
            return await _dbSet
                .Include(a => a.Medico)
                .Include(a => a.Certificado)
                .FirstOrDefaultAsync(a => a.Id == assinaturaId);
        }
    }
}
