using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMedicalRecordAccessLogRepository
    {
        Task<MedicalRecordAccessLog> CreateAsync(MedicalRecordAccessLog log);
        Task<List<MedicalRecordAccessLog>> GetAccessLogsAsync(Guid medicalRecordId, string tenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MedicalRecordAccessLog>> GetUserAccessLogsAsync(Guid userId, string tenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<MedicalRecordAccessLog>> GetAllAsync(string tenantId);
    }
}
