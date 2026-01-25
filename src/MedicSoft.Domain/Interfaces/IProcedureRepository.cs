using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IProcedureRepository : IRepository<Procedure>
    {
        Task<Procedure?> GetByCodeAsync(string code, string tenantId);
        Task<IEnumerable<Procedure>> GetByClinicAsync(string tenantId, bool activeOnly = true);
        Task<IEnumerable<Procedure>> GetByCategoryAsync(ProcedureCategory category, string tenantId, bool activeOnly = true);
        Task<bool> IsCodeUniqueAsync(string code, string tenantId, Guid? excludeId = null);
        Task<IEnumerable<Procedure>> GetByOwnerAsync(Guid ownerId, bool activeOnly = true);
    }
}
