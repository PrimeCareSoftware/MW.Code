using MedicSoft.Telemedicine.Domain.Entities;

namespace MedicSoft.Telemedicine.Domain.Interfaces;

/// <summary>
/// Repository interface for telemedicine recordings
/// </summary>
public interface ITelemedicineRecordingRepository
{
    Task<TelemedicineRecording?> GetByIdAsync(Guid id, string tenantId);
    Task<TelemedicineRecording?> GetBySessionIdAsync(Guid sessionId, string tenantId);
    Task<IEnumerable<TelemedicineRecording>> GetByTenantIdAsync(string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<TelemedicineRecording>> GetAvailableRecordingsAsync(string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<TelemedicineRecording>> GetRecordingsDueForDeletionAsync(string tenantId, int skip = 0, int take = 50);
    Task<long> GetTotalStorageSizeAsync(string tenantId);
    Task AddAsync(TelemedicineRecording recording);
    Task UpdateAsync(TelemedicineRecording recording);
    Task DeleteAsync(Guid id, string tenantId);
}
