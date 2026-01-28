using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.Fiscal;

namespace MedicSoft.Domain.Services
{
    public interface IApuracaoImpostosService
    {
        /// <summary>
        /// Gera apuração mensal de impostos
        /// </summary>
        Task<ApuracaoImpostos> GerarApuracaoMensalAsync(Guid clinicaId, int mes, int ano, string tenantId);
        
        /// <summary>
        /// Atualiza status da apuração
        /// </summary>
        Task<ApuracaoImpostos> AtualizarStatusAsync(Guid apuracaoId, StatusApuracao novoStatus, string tenantId);
        
        /// <summary>
        /// Registra pagamento de apuração
        /// </summary>
        Task<ApuracaoImpostos> RegistrarPagamentoAsync(Guid apuracaoId, DateTime dataPagamento, string comprovante, string tenantId);
    }
}
