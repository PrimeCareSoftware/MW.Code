using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissGuideProcedureRepository : IRepository<TissGuideProcedure>
    {
        Task<IEnumerable<TissGuideProcedure>> GetByGuideIdAsync(Guid guideId, string tenantId);
        Task<IEnumerable<TissGuideProcedure>> GetByProcedureCodeAsync(string procedureCode, string tenantId);
    }
}
