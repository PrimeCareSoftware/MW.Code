using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    public interface IMedicalRecordVersionService
    {
        Task<MedicalRecordVersion> CreateVersionAsync(
            Guid medicalRecordId, 
            string changeType, 
            Guid userId, 
            string tenantId,
            string? reason = null);
        
        Task<List<MedicalRecordVersion>> GetVersionHistoryAsync(Guid medicalRecordId, string tenantId);
        
        Task<MedicalRecordVersion?> GetVersionAsync(Guid medicalRecordId, int version, string tenantId);
        
        Task<string> GenerateContentHashAsync(MedicalRecord record);
        
        Task<string> GenerateChangesSummaryAsync(MedicalRecord? oldState, MedicalRecord newState);
    }
}
