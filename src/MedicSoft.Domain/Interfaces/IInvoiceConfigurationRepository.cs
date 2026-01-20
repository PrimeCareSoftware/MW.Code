using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IInvoiceConfigurationRepository : IRepository<InvoiceConfiguration>
    {
        Task<InvoiceConfiguration?> GetByTenantIdAsync(string tenantId);
        Task<InvoiceConfiguration?> GetByCnpjAsync(string cnpj);
        Task<bool> ExistsForTenantAsync(string tenantId);
        Task<int> GetNextInvoiceNumberAsync(string tenantId);
        Task<int> GetNextRpsNumberAsync(string tenantId);
    }
}
