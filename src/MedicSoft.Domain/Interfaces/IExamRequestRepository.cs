using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IExamRequestRepository : IRepository<ExamRequest>
    {
        Task<IEnumerable<ExamRequest>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<ExamRequest>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<ExamRequest>> GetByStatusAsync(ExamRequestStatus status, string tenantId);
        Task<IEnumerable<ExamRequest>> GetPendingExamsAsync(string tenantId);
        Task<IEnumerable<ExamRequest>> GetUrgentExamsAsync(string tenantId);
    }
}
