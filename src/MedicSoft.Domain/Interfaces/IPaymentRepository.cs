using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<List<Payment>> GetByAppointmentIdAsync(Guid appointmentId);
        Task<List<Payment>> GetByClinicSubscriptionIdAsync(Guid subscriptionId);
        Task<List<Payment>> GetPendingPaymentsAsync();
        Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status);
        Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate);
    }
}
