using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for AssinaturaDigital entity.
    /// </summary>
    public interface IAssinaturaDigitalRepository : IRepository<AssinaturaDigital>
    {
        /// <summary>
        /// Gets signatures for a specific document.
        /// </summary>
        Task<List<AssinaturaDigital>> GetAssinaturasPorDocumentoAsync(
            Guid documentoId, 
            TipoDocumento tipoDocumento);
        
        /// <summary>
        /// Gets signatures by a doctor.
        /// </summary>
        Task<List<AssinaturaDigital>> GetAssinaturasPorMedicoAsync(Guid medicoId);
        
        /// <summary>
        /// Gets the latest signature for a document.
        /// </summary>
        Task<AssinaturaDigital?> GetUltimaAssinaturaPorDocumentoAsync(
            Guid documentoId, 
            TipoDocumento tipoDocumento);
        
        /// <summary>
        /// Gets a signature by ID with related entities loaded.
        /// </summary>
        Task<AssinaturaDigital?> GetAssinaturaComRelacoesAsync(Guid assinaturaId);
    }
}
