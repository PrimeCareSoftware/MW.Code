using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Interface de repositório para Balanço Patrimonial
    /// </summary>
    public interface IBalancoPatrimonialRepository
    {
        /// <summary>
        /// Adiciona um novo Balanço Patrimonial
        /// </summary>
        Task<BalancoPatrimonial> AddAsync(BalancoPatrimonial balanco);

        /// <summary>
        /// Busca um Balanço por ID
        /// </summary>
        Task<BalancoPatrimonial?> GetByIdAsync(Guid id, string tenantId);

        /// <summary>
        /// Busca Balanço de uma data de referência específica
        /// </summary>
        Task<BalancoPatrimonial?> GetByDataReferenciaAsync(Guid clinicaId, DateTime dataReferencia, string tenantId);

        /// <summary>
        /// Atualiza um Balanço existente
        /// </summary>
        Task UpdateAsync(BalancoPatrimonial balanco);
    }
}
