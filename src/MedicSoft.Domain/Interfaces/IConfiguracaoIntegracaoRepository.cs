using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repositório para configurações de integração contábil
    /// </summary>
    public interface IConfiguracaoIntegracaoRepository : IRepository<ConfiguracaoIntegracao>
    {
        /// <summary>
        /// Obtém a configuração ativa de integração para uma clínica
        /// </summary>
        Task<ConfiguracaoIntegracao?> ObterConfiguracaoAtivaAsync(Guid clinicaId);

        /// <summary>
        /// Atualiza a data de última sincronização
        /// </summary>
        Task AtualizarUltimaSincronizacaoAsync(Guid clinicaId, DateTime data);

        /// <summary>
        /// Registra erro de integração
        /// </summary>
        Task RegistrarErroAsync(Guid clinicaId, string mensagem);

        /// <summary>
        /// Limpa contadores de erro após sucesso
        /// </summary>
        Task LimparErrosAsync(Guid clinicaId);
    }
}
