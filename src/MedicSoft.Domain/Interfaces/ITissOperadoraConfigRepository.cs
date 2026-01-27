using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissOperadoraConfigRepository : IRepository<TissOperadoraConfig>
    {
        Task<TissOperadoraConfig?> GetByOperatorIdAsync(Guid operatorId, string tenantId);
        Task<IEnumerable<TissOperadoraConfig>> GetActiveConfigsAsync(string tenantId);
    }
}
