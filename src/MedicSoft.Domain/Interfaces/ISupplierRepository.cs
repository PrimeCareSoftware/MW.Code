using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISupplierRepository
    {
        Task<Supplier?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<Supplier>> GetAllAsync(string tenantId);
        Task<IEnumerable<Supplier>> GetActiveAsync(string tenantId);
        Task<Supplier?> GetByDocumentNumberAsync(string documentNumber, string tenantId);
        Task<Supplier?> GetByNameAsync(string name, string tenantId);
        Task<Supplier> AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(Guid id, string tenantId);
    }
}
