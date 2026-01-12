using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ITissGuideRepository : IRepository<TissGuide>
    {
        Task<TissGuide?> GetByGuideNumberAsync(string guideNumber, string tenantId);
        Task<IEnumerable<TissGuide>> GetByBatchIdAsync(Guid batchId, string tenantId);
        Task<IEnumerable<TissGuide>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<TissGuide>> GetByStatusAsync(GuideStatus status, string tenantId);
        Task<TissGuide?> GetWithProceduresAsync(Guid id, string tenantId);
    }
}
