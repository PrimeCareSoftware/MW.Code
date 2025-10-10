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
    public class PaymentRepository : BaseRepository<Payment>, IPaymentRepository
    {
        public PaymentRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<List<Payment>> GetByAppointmentIdAsync(Guid appointmentId)
        {
            return await _context.Payments
                .Where(p => p.AppointmentId == appointmentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByClinicSubscriptionIdAsync(Guid subscriptionId)
        {
            return await _context.Payments
                .Where(p => p.ClinicSubscriptionId == subscriptionId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPendingPaymentsAsync()
        {
            return await _context.Payments
                .Where(p => p.Status == PaymentStatus.Pending || p.Status == PaymentStatus.Processing)
                .OrderBy(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            return await _context.Payments
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetPaymentsByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _context.Payments
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }
    }
}
