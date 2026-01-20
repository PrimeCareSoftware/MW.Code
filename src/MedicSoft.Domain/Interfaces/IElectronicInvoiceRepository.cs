using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IElectronicInvoiceRepository : IRepository<ElectronicInvoice>
    {
        Task<ElectronicInvoice?> GetByIdAsync(Guid id, string tenantId);
        Task<ElectronicInvoice?> GetByNumberAsync(string number, string series, string tenantId);
        Task<ElectronicInvoice?> GetByAccessKeyAsync(string accessKey);
        Task<ElectronicInvoice?> GetByPaymentIdAsync(Guid paymentId, string tenantId);
        Task<ElectronicInvoice?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<ElectronicInvoice>> GetByPeriodAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<IEnumerable<ElectronicInvoice>> GetByStatusAsync(ElectronicInvoiceStatus status, string tenantId);
        Task<IEnumerable<ElectronicInvoice>> GetByClientCpfCnpjAsync(string cpfCnpj, string tenantId);
        Task<IEnumerable<ElectronicInvoice>> GetPendingAuthorizationAsync(string tenantId);
        Task<IEnumerable<ElectronicInvoice>> GetAuthorizedInPeriodAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<decimal> GetTotalIssuedInPeriodAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<int> GetCountByStatusAsync(ElectronicInvoiceStatus status, string tenantId);
    }
}
