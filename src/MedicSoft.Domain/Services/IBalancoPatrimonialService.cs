using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Services
{
    /// <summary>
    /// Interface para serviço de geração de Balanço Patrimonial
    /// </summary>
    public interface IBalancoPatrimonialService
    {
        /// <summary>
        /// Gera um Balanço Patrimonial para uma data de referência
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="dataReferencia">Data de referência para o balanço</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Balanço Patrimonial gerado</returns>
        Task<BalancoPatrimonial> GerarBalancoAsync(Guid clinicaId, DateTime dataReferencia, string tenantId);

        /// <summary>
        /// Busca um Balanço Patrimonial específico por ID
        /// </summary>
        /// <param name="id">ID do balanço</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Balanço encontrado ou null</returns>
        Task<BalancoPatrimonial?> ObterBalancoAsync(Guid id, string tenantId);

        /// <summary>
        /// Busca Balanço Patrimonial de uma data de referência
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="dataReferencia">Data de referência</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>Balanço da data ou null</returns>
        Task<BalancoPatrimonial?> ObterBalancoPorDataAsync(Guid clinicaId, DateTime dataReferencia, string tenantId);
    }
}
