using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissRecursoGlosaRepository : IRepository<TissRecursoGlosa>
    {
        Task<IEnumerable<TissRecursoGlosa>> GetByGlosaIdAsync(Guid glosaId, string tenantId);
        Task<IEnumerable<TissRecursoGlosa>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<IEnumerable<TissRecursoGlosa>> GetPendingResponseAsync(string tenantId);
        Task<IEnumerable<TissRecursoGlosa>> GetByResultadoAsync(ResultadoRecurso resultado, string tenantId);
    }
}
