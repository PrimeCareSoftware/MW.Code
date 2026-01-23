using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    public interface IMedicalRecordAuditService
    {
        Task LogAccessAsync(
            Guid recordId, 
            Guid userId, 
            string accessType, 
            string tenantId,
            string? ipAddress = null, 
            string? userAgent = null, 
            string? details = null);
        
        Task<List<MedicalRecordAccessLog>> GetAccessLogsAsync(
            Guid recordId, 
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null);
        
        Task<List<MedicalRecordAccessLog>> GetUserAccessLogsAsync(
            Guid userId, 
            string tenantId, 
            DateTime? startDate = null, 
            DateTime? endDate = null);
    }
}
