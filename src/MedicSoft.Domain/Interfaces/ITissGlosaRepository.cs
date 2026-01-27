using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissGlosaRepository : IRepository<TissGlosa>
    {
        Task<IEnumerable<TissGlosa>> GetByGuideIdAsync(Guid guideId, string tenantId);
        Task<IEnumerable<TissGlosa>> GetByGuideNumberAsync(string guideNumber, string tenantId);
        Task<IEnumerable<TissGlosa>> GetByStatusAsync(StatusGlosa status, string tenantId);
        Task<IEnumerable<TissGlosa>> GetByTipoAsync(TipoGlosa tipo, string tenantId);
        Task<IEnumerable<TissGlosa>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<TissGlosa?> GetWithRecursosAsync(Guid id, string tenantId);
        Task<IEnumerable<TissGlosa>> GetPendingRecursosAsync(string tenantId);
    }
}
