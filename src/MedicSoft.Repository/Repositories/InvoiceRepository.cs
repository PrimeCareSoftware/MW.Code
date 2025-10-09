using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class InvoiceRepository : BaseRepository<Invoice>, IInvoiceRepository
    {
        public InvoiceRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Invoice?> GetByPaymentIdAsync(Guid paymentId)
        {
            return await _context.Invoices
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.PaymentId == paymentId);
        }

        public async Task<Invoice?> GetByInvoiceNumberAsync(string invoiceNumber)
        {
            return await _context.Invoices
                .Include(i => i.Payment)
                .FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber);
        }

        public async Task<List<Invoice>> GetOverdueInvoicesAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _context.Invoices
                .Where(i => i.DueDate < today && 
                           i.Status != InvoiceStatus.Paid && 
                           i.Status != InvoiceStatus.Cancelled)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices
                .Where(i => i.Status == status)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<List<Invoice>> GetInvoicesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Invoices
                .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }
    }
}
