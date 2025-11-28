using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IExamCatalogRepository : IRepository<ExamCatalog>
    {
        Task<ExamCatalog?> GetByNameAsync(string name, string tenantId);
        Task<IEnumerable<ExamCatalog>> SearchByNameAsync(string searchTerm, string tenantId);
        Task<IEnumerable<ExamCatalog>> GetByExamTypeAsync(ExamType examType, string tenantId);
        Task<IEnumerable<ExamCatalog>> GetByCategoryAsync(string category, string tenantId);
        Task<IEnumerable<ExamCatalog>> GetActiveAsync(string tenantId);
        Task<bool> IsNameUniqueAsync(string name, string tenantId, Guid? excludeId = null);
    }
}
