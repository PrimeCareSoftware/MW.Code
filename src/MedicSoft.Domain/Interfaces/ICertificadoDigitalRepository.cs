using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for CertificadoDigital entity.
    /// </summary>
    public interface ICertificadoDigitalRepository : IRepository<CertificadoDigital>
    {
        /// <summary>
        /// Gets the active certificate for a doctor.
        /// </summary>
        Task<CertificadoDigital?> GetCertificadoAtivoAsync(Guid medicoId);
        
        /// <summary>
        /// Gets all certificates for a doctor.
        /// </summary>
        Task<System.Collections.Generic.List<CertificadoDigital>> GetCertificadosPorMedicoAsync(Guid medicoId);
        
        /// <summary>
        /// Gets a certificate by thumbprint.
        /// </summary>
        Task<CertificadoDigital?> GetByThumbprintAsync(string thumbprint);
    }
}
