using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Interfaces
{
    public interface IAuditRepository
    {
        Task AddAsync(AuditLog auditLog);
        Task<List<AuditLog>> QueryAsync(AuditFilter filter);
        Task<int> CountAsync(AuditFilter filter);
        Task<List<AuditLog>> GetByUserIdAsync(string userId, string tenantId, DateTime? startDate = null, DateTime? endDate = null);
        Task<List<AuditLog>> GetByEntityAsync(string entityType, string entityId, string tenantId);
        Task<List<AuditLog>> GetSecurityEventsAsync(string tenantId, DateTime? startDate = null, DateTime? endDate = null);
    }

    public interface IDataProcessingConsentRepository
    {
        Task AddAsync(DataProcessingConsent consent);
        Task<DataProcessingConsent?> GetByIdAsync(Guid id, string tenantId);
        Task<List<DataProcessingConsent>> GetByUserIdAsync(string userId, string tenantId);
        Task UpdateAsync(DataProcessingConsent consent);
    }
}
