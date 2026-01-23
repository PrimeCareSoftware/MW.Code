using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface IMedicalRecordVersionRepository
    {
        Task<MedicalRecordVersion?> GetByIdAsync(Guid id, string tenantId);
        Task<List<MedicalRecordVersion>> GetVersionHistoryAsync(Guid medicalRecordId, string tenantId);
        Task<MedicalRecordVersion?> GetVersionAsync(Guid medicalRecordId, int version, string tenantId);
        Task<MedicalRecordVersion?> GetLatestVersionAsync(Guid medicalRecordId, string tenantId);
        Task<MedicalRecordVersion> CreateAsync(MedicalRecordVersion version);
        Task<List<MedicalRecordVersion>> GetAllAsync(string tenantId);
    }
}
