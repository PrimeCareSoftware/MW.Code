using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissBatchRepository : IRepository<TissBatch>
    {
        Task<TissBatch?> GetByBatchNumberAsync(string batchNumber, string tenantId);
        Task<IEnumerable<TissBatch>> GetByClinicIdAsync(Guid clinicId, string tenantId);
        Task<IEnumerable<TissBatch>> GetByOperatorIdAsync(Guid operatorId, string tenantId);
        Task<IEnumerable<TissBatch>> GetByStatusAsync(BatchStatus status, string tenantId);
        Task<TissBatch?> GetWithGuidesAsync(Guid id, string tenantId);
    }
}
