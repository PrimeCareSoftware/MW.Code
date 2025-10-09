using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IInvoiceRepository : IRepository<Invoice>
    {
        Task<Invoice?> GetByPaymentIdAsync(Guid paymentId);
        Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber);
        Task<List<Invoice>> GetOverdueInvoicesAsync();
        Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
        Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
