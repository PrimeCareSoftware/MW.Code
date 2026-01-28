using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface de repositório para DRE
    /// </summary>
    public interface IDRERepository
    {
        /// <summary>
        /// Adiciona uma nova DRE
        /// </summary>
        Task<DRE> AddAsync(DRE dre);

        /// <summary>
        /// Busca uma DRE por ID
        /// </summary>
        Task<DRE?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Busca DRE de um período específico
        /// </summary>
        Task<DRE?> GetByPeriodoAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId);

        /// <summary>
        /// Atualiza uma DRE existente
        /// </summary>
        Task UpdateAsync(DRE dre);
    }
}
