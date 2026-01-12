using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITussProcedureRepository : IRepository<TussProcedure>
    {
        Task<TussProcedure?> GetByCodeAsync(string code, string tenantId);
        Task<IEnumerable<TussProcedure>> SearchByDescriptionAsync(string description, string tenantId);
        Task<IEnumerable<TussProcedure>> GetByCategoryAsync(string category, string tenantId);
        Task<IEnumerable<TussProcedure>> GetActiveAsync(string tenantId);
        Task<IEnumerable<TussProcedure>> GetRequiringAuthorizationAsync(string tenantId);
    }
}
