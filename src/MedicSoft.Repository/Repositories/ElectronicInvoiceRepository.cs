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
    public class ElectronicInvoiceRepository : BaseRepository<ElectronicInvoice>, IElectronicInvoiceRepository
    {
        public ElectronicInvoiceRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ElectronicInvoice?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Id == id && i.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<ElectronicInvoice?> GetByNumberAsync(string number, string series, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Number == number && i.Series == series && i.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<ElectronicInvoice?> GetByAccessKeyAsync(string accessKey)
        {
            return await _dbSet
                .Where(i => i.AccessKey == accessKey)
                .FirstOrDefaultAsync();
        }

        public async Task<ElectronicInvoice?> GetByPaymentIdAsync(Guid paymentId, string tenantId)
        {
            return await _dbSet
                .Where(i => i.PaymentId == paymentId && i.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<ElectronicInvoice?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(i => i.AppointmentId == appointmentId && i.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ElectronicInvoice>> GetByPeriodAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(i => i.IssueDate >= startDate && 
                           i.IssueDate <= endDate && 
                           i.TenantId == tenantId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElectronicInvoice>> GetByStatusAsync(ElectronicInvoiceStatus status, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Status == status && i.TenantId == tenantId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElectronicInvoice>> GetByClientCpfCnpjAsync(string cpfCnpj, string tenantId)
        {
            return await _dbSet
                .Where(i => i.ClientCpfCnpj == cpfCnpj && i.TenantId == tenantId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElectronicInvoice>> GetPendingAuthorizationAsync(string tenantId)
        {
            return await _dbSet
                .Where(i => i.Status == ElectronicInvoiceStatus.PendingAuthorization && 
                           i.TenantId == tenantId)
                .OrderBy(i => i.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ElectronicInvoice>> GetAuthorizedInPeriodAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Status == ElectronicInvoiceStatus.Authorized &&
                           i.AuthorizationDate >= startDate &&
                           i.AuthorizationDate <= endDate &&
                           i.TenantId == tenantId)
                .OrderByDescending(i => i.AuthorizationDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIssuedInPeriodAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Status == ElectronicInvoiceStatus.Authorized &&
                           i.IssueDate >= startDate &&
                           i.IssueDate <= endDate &&
                           i.TenantId == tenantId)
                .SumAsync(i => i.ServiceAmount);
        }

        public async Task<int> GetCountByStatusAsync(ElectronicInvoiceStatus status, string tenantId)
        {
            return await _dbSet
                .Where(i => i.Status == status && i.TenantId == tenantId)
                .CountAsync();
        }
    }
}
