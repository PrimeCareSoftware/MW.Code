using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISenhaFilaRepository : IRepository<SenhaFila>
    {
        Task<SenhaFila?> GetByIdAsync(Guid id, string tenantId);
        Task<SenhaFila?> GetByNumeroSenhaAsync(string numeroSenha, Guid filaId, string tenantId);
        Task<List<SenhaFila>> GetByFilaIdAsync(Guid filaId, string tenantId);
        Task<List<SenhaFila>> GetActiveSenhasByFilaAsync(Guid filaId, string tenantId);
        Task<List<SenhaFila>> GetAguardandoByFilaAsync(Guid filaId, string tenantId);
        Task<SenhaFila?> GetProximaSenhaAsync(Guid filaId, string tenantId);
        Task<List<SenhaFila>> GetProximasSenhasAsync(Guid filaId, int quantidade, string tenantId);
        Task<List<SenhaFila>> GetSenhasByDataAsync(DateTime data, Guid clinicaId, string tenantId);
        Task<int> GetPosicaoNaFilaAsync(Guid senhaId, string tenantId);
        Task<int> CountSenhasAFrenteAsync(Guid senhaId, string tenantId);
    }
}
