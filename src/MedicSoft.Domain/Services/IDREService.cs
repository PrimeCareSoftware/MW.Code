using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Services
{
    /// <summary>
    /// Interface para serviço de geração de DRE (Demonstração do Resultado do Exercício)
    /// </summary>
    public interface IDREService
    {
        /// <summary>
        /// Gera uma DRE para o período especificado
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="dataInicio">Data de início do período</param>
        /// <param name="dataFim">Data de fim do período</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>DRE gerada</returns>
        Task<DRE> GerarDREAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId);

        /// <summary>
        /// Busca uma DRE específica por ID
        /// </summary>
        /// <param name="id">ID da DRE</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>DRE encontrada ou null</returns>
        Task<DRE?> ObterDREAsync(Guid id, string tenantId);

        /// <summary>
        /// Busca DRE de um período específico
        /// </summary>
        /// <param name="clinicaId">ID da clínica</param>
        /// <param name="dataInicio">Data de início</param>
        /// <param name="dataFim">Data de fim</param>
        /// <param name="tenantId">ID do tenant</param>
        /// <returns>DRE do período ou null</returns>
        Task<DRE?> ObterDREPorPeriodoAsync(Guid clinicaId, DateTime dataInicio, DateTime dataFim, string tenantId);
    }
}
